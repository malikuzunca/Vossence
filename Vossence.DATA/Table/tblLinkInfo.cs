using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vossence.DATA.Table
{
    public class tblLinkInfo
    {
        [Key]
        public int ID { get; set; }
        public string Text { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
    }
}
