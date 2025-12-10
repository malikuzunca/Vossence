using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vossence.DATA.Table;

namespace Vossence.DATA.Procedure
{
    public class SP_Variants
    {
        [Key]
        public int VariantID { get; set; }
        public string? VariantName { get; set; }
        public bool IsActive { get; set; }
        public bool Deleted { get; set; }

    }
}
