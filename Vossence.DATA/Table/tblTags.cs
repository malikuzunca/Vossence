using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vossence.DATA.Table
{
    public class tblTags
    {
        [Key]
        public int TagID { get; set; }
        public string TagName { get; set; }
    }
}
