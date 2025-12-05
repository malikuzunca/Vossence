using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace Vossence.DATA.Table
{
    public class tblStockAction
    {
        [Key]
        public int StockActionID { get; set; }
        public int StockID { get; set; }
        public string UserID { get; set; }
        public string Type { get; set; }
        public int TotalCost { get; set; }
        public int UnitCost { get; set; }
        public int Quantity { get; set; }
        public DateTime ActionDate { get; set; }
        public string Note { get; set; }
    }
}
