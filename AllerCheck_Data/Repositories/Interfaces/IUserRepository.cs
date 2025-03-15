using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AllerCheck_Core.Entities;

namespace AllerCheck_Data.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllActiveUsersAsync();
        Task<User> GetUserByIdWithDetailsAsync(int id);
        Task<bool> CreateUserWithDetailsAsync(User user, List<FavoriteList> favoriteLists = null, List<BlackList> blackLists = null);
        Task<bool> UpdateUserWithDetailsAsync(User user, List<FavoriteList> favoriteLists = null, List<BlackList> blackLists = null);
        Task<bool> DeleteUserAndRelatedDataAsync(int id);
        Task<User> GetUserByEmailAsync(string email);
        Task<bool> CheckUserExistsAsync(string email);
    }
}
