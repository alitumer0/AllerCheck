using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllerCheck_Core.Entities
{
    public class User
    {
        public User()
        {
            BlackLists = new HashSet<BlackList>();
            FavoriteLists = new HashSet<FavoriteList>();
        }

        public int UserId { get; set; }
        public string UserName { get; set; }
        public string UserSurname { get; set; }
        public string MailAdress { get; set; }
        public string UserPassword { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }

        public virtual ICollection<BlackList> BlackLists { get; set; }
        public virtual ICollection<FavoriteList> FavoriteLists { get; set; }

        public virtual ICollection<Contact> Contacts { get; set; } = new List<Contact>();

        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
