using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vossence.DATA.Table
{
    public class tblBlogCategoryContent
    {
        [Key]
        public int BlogCategorySubID { get; set; }
        public int BlogCategoryID { get; set; }
        public int LangID { get; set; }
        public string BlogCategorySubName { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string URL { get; set; }
    }
}
