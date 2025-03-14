using System.Collections.Generic;
using AllerCheck.API.DTOs.ProductDTO;

namespace AllerCheck.API.DTOs.CategoryDTO
{
    public class CategoryDto
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int TopCategoryId { get; set; }
        public string TopCategoryName { get; set; }
        public List<ProductDto> Products { get; set; }
    }
}
