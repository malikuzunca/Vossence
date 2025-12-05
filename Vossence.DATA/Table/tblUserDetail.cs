using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vossence.DATA.Table
{
    public class tblUserDetail
    {
        [Key]
        public int ElementID { get; set; }
        public string UserID { get; set; }
        public string NameSurname { get; set; }
        public string EmailAdress { get; set; }
        public string? Password { get; set; }
        public string? Company { get; set; }
        public string PhoneNumber { get; set; }
    }
}
