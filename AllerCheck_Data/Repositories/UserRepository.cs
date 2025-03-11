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
    public class UserRepository : IUserRepository
    {
        private readonly AllerCheckDbContext _db;

        public UserRepository(AllerCheckDbContext db)
        {
            _db = db;
        }

        public IEnumerable<User> GetAll()
        {
            return _db.Users.ToList();
        }

        public User GetById(int id)
        {
            return _db.Users.Find(id);
        }

        public void Add(User user)
        {
            _db.Users.Add(user);
            _db.SaveChanges();
        }

        public void Update(User user)
        {
            _db.Users.Update(user);
            _db.SaveChanges();
        }

        public void Delete(int id)
        {
            var user = GetById(id);
            if (user != null)
            {
            _db.Users.Remove(user);
            _db.SaveChanges();
            }
        }
    }
}
