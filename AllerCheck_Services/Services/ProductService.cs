using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AllerCheck_Core.Entities;
using AllerCheck_Data.Repositories.Interfaces;
using AllerCheck_Services.Services.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using AllerCheck_Data.Context;

namespace AllerCheck_Services.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        private readonly AllerCheckDbContext _context;

        public ProductService(IProductRepository productRepository, IMapper mapper, AllerCheckDbContext context)
        {
            _productRepository = productRepository;
            _mapper = mapper;
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetAllProductsWithDetailsAsync()
        {
            return await _productRepository.GetAllProductsWithDetailsAsync();
        }

        public async Task<IEnumerable<Product>> SearchProductsAsync(string query)
        {
            return await _productRepository.SearchProductsAsync(query);
        }

        public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId)
        {
            return await _productRepository.GetProductsByCategoryAsync(categoryId);
        }

        public async Task<IEnumerable<Producer>> GetAllProducersAsync()
        {
            return await _context.Producers.ToListAsync();
        }

        public async Task<IEnumerable<Content>> GetAllContentsWithRiskStatusAsync()
        {
            return await _context.Contents
                .Include(c => c.RiskStatus)
                .ToListAsync();
        }

        public async Task<bool> CreateProductWithContentsAsync(Product product, List<int> existingContentIds, List<(string name, int riskStatusId)> newContents)
        {
            try
            {
                // Önce yeni içerikleri oluştur
                var allContentIds = new List<int>();
                allContentIds.AddRange(existingContentIds);

                if (newContents != null && newContents.Any())
                {
                    foreach (var (name, riskStatusId) in newContents)
                    {
                        var newContent = new Content
                        {
                            ContentName = name,
                            RiskStatusId = riskStatusId,
                            ContentInfo = "Kullanıcı tarafından eklendi"
                        };

                        await _context.Contents.AddAsync(newContent);
                        await _context.SaveChangesAsync();
                        allContentIds.Add(newContent.ContentId);
                    }
                }

                // Şimdi ürünü ve içerik ilişkilerini oluştur
                await _context.Products.AddAsync(product);
                await _context.SaveChangesAsync();

                if (allContentIds.Any())
                {
                    foreach (var contentId in allContentIds)
                    {
                        var contentProduct = new ContentProduct
                        {
                            ProductId = product.ProductId,
                            ContentId = contentId
                        };
                        await _context.ContentProducts.AddAsync(contentProduct);
                    }
                    await _context.SaveChangesAsync();
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<Product> GetProductWithContentsAsync(int productId)
        {
            return await _productRepository.GetProductWithContentsAsync(productId);
        }

        public async Task<bool> UpdateProductWithContentsAsync(Product product, List<int> selectedContents)
        {
            return await _productRepository.UpdateProductWithContentsAsync(product, selectedContents);
        }

        public async Task<bool> CreateContentAsync(Content content)
        {
            try
            {
                await _context.Contents.AddAsync(content);
                return await _context.SaveChangesAsync() > 0;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateContentAsync(Content content)
        {
            try
            {
                var existingContent = await _context.Contents.FindAsync(content.ContentId);
                if (existingContent == null) return false;

                existingContent.ContentName = content.ContentName;
                existingContent.RiskStatusId = content.RiskStatusId;
                existingContent.ContentInfo = content.ContentInfo;

                return await _context.SaveChangesAsync() > 0;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteContentAsync(int id)
        {
            try
            {
                // Önce içeriğin kullanımda olup olmadığını kontrol et
                var contentProducts = await _context.ContentProducts
                    .Where(cp => cp.ContentId == id)
                    .Select(cp => new { cp.Product.ProductName })
                    .ToListAsync();

                if (contentProducts.Any())
                {
                    var productNames = string.Join("\n", contentProducts.Select(cp => $"- {cp.ProductName}"));
                    throw new InvalidOperationException($"Bu içerik aşağıdaki ürünlerde kullanılmaktadır:\n{productNames}\n\nÖnce bu ürünlerden içeriği kaldırmanız gerekmektedir.");
                }

                // İçeriği bul
                var content = await _context.Contents
                    .Include(c => c.BlackLists)
                    .FirstOrDefaultAsync(c => c.ContentId == id);

                if (content == null)
                {
                    throw new InvalidOperationException("İçerik bulunamadı.");
                }

                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        // Önce kara listelerden kaldır
                        if (content.BlackLists.Any())
                        {
                            _context.BlackLists.RemoveRange(content.BlackLists);
                            await _context.SaveChangesAsync();
                        }

                        // İçeriği sil
                        _context.Contents.Remove(content);
                        await _context.SaveChangesAsync();

                        await transaction.CommitAsync();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        throw new Exception($"İçerik silinirken bir hata oluştu: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
