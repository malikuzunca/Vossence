using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vossence.DATA.Procedure
{
    public class SP_Categories
    {
        [Key]
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public bool IsActive { get; set; }
        public bool IsFeatured { get; set; }
        public bool Deleted { get; set; }
        public int Arrangement { get; set; }
        public string Description { get; set; }
        public int LangID { get; set; }
        public string Title { get; set; }
        public string URL { get; set; }
        public string ListImage { get; set; }

    }
}
