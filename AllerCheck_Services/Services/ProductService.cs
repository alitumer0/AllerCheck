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

        public async Task<bool> CreateProductWithContentsAsync(Product product, List<int> selectedContents)
        {
            return await _productRepository.CreateProductWithContentsAsync(product, selectedContents);
        }
    }
}
