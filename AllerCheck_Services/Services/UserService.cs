using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<User> GetUserByIdWithDetailsAsync(int userId)
        {
            return await _userRepository.GetUserByIdWithDetailsAsync(userId);
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            return await _userRepository.GetUserByIdWithDetailsAsync(id);
        }

        public async Task<IEnumerable<FavoriteList>> GetUserFavoriteListsAsync(int userId)
        {
            var user = await _userRepository.GetUserByIdWithDetailsAsync(userId);
            return user.FavoriteLists;
        }

        public async Task<IEnumerable<BlackList>> GetUserBlackListAsync(int userId)
        {
            var user = await _userRepository.GetUserByIdWithDetailsAsync(userId);
            return user.BlackLists;
        }

        public async Task<bool> AddToFavoritesAsync(int userId, int productId, string listName)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var user = await _context.Users
                        .Include(u => u.FavoriteLists)
                            .ThenInclude(fl => fl.FavoriteListDetails)
                        .FirstOrDefaultAsync(u => u.UserId == userId);

                    if (user == null)
                        return false;

                    var favoriteList = user.FavoriteLists.FirstOrDefault(fl => fl.ListName == listName);
                    if (favoriteList == null)
                    {
                        favoriteList = new FavoriteList { UserId = userId, ListName = listName };
                        await _context.FavoriteLists.AddAsync(favoriteList);
                        await _context.SaveChangesAsync();
                    }

                    var existingDetail = favoriteList.FavoriteListDetails
                        .FirstOrDefault(fld => fld.ProductId == productId);

                    if (existingDetail == null)
                    {
                        var detail = new FavoriteListDetail
                        {
                            FavoriteListId = favoriteList.FavoriteListId,
                            ProductId = productId
                        };
                        await _context.FavoriteListDetails.AddAsync(detail);
                        await _context.SaveChangesAsync();
                    }

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

        public async Task<bool> AddToBlackListAsync(int userId, int contentId)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var existingBlackList = await _context.BlackLists
                        .FirstOrDefaultAsync(bl => bl.UserId == userId && bl.ContentId == contentId);

                    if (existingBlackList != null)
                        return false;

                    var blackList = new BlackList
                    {
                        UserId = userId,
                        ContentId = contentId
                    };

                    await _context.BlackLists.AddAsync(blackList);
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

        public async Task<bool> RemoveFromFavoritesAsync(int userId, int favoriteListDetailId)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var favoriteListDetail = await _context.FavoriteListDetails
                        .Include(fld => fld.FavoriteList)
                        .FirstOrDefaultAsync(fld => fld.FavoriteListDetailId == favoriteListDetailId &&
                                                  fld.FavoriteList.UserId == userId);

                    if (favoriteListDetail == null)
                        return false;

                    _context.FavoriteListDetails.Remove(favoriteListDetail);
                    await _context.SaveChangesAsync();

                    // Eğer liste boşsa, listeyi de sil
                    var remainingDetails = await _context.FavoriteListDetails
                        .AnyAsync(fld => fld.FavoriteListId == favoriteListDetail.FavoriteListId);

                    if (!remainingDetails)
                    {
                        var emptyList = await _context.FavoriteLists
                            .FirstOrDefaultAsync(fl => fl.FavoriteListId == favoriteListDetail.FavoriteListId);
                        if (emptyList != null)
                        {
                            _context.FavoriteLists.Remove(emptyList);
                            await _context.SaveChangesAsync();
                        }
                    }

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
                        return false;

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
                if (user == null)
                    return false;

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
