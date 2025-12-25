using Dapper;
using Microsoft.AspNetCore.Mvc;
using Vossence.DATA.ORM;
using Vossence.DATA.Procedure;
using Vossence.DATA.Table;
using Vossence.DATA.Validation;
using static Vossence.DATA.Validation.Cls;

namespace Vossence.ADMIN.Controllers;

public class SalesController : SharedController
{
    private readonly IDapper db;
    private readonly IConfiguration? configuration;

    #region Ctor

    public SalesController(IDapper dapper, IConfiguration? configuration, ILogger<SharedController> logger) : base(
        dapper, configuration, logger)
    {
        this.db = dapper;
        this.configuration = configuration;
    }

    #endregion

    #region Manuel Satış Ekranı

    #region MyRegion

    #endregion

    [HttpGet]
    [Route("manual-sale")]
    public IActionResult ManualSales()
    {
        List<SP_AdminProducts> productsList = db.GetAll<SP_AdminProducts>("SP_AdminProducts", new DynamicParameters(
            new Dictionary<string, object>
            {
                { "@langID", langID }
            })).ToList();
        List<SP_ProductColorStocks> productStocks = db.GetAll<SP_ProductColorStocks>("SP_ProductColorStocks",
            new DynamicParameters(new Dictionary<string, object>
            {
                { "@productID", -1 }
            })).ToList();

        return View(Tuple.Create(productsList, productStocks));
    }

    #endregion

    #region Sepete Ürün Ekleme
    public async Task<ResultModel> CartProcess(int productID, int colorID, decimal price)
    {
        try
        {
            tblStock? hasStock =
                db.QueryApp<tblStock>(string.Format("SELECT * FROM tblStock WHERE ProductID={0} AND Variant='{1}'",
                    productID, colorID)).FirstOrDefault();
            if (hasStock != null)
            {
                List<CartModel> cartList = new List<CartModel>();
                if (HttpContext.Session.GetString("CART") != null)
                {
                    cartList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<CartModel>>(
                        HttpContext.Session.GetString("CART"));
                }

                CartModel? isInCart = cartList.Where(x => x.ProductID == productID && x.ColorID == colorID)
                    .FirstOrDefault();
                if (isInCart != null)
                {
                    if (hasStock.CurrentQuantity > isInCart.Quantity)
                    {
                        isInCart.Quantity += 1;
                        isInCart.TotalPrice = isInCart.Quantity * isInCart.UnitPrice;
                    }
                    else
                    {
                        return await Task.FromResult(new Cls.ResultModel()
                        {
                            resultType = 0, resultCaption = "Stok Yetersiz",
                            resultMessage = "Seçilen ürünün stok adedi yetersizdir."
                        });
                    }
                }
                else
                {
                    tblColors? infoColor = db.QueryApp<tblColors>(string.Format("SELECT * FROM tblColors WHERE ColorID={0}",
                        colorID)).FirstOrDefault();
                    CartModel newCartItem = new CartModel()
                    {
                        ProductID = productID,
                        ColorID = colorID,
                        ColorName =  infoColor.ColorName,
                        Quantity = 1,
                        UnitPrice = price,
                        TotalPrice = price
                    };
                    cartList.Add(newCartItem);
                }

                HttpContext.Session.SetString("CART", Newtonsoft.Json.JsonConvert.SerializeObject(cartList));
                return await Task.FromResult(new Cls.ResultModel()
                    { resultType = 1, resultCaption = "Başarılı", resultMessage = "Ürün sepete eklendi." });
            }
            else
            {
                return await Task.FromResult(new Cls.ResultModel()
                {
                    resultType = 0, resultCaption = "Stok Bulunamadı",
                    resultMessage = "Seçilen ürünün stoğu bulunamadı."
                });
            }
        }
        catch (Exception)
        {
            await Log(false, "AddToCart");
            return await Task.FromResult(new Cls.ResultModel()
            {
                resultType = 0, resultCaption = "Hata",
                resultMessage = "Bir hata oluştu lütfen sistem yöneticinize başvurun."
            });
        }
    }

    #endregion
}