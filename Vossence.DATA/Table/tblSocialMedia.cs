using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vossence.DATA.Table
{
    public class tblSocialMedia
    {
        [Key]
        public int SocialID { get; set; }
        public string Title { get; set; }
        public string Icon { get; set; }
        public string Link { get; set; }
    }
}
