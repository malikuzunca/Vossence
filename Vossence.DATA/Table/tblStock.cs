using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vossence.DATA.Table
{
    public class tblStock
    {
        [Key]
        public int StockID { get; set; }
        public int ProductID { get; set; }
        public string Variant { get; set; }
        public int CurrentQuantity { get; set; }
        public int MinStock { get; set; }
    }
}
