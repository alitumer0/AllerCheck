using AllerCheck_Core.Entities;

namespace AllerCheck_Services.Services.Interfaces
{
    public interface IContentService
    {
        void AddContent(Content content);
        void UpdateContent(Content content);
        void DeleteContent(int id);
        Content GetContentById(int id);
        List<Content> GetAllContents();
    }
} 