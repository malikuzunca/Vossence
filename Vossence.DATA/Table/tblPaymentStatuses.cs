using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vossence.DATA.Table
{
    public class tblPaymentStatuses
    {
        [Key]
        public int PaymentStatusID { get; set; }
        public string StatusName { get; set; }
        public bool IsActive { get; set; }
        public bool Deleted { get; set; }
    }
}
