using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vossence.DATA.Procedure
{
    public class SP_WebBlogPosts
    {
        [Key]
        public int BlogPostID { get; set; }
        public int BlogCategoryID { get; set; }
        public string BlogCategorySubName { get; set; }
        public string BlogPostName { get; set; }
        public string ListImage { get; set; }
        public DateTime RegisterDate { get; set; }
        public int LangID { get; set; }
        public string BlogPostSubName { get; set; }
        public string BlogPostShortText { get; set; }
        public string BlogPostContent { get; set; }
        public int ClickCount { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string URL { get; set; }
        public string BlogCategoryURL { get; set; }
    }
}
