using System.Collections.Generic;
using System.Threading.Tasks;
using AllerCheck_Core.Entities;
using AllerCheck.API.DTOs.UserDTO;

namespace AllerCheck_Services.Services.Interfaces
{
    public interface IUserService
    {
        Task<User> GetUserByIdWithDetailsAsync(int userId);
        Task<User> GetUserByIdAsync(int userId);
        Task<IEnumerable<FavoriteList>> GetUserFavoriteListsAsync(int userId);
        Task<IEnumerable<BlackList>> GetUserBlackListAsync(int userId);
        Task<bool> AddToFavoritesAsync(int userId, int productId, string listName);
        Task<bool> AddToBlackListAsync(int userId, int contentId);
        Task<bool> RemoveFromFavoritesAsync(int userId, int favoriteListDetailId);
        Task<bool> RemoveFromBlackListAsync(int userId, int blackListId);
        Task<bool> UpdateUserProfileAsync(int userId, UserDto userDto);
    }
}
