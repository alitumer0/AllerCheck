using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AllerCheck_Core.Entities;
using AllerCheck_Data.Repositories.Interfaces;
using AllerCheck_Data.Context;
using Microsoft.EntityFrameworkCore;

namespace AllerCheck_Data.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AllerCheckDbContext _context;

        public ProductRepository(AllerCheckDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetAllProductsWithDetailsAsync()
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Producer)
                .Include(p => p.User)
                .Include(p => p.ContentProducts)
                    .ThenInclude(cp => cp.Content)
                        .ThenInclude(c => c.RiskStatus)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> SearchProductsAsync(string query)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Producer)
                .Include(p => p.User)
                .Include(p => p.ContentProducts)
                    .ThenInclude(cp => cp.Content)
                        .ThenInclude(c => c.RiskStatus)
                .Where(p => p.ProductName.Contains(query) ||
                           p.Category.CategoryName.Contains(query) ||
                           p.Producer.ProducerName.Contains(query))
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Producer)
                .Include(p => p.User)
                .Include(p => p.ContentProducts)
                    .ThenInclude(cp => cp.Content)
                        .ThenInclude(c => c.RiskStatus)
                .Where(p => p.CategoryId == categoryId)
                .ToListAsync();
        }

        public async Task<bool> CreateProductWithContentsAsync(Product product, List<int> selectedContents)
        {
            try
            {
                await _context.Products.AddAsync(product);
                await _context.SaveChangesAsync();

                if (selectedContents != null && selectedContents.Any())
                {
                    foreach (var contentId in selectedContents)
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
    }
}
