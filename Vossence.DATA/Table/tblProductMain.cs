using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Vossence.DATA.Table
{
    public class tblProductMain
    {
        [Key]
        public int ProductID { get; set; }
        public int CategoryID { get; set; }
        public int SliderID { get; set; }
        public string ProductName { get; set; }
        public int Arrangement { get; set; }
        public DateTime RegisterName { get; set; }
        public bool IsFeatured { get; set; }
        public bool IsActive { get; set; }
        public bool Deleted { get; set; }
    }
}
