using AllerCheck_Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AllerCheck_Services.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<Category>> GetAllCategoriesAsync();
        Task<bool> CreateCategoryAsync(Category category);
    }
}
