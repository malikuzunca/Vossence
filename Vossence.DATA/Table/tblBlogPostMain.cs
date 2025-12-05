using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vossence.DATA.Table
{
    public class tblBlogPostMain
    {
        [Key]
        public int BlogPostID { get; set; }
        public int BlogCategoryID { get; set; }
        public string BlogPostName { get; set; }
        public string ListImage { get; set; }
        public int Arrangement { get; set; }
        public DateTime RegisterDate { get; set; }
        public bool IsActive { get; set; }
        public bool Deleted { get; set; }
    }
}
