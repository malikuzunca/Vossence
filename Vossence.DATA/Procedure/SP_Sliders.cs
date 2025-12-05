using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Vossence.DATA.Procedure
{
    public class SP_Sliders
    {
        [Key]
        public int SliderID { get; set; }
        public string SliderName { get; set; }
        public string ImageURL { get; set; }
        public bool IsActive { get; set; }
        public bool Deleted { get; set; }
        public int Arrangement { get; set; }
        public string SlideTitle { get; set; }
        public string SlideText { get; set; }
        public string ButtonText { get; set; }
        public int LangID { get; set; }
        public string URL { get; set; }
    }
}
