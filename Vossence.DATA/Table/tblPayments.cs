using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vossence.DATA.Table
{
    public class tblPayments
    {
        [Key]
        public int PaymentID { get; set; }
        public int OrderID { get; set; }
        public int PaymentMethodID { get; set; }
        public int PaymentStatusID { get; set; }
        public decimal Amount { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime PaymentDate { get; set; }    
    }
}
