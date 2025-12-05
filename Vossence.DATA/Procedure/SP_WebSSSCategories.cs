using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Vossence.DATA.Procedure
{
    public class SP_WebSSSCategories
    {
        [Key]
        public int SSSCategoryID { get; set; }
        public string SSSCategoryName { get; set; }
        public int LangID { get; set; }
        public string ListImage { get; set; }
        public string CategorySubName { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string URL { get; set; }
    }
}
