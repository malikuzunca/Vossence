using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vossence.DATA.Table
{
    public class tblColors
    {
        [Key]
        public int ColorID { get; set; }
        public string ColorName { get; set; }
    }
}
