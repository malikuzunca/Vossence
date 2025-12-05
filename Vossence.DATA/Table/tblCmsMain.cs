using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vossence.DATA.Table
{
    public class tblCmsMain
    {
        [Key]
        public int CmsID { get; set; }
        public int CmsType { get; set; }
        public int CmsSub { get; set; }
        public string CmsName { get; set; }
        public string SpecialURL { get; set; }
        public int Arrangement { get; set; }
        public bool IsActive { get; set; }
        public bool Deleted { get; set; }
    }
}
