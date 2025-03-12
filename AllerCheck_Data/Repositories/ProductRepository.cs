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
    public class ProductRepository : IProductRepository
    {
        private readonly AllerCheckDbContext _db;

        public ProductRepository(AllerCheckDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _db.Products.ToListAsync();
        }

        public async Task<Product> GetByIdAsync(int id)
        {
            return await _db.Products.FindAsync(id);
        }

        public async Task<bool> AddAsync(Product product)
        {
            bool result = false;
            using (var transaction = await _db.Database.BeginTransactionAsync())
            {
                try
                {
                    await _db.Products.AddAsync(product);
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

        public async Task<bool> UpdateAsync(Product product)
        {
            bool result = false;
            using (var transaction = await _db.Database.BeginTransactionAsync())
            {
                try
                {
                    _db.Products.Update(product);
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
                    var product = await GetByIdAsync(id);
                    if (product != null)
                    {
                        _db.Products.Remove(product);
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
