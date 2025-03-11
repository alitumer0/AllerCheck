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
    public class ProductRepository : IProductRepository
    {
        private readonly AllerCheckDbContext _db;

        public ProductRepository(AllerCheckDbContext db)
        {
            _db = db;
        }

        public IEnumerable<Product> GetAll()
        {
            return _db.Products.ToList();
        }

        public Product GetById(int id)
        {
            return _db.Products.Find(id);
        }

        public void Add(Product product)
        {
            _db.Products.Add(product);
            _db.SaveChanges();
        }

        public void Update(Product product)
        {
            _db.Products.Update(product);
            _db.SaveChanges();
        }

        public void Delete(int id)
        {
            var product = GetById(id);
            if (product != null)
            {
            _db.Products.Remove(product);
            _db.SaveChanges();
            }
        }
    }
}
