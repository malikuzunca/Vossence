using System.ComponentModel.DataAnnotations;

namespace Vossence.DATA.Procedure;

public class SP_ProductColorStocks
{
    public int ProductID { get; set; }
    public string ProductName { get; set; }
    public int ColorID { get; set; }
    public string ColorName { get; set; }
    public int CurrentQuantity { get; set; }
    public string StatusStock { get; set; }
}