using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vossence.DATA.Procedure
{
    public class SP_Services
    {
        [Key]
        public int ServiceID { get; set; }
        public string ServiceName { get; set; }
        public string ListImage { get; set; }
        public string ImageURL { get; set; }
        public int Arrangement { get; set; }
        public bool IsActive { get; set; }
        public int TranslateControl { get; set; }
    }
}
