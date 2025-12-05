using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vossence.DATA.Table
{
    public class tblSSSPostContent
    {
        [Key]
        public int SSSPostSubID { get; set; }
        public int SSSPostID { get; set; }
        public int LangID { get; set; }
        public string SubQuestion { get; set; }
        public string SubAnswer { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string URL { get; set; }
    }
}
