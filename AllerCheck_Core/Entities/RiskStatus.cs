using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllerCheck_Core.Entities
{
    public class RiskStatus
    {
        public int RiskStatusId { get; set; }
        public string RiskStatusName { get; set; } = null!;
        public virtual ICollection<Content> Contents { get; set; } = new List<Content>();
    }
}
