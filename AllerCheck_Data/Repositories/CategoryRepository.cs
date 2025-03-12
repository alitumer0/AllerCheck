using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AllerCheck_Core.Entities;
using AllerCheck_Data.Repositories.Interfaces;
using AllerCheck_Data.Context;
using Microsoft.EntityFrameworkCore;

namespace AllerCheck_Data.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AllerCheckDbContext _db;

        public CategoryRepository(AllerCheckDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            return await _db.Categories.ToListAsync();
        }

        public async Task<Category> GetByIdAsync(int id)
        {
            return await _db.Categories.FindAsync(id);
        }

        public async Task<bool> AddAsync(Category category)
        {
            bool result = false;
            using (var transaction = await _db.Database.BeginTransactionAsync())
            {
                try
                {
                    await _db.Categories.AddAsync(category);
                    if (await _db.SaveChangesAsync() > 0)
                    {
                        await transaction.CommitAsync();
                        result = true;
                    }
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                }
            }
            return result;
        }

        public async Task<bool> UpdateAsync(Category category)
        {
            bool result = false;
            using (var transaction = await _db.Database.BeginTransactionAsync())
            {
                try
                {
                    _db.Categories.Update(category);
                    if (await _db.SaveChangesAsync() > 0)
                    {
                        await transaction.CommitAsync();
                        result = true;
                    }
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                }
            }
            return result;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            bool result = false;
            using (var transaction = await _db.Database.BeginTransactionAsync())
            {
                try
                {
                    var category = await GetByIdAsync(id);
                    if (category != null)
                    {
                        _db.Categories.Remove(category);
                        if (await _db.SaveChangesAsync() > 0)
                        {
                            await transaction.CommitAsync();
                            result = true;
                        }
                    }
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                }
            }
            return result;
        }
    }
}
