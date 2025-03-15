using System.Collections.Generic;
using AllerCheck.API.DTOs.ContentDTO;

namespace AllerCheck.API.DTOs.ProductDTO
{
    public class ProductDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public int CategoryId { get; set; }
        public int ProducerId { get; set; }
        public string? CategoryName { get; set; }
        public string? ProducerName { get; set; }
        public string? AddedByUserName { get; set; }
        public int FavoriteListDetailId { get; set; }
        public List<ContentDto>? Contents { get; set; } = new List<ContentDto>();
        public int UrunTakipId { get; set; }
    }
}
