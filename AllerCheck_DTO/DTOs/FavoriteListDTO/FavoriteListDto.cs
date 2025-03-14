using System.Collections.Generic;
using AllerCheck.API.DTOs.ProductDTO;

namespace AllerCheck.API.DTOs.FavoriteListDTO
{
    public class FavoriteListDto
    {
        public int FavoriteListId { get; set; }
        public string ListName { get; set; }
        public int UserId { get; set; }
        public List<ProductDto> Products { get; set; }
    }
} 