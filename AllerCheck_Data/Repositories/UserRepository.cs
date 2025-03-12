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
    public class UserRepository : IUserRepository
    {
        private readonly AllerCheckDbContext _db;

        public UserRepository(AllerCheckDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<User>> GetAllActiveUsersAsync()
        {
            return await _db.Users
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<User> GetUserByIdWithDetailsAsync(int id)
        {
            return await _db.Users
                .Include(u => u.FavoriteLists)
                .Include(u => u.BlackLists)
                .FirstOrDefaultAsync(u => u.UserId == id);
        }

        public async Task<bool> CreateUserWithDetailsAsync(User user, List<FavoriteList> favoriteLists = null, List<BlackList> blackLists = null)
        {
            bool userReturn = false;
            using (var transaction = await _db.Database.BeginTransactionAsync())
            {
                try
                {
                    
                    if (string.IsNullOrEmpty(user.UserName) || string.IsNullOrEmpty(user.UserPassword))
                    {
                        throw new Exception("Kullanıcı adı ve şifre boş olamaz!");
                    }

                    
                    if (_db.Users.Any(u => u.MailAdress == user.MailAdress))
                    {
                        throw new Exception("Bu email adresi zaten kayıtlı!");
                    }

                    
                    user.CreatedDate = DateTime.Now;
                    await _db.Users.AddAsync(user);
                    if (await _db.SaveChangesAsync() > 0)
                    {
                        if (favoriteLists?.Any() == true)
                        {
                            foreach (var list in favoriteLists)
                            {
                                list.UserId = user.UserId;
                                await _db.FavoriteLists.AddAsync(list);
                            }
                        }

                        if (blackLists?.Any() == true)
                        {
                            foreach (var list in blackLists)
                            {
                                list.UserId = user.UserId;
                                await _db.BlackLists.AddAsync(list);
                            }
                        }

                        await _db.SaveChangesAsync();
                        await transaction.CommitAsync();
                        userReturn = true;
                    }
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                }
            }
            return userReturn;
        }

        public async Task<bool> UpdateUserWithDetailsAsync(User user, List<FavoriteList> favoriteLists = null, List<BlackList> blackLists = null)
        {
            bool userReturn = false;
            using (var transaction = await _db.Database.BeginTransactionAsync())
            {
                try
                {
                    var existingUser = await _db.Users.FindAsync(user.UserId);
                    if (existingUser != null)
                    {
                        _db.Entry(existingUser).CurrentValues.SetValues(user);

                        if (favoriteLists != null)
                        {
                            var existingFavorites = await _db.FavoriteLists.Where(f => f.UserId == user.UserId).ToListAsync();
                            _db.FavoriteLists.RemoveRange(existingFavorites);
                            await _db.FavoriteLists.AddRangeAsync(favoriteLists);
                        }

                        if (blackLists != null)
                        {
                            var existingBlacklists = await _db.BlackLists.Where(b => b.UserId == user.UserId).ToListAsync();
                            _db.BlackLists.RemoveRange(existingBlacklists);
                            await _db.BlackLists.AddRangeAsync(blackLists);
                        }

                        if (await _db.SaveChangesAsync() > 0)
                        {
                            await transaction.CommitAsync();
                            userReturn = true;
                        }
                    }
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                }
            }
            return userReturn;
        }

        public async Task<bool> DeleteUserAndRelatedDataAsync(int id)
        {
            bool userReturn = false;
            using (var transaction = await _db.Database.BeginTransactionAsync())
            {
                try
                {
                    var user = await GetUserByIdWithDetailsAsync(id);
                    if (user != null)
                    {
                        _db.Users.Remove(user);
                        if (await _db.SaveChangesAsync() > 0)
                        {
                            await transaction.CommitAsync();
                            userReturn = true;
                        }
                    }
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                }
            }
            return userReturn;
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _db.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.MailAdress == email);
        }

        public async Task<bool> CheckUserExistsAsync(string email)
        {
            return await _db.Users.AnyAsync(u => u.MailAdress == email);
        }
    }
}
//Todo : transaction script