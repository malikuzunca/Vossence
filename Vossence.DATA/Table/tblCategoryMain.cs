using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Vossence.DATA.Table
{
    public class tblCategoryMain
    {
        [Key]
        public int CategoryID { get; set; }
        public int CategorySubID { get; set; }
        public string CategoryName { get; set; }
        public string ListImage { get; set; }
        public int Arrangement { get; set; }
        public bool IsFeatured { get; set; }
        public bool IsActive { get; set; }
        public bool Deleted { get; set; }
    }
}
