using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AllerCheck_Core.Entities;
using AllerCheck_Core.Repositories.Interfaces;
using AllerCheck_Data.Context;

namespace AllerCheck_Core.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AllerCheckDbContext _db;

        public CategoryRepository(AllerCheckDbContext db)
        {
            _db = db;
        }

        public IEnumerable<Category> GetAll()
        {
            return _db.Categories.ToList();
        }

        public Category GetById(int id)
        {
            return _db.Categories.Find(id);
        }

        public void Add(Category category)
        {
            _db.Categories.Add(category);
            _db.SaveChanges();
        }

        public void Update(Category category)
        {
            _db.Categories.Update(category);
            _db.SaveChanges();
        }

        public void Delete(int id)
        {
            var category = GetById(id);
            if (category != null)
            {
            _db.Categories.Remove(category);
            _db.SaveChanges();
            }
        }
    }
}
