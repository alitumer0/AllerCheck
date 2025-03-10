using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllerCheck_Core.Entities
{
    public class FavoriteListDetail
    {
        public int FavoriteListDetailId { get; set; }

        public int ProductId { get; set; }

        public int FavoriteListId { get; set; }

        public virtual FavoriteList FavoriteList { get; set; } = null!;

        public virtual Product Product { get; set; } = null!;
    }
}
