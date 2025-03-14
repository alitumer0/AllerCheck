using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AllerCheck_Core.Entities;
using AllerCheck_Data.Repositories.Interfaces;
using AllerCheck.API.DTOs.CategoryDTO;
using AllerCheck_Services.Services.Interfaces;
using AutoMapper;

namespace AllerCheck_Services.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public CategoryService(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            return await _categoryRepository.GetAllAsync();
        }

        public async Task<bool> CreateCategoryAsync(Category category)
        {
            try
            {
                return await _categoryRepository.AddAsync(category);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
