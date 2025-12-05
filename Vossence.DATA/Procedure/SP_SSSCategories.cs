using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vossence.DATA.Procedure
{
    public class SP_SSSCategories
    {
        [Key]
        public int SSSCategoryID { get; set; }
        public string SSSCategoryName { get; set; }
        public string ListImage { get; set; }
        public int Arrangement { get; set; }
        public bool IsActive { get; set; }
        public int TranslateControl { get; set; }
    }
}
