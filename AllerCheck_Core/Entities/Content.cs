using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllerCheck_Core.Entities
{
    public class Content
    {
        public int ContentId { get; set; }

        public string ContentName { get; set; } = null!;

        public int RiskStatusId { get; set; }

        public string ContentInfo { get; set; } = null!;

        public virtual RiskStatus RiskStatus { get; set; } = null!;

        public virtual ICollection<BlackList> BlackLists{ get; set; } = new List<BlackList>();

        public virtual ICollection<ContentProduct> ContentProducts { get; set; } = new List<ContentProduct>();
    }
}
