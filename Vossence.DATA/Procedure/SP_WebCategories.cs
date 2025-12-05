using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Vossence.DATA.Procedure
{
    public class SP_WebCategories
    {
        [Key]
        public int CategoryID { get; set; } 
        public int CategorySubID { get; set; }
        public string CategoryName { get; set; }
        public string CategorySubName { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string URL { get; set; }
        public string ListImage { get; set; }

    }
}
