using AllerCheck_Core.Entities;

namespace AllerCheck_Data.Repositories.Interfaces
{
    public interface IContentRepository
    {
        void Add(Content content);
        void Update(Content content);
        void Delete(Content content);
        Content GetById(int id);
        IQueryable<Content> GetAll();
        void SaveChanges();
    }
} 