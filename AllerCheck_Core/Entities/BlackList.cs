using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllerCheck_Core.Entities
{
    public class BlackList 
    {
        public int BlackListId { get; set; }

        public int UserId { get; set; }

        public int ContentId { get; set; }

        public virtual Content Content { get; set; } = null!;

        public virtual User User { get; set; } = null!;
    }
}
