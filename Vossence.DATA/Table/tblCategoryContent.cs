using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Vossence.DATA.Table
{
    public class tblCategoryContent
    {
        [Key]
        public int CategorySubID { get; set; }
        public int CategoryID { get; set; }
        public int LangID { get; set; }
        public string CategorySubName { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string URL { get; set; }
    }
}
