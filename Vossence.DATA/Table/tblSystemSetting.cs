using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vossence.DATA.Table
{
    public class tblSystemSetting
    {
        [Key]
        public int SystemID { get; set; }
        public string? BusinessName { get; set; }
        public string? EmailAddress { get; set; }
        public string? PhoneNumber { get; set; }
        public string? WpNumber { get; set; }
        public string? LogoURL { get; set; }
        public string? Adress { get; set; }
        public string? CdnURL { get; set; }
        public string? FtpURL { get; set; }
        public string? MapURL { get; set; }
        public string? ProductStart { get; set; }
        public string? HeadCode { get; set; }
        public string? BodyCode { get; set; }
        public string? WorkTime { get; set; }
    }

}
