using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vossence.DATA.Table
{
    public class tblServiceGallery
    {
        [Key]
        public int FileID { get; set; }
        public int ServiceID { get; set; }
        public int Arrangement { get; set; }
        public string FileURL { get; set; }
    }
}
