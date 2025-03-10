using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllerCheck_Core.Entities
{
    public class Admin
    {
        public int AdminId { get; set; }

        public string AdminUsername { get; set; } = null!;

        public string AdminMail { get; set; } = null!;

        public string AdminPassword { get; set; } = null!;

        public virtual ICollection<User> Users { get; set; } = new List<User>();

    }
}
