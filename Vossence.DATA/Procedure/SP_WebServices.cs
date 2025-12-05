using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Vossence.DATA.Procedure
{
    public class SP_WebServices
    { 
        public int ServiceID { get; set; }
        public string? ServiceSubName { get; set; }
        public string? ServiceContent { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? URL { get; set; }
        public string? ImageURL { get; set; }
        public bool LangID { get; set; }
        public bool IsActive { get; set; }
        public bool Deleted { get; set; }

    }
}
