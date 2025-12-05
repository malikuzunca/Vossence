using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vossence.DATA.Table
{
    public class tblFtpSetting
    {
        [Key]
        public int FtpID { get; set; }
        public string FtpIP { get; set; }
        public string FtpPassword { get; set; }
        public string FtpName { get; set; }
    }
}
