using System.Collections.Generic;
using System.Threading.Tasks;
using AllerCheck_Core.Entities;

namespace AllerCheck_Data.Repositories.Interfaces
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllProductsWithDetailsAsync();
        Task<IEnumerable<Product>> SearchProductsAsync(string query);
        Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId);
        Task<bool> CreateProductWithContentsAsync(Product product, List<int> selectedContents);
    }
}
