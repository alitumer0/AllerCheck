using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllerCheck_Core.Entities
{
    public class Product
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; } = null!;

        public int CategoryId { get; set; }

        public int ProducerId { get; set; }

        public int UserId { get; set; }

        public int CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public int? ModifiedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public virtual ICollection<FavoriteListDetail> FavoriteListDetails { get; set; } = new List<FavoriteListDetail>();

        public virtual Category Category { get; set; } = null!;

        public virtual Producer Producer { get; set; } = null!;

        public virtual ICollection<ContentProduct> ContentProducts { get; set; } = new List<ContentProduct>();

        public virtual User User { get; set; } = null!;
    }

}
