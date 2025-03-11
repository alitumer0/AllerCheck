using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllerCheck_Core.Entities
{
    public class User
    {
        public int UserId { get; set; }

        public string UserName { get; set; } = null!;

        public string UserSurname { get; set; } = null!;

        public string MailAdress { get; set; } = null!;
        
        public DateTime UserCreateDate { get; set; }

        public string UserPassword { get; set; } = null!;

        public int CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public int? ModifiedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }       

        public virtual Admin CreatedByNavigation { get; set; } = null!;

        public virtual ICollection<FavoriteList> FavoriteLists { get; set; } = new List<FavoriteList>();

        public virtual ICollection<Contact> Contacts { get; set; } = new List<Contact>();

        public virtual ICollection<BlackList> BlackLists { get; set; } = new List<BlackList>();

        public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    }
}
