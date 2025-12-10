using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vossence.DATA.Procedure
{
    public class SP_Products
    {
        [Key]
        public int ProductID { get; set; }
        public int CategoryID { get; set; }
        public string ProductName { get; set; }
        public bool IsActive { get; set; }
        public bool IsFeatured { get; set; }
        public bool Deleted { get; set; }
        public int Arrangement { get; set; }
        public DateTime RegisterDate { get; set; }
        public string Description { get; set; }
        public int LangID { get; set; }
        public string Title { get; set; }
        public string ImgURL { get; set; }
        public string URL { get; set; }
        public string ProductCode { get; set; }
        public string ProductContent { get; set; }
        public int SliderID { get; set; }
        public string TagIDs { get; set; }
        public string ColorIDs { get; set; }
        public string TagNames { get; set; }
        public string ColorNames { get; set; }
        public string CategoryName { get; set; }
        public decimal SalesPrice { get; set; }
    }
}
