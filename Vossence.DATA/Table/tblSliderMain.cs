using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vossence.DATA.Table
{
    public class tblSliderMain
    {
        [Key]
        public int SliderID { get; set; }
        public string ImageURL { get; set; }
        public string SliderName { get; set; }
        public int Arrangement { get; set; }
        public bool IsActive { get; set; }
        public bool Deleted { get; set; }
    }
}
