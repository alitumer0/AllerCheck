using AllerCheck_Core.Entities;
using AllerCheck_Data.Context;
using AllerCheck_Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AllerCheck_Data.Repositories
{
    public class ContentRepository : IContentRepository
    {
        private readonly AllerCheckDbContext _context;

        public ContentRepository(AllerCheckDbContext context)
        {
            _context = context;
        }

        public void Add(Content content)
        {
            _context.Contents.Add(content);
        }

        public void Delete(Content content)
        {
            _context.Contents.Remove(content);
        }

        public IQueryable<Content> GetAll()
        {
            return _context.Contents
                .Include(c => c.RiskStatus);
        }

        public Content GetById(int id)
        {
            return _context.Contents
                .Include(c => c.RiskStatus)
                .Include(c => c.ContentProducts)
                .Include(c => c.BlackLists)
                .FirstOrDefault(c => c.ContentId == id);
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        public void Update(Content content)
        {
            _context.Contents.Update(content);
        }
    }
} 