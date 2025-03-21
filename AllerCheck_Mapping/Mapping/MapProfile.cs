﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using AllerCheck_Core.Entities;
using AllerCheck.API.DTOs.UserDTO;
using AllerCheck.API.DTOs.ProductDTO;
using AllerCheck.API.DTOs.CategoryDTO;
using AllerCheck.API.DTOs.ContentDTO;
using AllerCheck.API.DTOs.FavoriteListDTO;
using AllerCheck.API.DTOs.BlackListDTO;
using AllerCheck.API.DTOs.RegisterDTO;
using AllerCheck.API.DTOs.LoginDTO;
using Microsoft.AspNetCore.Routing.Constraints;

namespace AllerCheck_Mapping.Mapping
{
    public class MapProfile : Profile
    {
        public MapProfile()
        {
            // Register DTO -> User
            CreateMap<RegisterDto, User>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.MailAdress, opt => opt.MapFrom(src => src.MailAdress))
                .ForMember(dest => dest.UserPassword, opt => opt.MapFrom(src => src.UserPassword))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.UserSurname, opt => opt.MapFrom(src => src.UserSurname))
                .ForMember(dest => dest.UyelikTipiId, opt => opt.MapFrom(src => 1))
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => 1))
                .ForMember(dest => dest.Contacts, opt => opt.Ignore())
                .ForMember(dest => dest.Products, opt => opt.Ignore())
                .ForMember(dest => dest.BlackLists, opt => opt.Ignore())
                .ForMember(dest => dest.FavoriteLists, opt => opt.Ignore());

            // User -> UserDto
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.UserSurname, opt => opt.MapFrom(src => src.UserSurname))
                .ForMember(dest => dest.MailAdress, opt => opt.MapFrom(src => src.MailAdress))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CreatedDate));

            // UserDto -> User
            CreateMap<UserDto, User>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.UserSurname, opt => opt.MapFrom(src => src.UserSurname))
                .ForMember(dest => dest.MailAdress, opt => opt.Ignore())
                .ForMember(dest => dest.UserPassword, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.UyelikTipiId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.Contacts, opt => opt.Ignore())
                .ForMember(dest => dest.Products, opt => opt.Ignore())
                .ForMember(dest => dest.BlackLists, opt => opt.Ignore())
                .ForMember(dest => dest.FavoriteLists, opt => opt.Ignore());

            // Category mappingi
            CreateMap<Category, CategoryDto>()
                .ForMember(dest => dest.TopCategoryName, opt => opt.MapFrom(src => src.TopCategory.CategoryName))
                .ForMember(dest => dest.Products, opt => opt.MapFrom(src => src.Products));
            CreateMap<CategoryDto, Category>();

            // Product mappingi
            CreateMap<Product, ProductDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.CategoryName : string.Empty))
                .ForMember(dest => dest.ProducerName, opt => opt.MapFrom(src => src.Producer != null ? src.Producer.ProducerName : string.Empty))
                .ForMember(dest => dest.AddedByUserName, opt => opt.MapFrom(src => src.User != null ? src.User.UserName : string.Empty))
                .ForMember(dest => dest.Contents, opt => opt.MapFrom(src => 
                    src.ContentProducts != null ? src.ContentProducts.Select(cp => new ContentDto 
                    {
                        ContentId = cp.Content.ContentId,
                        ContentName = cp.Content.ContentName,
                        RiskStatusName = cp.Content.RiskStatus != null ? cp.Content.RiskStatus.RiskStatusName : "Belirsiz"
                    }) : new List<ContentDto>()))
                .ForMember(dest => dest.FavoriteListDetailId, opt => opt.MapFrom(src => 
                    src.FavoriteListDetails != null && src.FavoriteListDetails.Any() 
                        ? src.FavoriteListDetails.First().FavoriteListDetailId 
                        : 0));
                
            CreateMap<ProductDto, Product>()
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.ProductName))
                .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.CategoryId))
                .ForMember(dest => dest.ProducerId, opt => opt.MapFrom(src => src.ProducerId))
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedBy, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore())
                .ForMember(dest => dest.Category, opt => opt.Ignore())
                .ForMember(dest => dest.Producer, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.ContentProducts, opt => opt.Ignore())
                .ForMember(dest => dest.FavoriteListDetails, opt => opt.Ignore());

            // FavoriteList mappingi
            CreateMap<FavoriteList, FavoriteListDto>()
                .ForMember(dest => dest.Products, opt => opt.MapFrom(src => 
                    src.FavoriteListDetails.Select(fd => new ProductDto 
                    { 
                        Id = fd.Product.ProductId,
                        ProductId = fd.Product.ProductId,
                        ProductName = fd.Product.ProductName,
                        CategoryName = fd.Product.Category.CategoryName,
                        ProducerName = fd.Product.Producer.ProducerName,
                        FavoriteListDetailId = fd.FavoriteListDetailId,
                        Contents = fd.Product.ContentProducts.Select(cp => new ContentDto 
                        { 
                            ContentId = cp.Content.ContentId,
                            ContentName = cp.Content.ContentName,
                            RiskStatusName = cp.Content.RiskStatus.RiskStatusName
                        }).ToList()
                    })));
            CreateMap<FavoriteListDto, FavoriteList>();

            // BlackList mappingi
            CreateMap<BlackList, BlackListDto>()
                .ForMember(dest => dest.ContentName, opt => opt.MapFrom(src => src.Content.ContentName))
                .ForMember(dest => dest.RiskStatusName, opt => opt.MapFrom(src => src.Content.RiskStatus.RiskStatusName));
            CreateMap<BlackListDto, BlackList>();

            // Content mappingi
            CreateMap<Content, ContentDto>()
                .ForMember(dest => dest.RiskStatusName, opt => opt.MapFrom(src => src.RiskStatus.RiskStatusName))
                .ForMember(dest => dest.ContentInfo, opt => opt.MapFrom(src => src.ContentInfo));

            CreateMap<ContentDto, Content>()
                .ForMember(dest => dest.ContentName, opt => opt.MapFrom(src => src.ContentName))
                .ForMember(dest => dest.RiskStatusId, opt => opt.MapFrom(src => src.RiskStatusId))
                .ForMember(dest => dest.ContentInfo, opt => opt.MapFrom(src => src.ContentInfo))
                .ForMember(dest => dest.BlackLists, opt => opt.Ignore())
                .ForMember(dest => dest.ContentProducts, opt => opt.Ignore());
        }
    }
}
