using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Vossence.DATA.Table
{
    public class tblSliderContent
    {
        [Key]
        public int SliderSubID { get; set; }
        public int SliderID { get; set; }
        public int LangID { get; set; }
        public string SlideTitle { get; set; }
        public string SlideText { get; set; }
        public string ButtonText { get; set; }
        public string URL { get; set; }
    }
}
