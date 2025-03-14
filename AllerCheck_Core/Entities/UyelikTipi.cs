using System.Collections.Generic;

namespace AllerCheck_Core.Entities
{
    public class UyelikTipi
    {
        public UyelikTipi()
        {
            Users = new HashSet<User>();
        }

        public int UyelikTipiId { get; set; }
        public string TipAdi { get; set; }

        public virtual ICollection<User> Users { get; set; }
    }
} 