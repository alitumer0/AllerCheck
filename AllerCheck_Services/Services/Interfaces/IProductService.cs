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
        Task<bool> CreateProductWithContentsAsync(Product product, List<int> selectedContents);
    }
}
