using AllerCheck_Core.Entities;
using AllerCheck_Data.Repositories.Interfaces;
using AllerCheck_Services.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using AllerCheck_Data.Context;
using System.Threading.Tasks;

namespace AllerCheck_Services.Services
{
    public class ContentService : IContentService
    {
        private readonly IContentRepository _contentRepository;
        private readonly AllerCheckDbContext _context;

        public ContentService(IContentRepository contentRepository, AllerCheckDbContext context)
        {
            _contentRepository = contentRepository;
            _context = context;
        }

        public async Task<bool> AddContentAsync(Content content)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    await _context.Contents.AddAsync(content);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return true;
                }
                catch
                {
                    await transaction.RollbackAsync();
                    return false;
                }
            }
        }

        public async Task<bool> UpdateContentAsync(Content content)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var existingContent = await _context.Contents
                        .FirstOrDefaultAsync(c => c.ContentId == content.ContentId);

                    if (existingContent == null)
                        return false;

                    _context.Entry(existingContent).CurrentValues.SetValues(content);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return true;
                }
                catch
                {
                    await transaction.RollbackAsync();
                    return false;
                }
            }
        }

        public async Task<bool> DeleteContentAsync(int id)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var content = await _context.Contents
                        .Include(c => c.ContentProducts)
                        .Include(c => c.BlackLists)
                        .FirstOrDefaultAsync(c => c.ContentId == id);

                    if (content == null)
                        return false;

                    if (content.ContentProducts.Any())
                        return false;

                    _context.BlackLists.RemoveRange(content.BlackLists);
                    _context.Contents.Remove(content);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return true;
                }
                catch
                {
                    await transaction.RollbackAsync();
                    return false;
                }
            }
        }

        public async Task<Content> GetContentByIdAsync(int id)
        {
            return await _context.Contents
                .Include(c => c.RiskStatus)
                .Include(c => c.ContentProducts)
                .Include(c => c.BlackLists)
                .FirstOrDefaultAsync(c => c.ContentId == id);
        }

        public async Task<List<Content>> GetAllContentsAsync()
        {
            return await _context.Contents
                .Include(c => c.RiskStatus)
                .ToListAsync();
        }
    }
} 