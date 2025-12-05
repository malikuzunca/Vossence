using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vossence.DATA.Table
{
    public class tblSSSPostMain
    {
        [Key]
        public int SSSPostID { get; set; }
        public int CategoryID { get; set; }
        public string SSSPostName { get; set; }
        public int Arrangement { get; set; }
        public bool IsActive { get; set; }
        public bool Deleted { get; set; }
    }
}
