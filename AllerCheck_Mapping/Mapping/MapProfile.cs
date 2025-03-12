using System;
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

namespace AllerCheck_Mapping.Mapping
{
    public class MapProfile : Profile
    {
        public MapProfile()
        {
            // User mappingi
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.FavoriteLists, opt => opt.MapFrom(src => src.FavoriteLists))
                .ForMember(dest => dest.BlackLists, opt => opt.MapFrom(src => src.BlackLists));
            CreateMap<UserDto, User>();

            // Category mappingi
            CreateMap<Category, CategoryDto>()
                .ForMember(dest => dest.TopCategoryName, opt => opt.MapFrom(src => src.TopCategory.CategoryName))
                .ForMember(dest => dest.Products, opt => opt.MapFrom(src => src.Products));
            CreateMap<CategoryDto, Category>();

            // Product mappingi
            CreateMap<Product, ProductDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.CategoryName))
                .ForMember(dest => dest.ProducerName, opt => opt.MapFrom(src => src.Producer.ProducerName))
                .ForMember(dest => dest.Contents, opt => opt.MapFrom(src => src.ContentProducts.Select(cp => cp.Content)));
            CreateMap<ProductDto, Product>();

            // FavoriteList mappingi
            CreateMap<FavoriteList, FavoriteListDto>()
                .ForMember(dest => dest.Products, opt => opt.MapFrom(src => 
                    src.FavoriteListDetails.Select(fd => fd.Product)));
            CreateMap<FavoriteListDto, FavoriteList>();

            // BlackList mappingi
            CreateMap<BlackList, BlackListDto>()
                .ForMember(dest => dest.ContentName, opt => opt.MapFrom(src => src.Content.ContentName));
            CreateMap<BlackListDto, BlackList>();

            // Todo: Content mappingi
            CreateMap<Content, ContentDto>()
                .ForMember(dest => dest.RiskStatusName, opt => opt.MapFrom(src => src.RiskStatus.RiskStatusName));
            CreateMap<ContentDto, Content>();
        }
    }
}
