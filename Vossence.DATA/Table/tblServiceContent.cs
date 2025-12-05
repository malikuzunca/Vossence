using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vossence.DATA.Table
{
    public class tblServiceContent
    {
        [Key]
        public int ServiceSubID { get; set; }
        public int ServiceID { get; set; }
        public int LangID { get; set; }
        public string ServiceSubName { get; set; }
        public string ServiceContent { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string URL { get; set; }
    }
}
