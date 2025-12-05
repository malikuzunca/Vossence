using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vossence.DATA.Table
{
    public class tblCustomers
    {
        [Key]
        public int CustomerID { get; set; }
        public string Name { get; set; }
        public string SurName { get; set; }
        public string FullName => $"{Name} {SurName}";
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string PasswordHash { get; set; }
        public DateTime RegisterDate { get; set; }
    }
}
