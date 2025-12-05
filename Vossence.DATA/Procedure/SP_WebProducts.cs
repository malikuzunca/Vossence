using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Vossence.DATA.Procedure
{
    public class SP_WebProducts
    {
        [Key]
        public int ProductID { get; set; }
        public string CategoryName { get; set; }
        public string ProductSubName { get; set; }
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public string ImgURL { get; set; }
        public int StockCount { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string URL { get; set; }
        public int CategoryID { get; set; }
        public DateTime RegisterDate { get; set; }
    }
}
