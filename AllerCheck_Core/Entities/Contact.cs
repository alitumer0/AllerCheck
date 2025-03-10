using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllerCheck_Core.Entities
{
    public class Contact
    {
        public int ContactId { get; set; }
        public int UserId { get; set; }
        public string Message { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}
