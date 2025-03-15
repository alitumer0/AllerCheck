using AllerCheck_Core.Entities;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace AllerCheck_Services.Services.Interfaces
{
    public interface IContentService
    {
        Task<bool> AddContentAsync(Content content);
        Task<bool> UpdateContentAsync(Content content);
        Task<bool> DeleteContentAsync(int id);
        Task<Content> GetContentByIdAsync(int id);
        Task<List<Content>> GetAllContentsAsync();
    }
} 