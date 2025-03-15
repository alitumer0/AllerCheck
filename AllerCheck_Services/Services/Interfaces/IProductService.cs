using AllerCheck_Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AllerCheck_Services.Services.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAllProductsWithDetailsAsync();
        Task<IEnumerable<Product>> SearchProductsAsync(string query);
        Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId);
        Task<IEnumerable<Producer>> GetAllProducersAsync();
        Task<IEnumerable<Content>> GetAllContentsWithRiskStatusAsync();
        Task<bool> CreateProductWithContentsAsync(Product product, List<int> existingContentIds, List<(string name, int riskStatusId)> newContents);
        Task<Product> GetProductWithContentsAsync(int productId);
        Task<bool> UpdateProductWithContentsAsync(Product product, List<int> selectedContents);
        Task<bool> CreateContentAsync(Content content);
        Task<bool> UpdateContentAsync(Content content);
        Task<bool> DeleteContentAsync(int id);
    }
}
