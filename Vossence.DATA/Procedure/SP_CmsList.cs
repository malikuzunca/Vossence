using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vossence.DATA.Procedure
{
    public class SP_CmsList
    {
        [Key]
        public int CmsID { get; set; }
        public int CmsSub { get; set; }
        public string CmsSubGet { get; set; }
        public int CmsType { get; set; }
        public string CmsName { get; set; }
        public int Arrangement { get; set; }
        public bool IsActive { get; set; }
        public string ExternalURL { get; set; }
        public int TranslateControl { get; set; }
    }
}
