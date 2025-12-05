using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vossence.DATA.Procedure
{
    public class SP_WebCmsItems
    {
        [Key]
        public int CmsID { get; set; }
        public int CmsType { get; set; }
        public int CmsSub { get; set; }
        public string CmsName { get; set; }
        public string ExternalURL { get; set; }
        public int LangID { get; set; }
        public string CmsSubName { get; set; }
        public string CmsContent { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string URL { get; set; }
    }
}
