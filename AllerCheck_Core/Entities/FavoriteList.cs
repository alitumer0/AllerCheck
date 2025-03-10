using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllerCheck_Core.Entities
{
    public class FavoriteList
    {
        public int FavoriteListId { get; set; }

        public string ListName { get; set; } = null!;

        public int UserId { get; set; }

        public virtual ICollection<FavoriteListDetail> FavoriteListDetails { get; set; } = new List<FavoriteListDetail>();

        public virtual User User { get; set; } = null!;
    }
}
