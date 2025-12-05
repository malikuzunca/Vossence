using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vossence.DATA.Table
{
    public class tblBlogCategoryMain
    {
        [Key]
        public int BlogCategoryID { get; set; }
        public string BlogCategoryName { get; set; }
        public int Arrangement { get; set; }
        public bool IsActive { get; set; }
        public bool Deleted { get; set; }
    }
}
