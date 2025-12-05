using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vossence.DATA.Table
{
    public class tblLanguageContent
    {
        [Key]
        public int LangContentID { get; set; }
        public int LangID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }
}
