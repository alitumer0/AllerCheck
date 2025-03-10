using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllerCheck_Core.Entities
{
    public class ContentProduct
    {
        public int ContentProductId { get; set; }

        public int ProductId { get; set; }

        public int ContentId { get; set; }

        public virtual Content Content { get; set; } = null!;

        public virtual Product Product { get; set; } = null!;
    }
}
