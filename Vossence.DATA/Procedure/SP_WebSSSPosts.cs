using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Vossence.DATA.Procedure
{
    public class SP_WebSSSPosts
    {
        [Key]
        public int SSSPostID { get; set; }
        public int SSSCategoryID { get; set; }
        public string SSSCategorySubName { get; set; }
        public string SSSPostName { get; set; }
        public int LangID { get; set; }
        public string SubQuestion { get; set; }
        public string SubAnswer { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string URL { get; set; }
    }
}
