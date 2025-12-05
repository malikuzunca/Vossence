using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vossence.DATA.Table
{
    public class tblLink
    {
        [Key]
        public int LinkID { get; set; }
        public int LangID { get; set; }
        public string LangCode { get; set; }
        public string Link { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public DateTime RegisterDate { get; set; }
        public string GroupID { get; set; }
        public bool IsActive { get; set; }
        public bool Deleted { get; set; }
    }
}
