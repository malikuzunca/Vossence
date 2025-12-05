using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vossence.DATA.Table
{
    public class tblLanguage
    {
        [Key]
        public int LangID { get; set; }
        public string LangName { get; set; }
        public string LangCode { get; set; }
        public bool LangDefault { get; set; }
        public bool IsActive { get; set; }
        public int Arrangement { get; set; }
        public string Link { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int GroupID { get; set; }
        public string Flag { get; set; }

    }
}
