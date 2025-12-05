using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vossence.DATA.Table
{
    public class tblProductContent
    {
        [Key]
        public int ProductSubID { get; set; }
        public int ProductID { get; set; }
        public int LangID { get; set; }
        public string ProductSubName { get; set; }
        public string ProductContent { get; set; }
        public string ProductCode { get; set; }
        public string ImgURL { get; set; }
        public int ClickCount { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string URL { get; set; }
}
}
