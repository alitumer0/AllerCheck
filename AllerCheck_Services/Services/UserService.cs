using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AllerCheck.API.DTOs.UserDTO;
using AllerCheck_Core.Entities;
using AllerCheck_Data.Repositories.Interfaces;
using AllerCheck_Services.Services.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using AllerCheck_Data.Context;

namespace AllerCheck_Services.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly AllerCheckDbContext _context;

        public UserService(IUserRepository userRepository, IMapper mapper, AllerCheckDbContext context)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _context = context;
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllActiveUsersAsync();
            return _mapper.Map<IEnumerable<UserDto>>(users);
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            return await _userRepository.GetUserByIdWithDetailsAsync(id);
        }

        public async Task<UserDto> GetUserByEmailAsync(string email)
        {
            var user = await _userRepository.GetUserByEmailAsync(email);
            return _mapper.Map<UserDto>(user);
        }

        public async Task<bool> CheckUserExistsAsync(string email)
        {
            return await _userRepository.CheckUserExistsAsync(email);
        }

        public async Task<bool> CreateUserAsync(UserDto userDto)
        {
            var user = _mapper.Map<User>(userDto);
            user.CreatedDate = DateTime.Now;
            return await _userRepository.CreateUserWithDetailsAsync(user);
        }

        public async Task<bool> UpdateUserAsync(UserDto userDto)
        {
            var user = _mapper.Map<User>(userDto);
            return await _userRepository.UpdateUserWithDetailsAsync(user);
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            return await _userRepository.DeleteUserAndRelatedDataAsync(id);
        }

        public async Task<User> GetUserByIdWithDetailsAsync(int userId)
        {
            return await _userRepository.GetUserByIdWithDetailsAsync(userId);
        }

        public async Task<IEnumerable<FavoriteList>> GetUserFavoriteListsAsync(int userId)
        {
            var user = await _userRepository.GetUserByIdWithDetailsAsync(userId);
            return user?.FavoriteLists ?? new List<FavoriteList>();
        }

        public async Task<IEnumerable<BlackList>> GetUserBlackListAsync(int userId)
        {
            var user = await _userRepository.GetUserByIdWithDetailsAsync(userId);
            return user?.BlackLists ?? new List<BlackList>();
        }

        public async Task<bool> AddToFavoritesAsync(int userId, int productId, string listName)
        {
            try
            {
                var user = await _userRepository.GetUserByIdWithDetailsAsync(userId);
                if (user == null) return false;

                var favoriteList = user.FavoriteLists.FirstOrDefault(fl => fl.ListName == listName);
                if (favoriteList == null)
                {
                    favoriteList = new FavoriteList
                    {
                        UserId = userId,
                        ListName = listName,
                        FavoriteListDetails = new List<FavoriteListDetail>()
                    };
                    user.FavoriteLists.Add(favoriteList);
                }

                if (favoriteList.FavoriteListDetails.Any(fld => fld.ProductId == productId))
                {
                    return false;
                }

                var favoriteListDetail = new FavoriteListDetail
                {
                    ProductId = productId,
                    FavoriteList = favoriteList
                };
                favoriteList.FavoriteListDetails.Add(favoriteListDetail);

                return await _userRepository.UpdateUserWithDetailsAsync(user);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> AddToBlackListAsync(int userId, int contentId)
        {
            try
            {
                var user = await _userRepository.GetUserByIdWithDetailsAsync(userId);
                if (user == null) return false;

                if (user.BlackLists.Any(bl => bl.ContentId == contentId))
                {
                    return false;
                }

                var blackList = new BlackList
                {
                    UserId = userId,
                    ContentId = contentId
                };
                user.BlackLists.Add(blackList);

                return await _userRepository.UpdateUserWithDetailsAsync(user);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> RemoveFromFavoritesAsync(int userId, int favoriteListDetailId)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var favoriteListDetail = await _context.FavoriteListDetails
                        .Include(f => f.FavoriteList)
                        .FirstOrDefaultAsync(f => f.FavoriteListDetailId == favoriteListDetailId && f.FavoriteList.UserId == userId);

                    if (favoriteListDetail == null)
                    {
                        return false;
                    }

                    _context.FavoriteListDetails.Remove(favoriteListDetail);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return true;
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    return false;
                }
            }
        }

        public async Task<bool> RemoveFromBlackListAsync(int userId, int blackListId)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var blackList = await _context.BlackLists
                        .FirstOrDefaultAsync(bl => bl.BlackListId == blackListId && bl.UserId == userId);

                    if (blackList == null)
                    {
                        return false;
                    }

                    _context.BlackLists.Remove(blackList);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return true;
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    return false;
                }
            }
        }

        public async Task<bool> UpdateUserProfileAsync(int userId, UserDto userDto)
        {
            try
            {
                var user = await _userRepository.GetUserByIdWithDetailsAsync(userId);
                if (user == null) return false;

                user.UserName = userDto.UserName;
                user.UserSurname = userDto.UserSurname;

                return await _userRepository.UpdateUserWithDetailsAsync(user);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
