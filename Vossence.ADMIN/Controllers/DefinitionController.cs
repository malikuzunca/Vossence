using Dapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using System;
using System.Drawing;
using System.Text.RegularExpressions;
using Vossence.DATA.Helper;
using Vossence.DATA.Identity;
using Vossence.DATA.ORM;
using Vossence.DATA.Procedure;
using Vossence.DATA.Table;
using static Vossence.DATA.Validation.Cls;

namespace Vossence.ADMIN.Controllers
{
    public class DefinitionController : SharedController
    {
        private readonly IDapper db;
        private readonly IConfiguration? configuration;
        private readonly UserManager<AppUser> userManager; 
        private readonly RoleManager<AppRole> roleManager;
        private readonly SignInManager<AppUser> signManager;

        #region Ctor
        public DefinitionController(IDapper dapper, IConfiguration? configuration, ILogger<SharedController> logger, UserManager<AppUser> userManager, SignInManager<AppUser> signManager, RoleManager<AppRole> roleManager) : base(dapper, configuration, logger)
        {
            this.db = dapper;
            this.configuration = configuration;
            this.userManager = userManager;
            this.signManager = signManager;
            this.roleManager = roleManager;
        }
        #endregion

        #region Kategoriler

        #region Liste

        [Route("category-management")]
        public IActionResult CategoryManagement()
        {
            List<SP_Categories> list = db.GetAll<SP_Categories>("SP_Categories", new DynamicParameters(new Dictionary<string, object>
            {
                { "@LangID", langID }
            })); 
            List<tblCategoryMain> categoryList = db.QueryApp<tblCategoryMain>("SELECT * FROM tblCategoryMain WHERE IsActive=1 AND Deleted=0 ORDER BY Arrangement");
            return View(Tuple.Create(list, categoryList));
        }

        #endregion

        #region Oluştur - Güncelle

        [Route("category-app")]
        public async Task<ResultModel> CategoryApp(LangItem[] list)
        {
            try
            {
                IFormCollection model = Request.Form;

                int categoryID = Convert.ToInt32(FormRowGet(model, "categoryID"));
                int categorySubID = Convert.ToInt32(FormRowGet(model, "categorySubID"));
                if (categoryID == 0)
                {
                    Task<ResultModel> mainTable = Task.FromResult(db.Insert<ResultModel>("SP_CategoryCrud", new DynamicParameters(new Dictionary<string, object?>
                    {
                        { "@processType", 1 },
                        { "@rowType", 1 },
                        { "@categoryID", 0 },
                        { "@categorySubID", categorySubID },
                        { "@langID", langID },
                        { "@categoryName", FormRowGet(model, "categoryName") },
                        { "@categorySubName", "" },
                        { "@title", "" },
                        { "@description", "" },
                        { "@url", "" },
                    })));

                    if (mainTable.IsCompletedSuccessfully)
                    {
                        if (list != null)
                        {
                            foreach (var item in list)
                            {
                                string? caption = !string.IsNullOrEmpty(item.caption) ? item.caption.Replace("'", "''") : item.caption;
                                string? content = !string.IsNullOrEmpty(item.content) ? item.content.Replace("'", "''") : item.content;
                                string? title = !string.IsNullOrEmpty(item.title) ? item.title.Replace("'", "''") : item.title;
                                string? description = !string.IsNullOrEmpty(item.description) ? item.description.Replace("'", "''") : item.description;

                                if (!string.IsNullOrEmpty(item.caption))
                                {
                                    categoryID = MainTableMaxID("tblCategoryMain").Result;
                                    string groupID = "Category_" + categoryID;
                                    string url = AppFunc.TextLinkReturning(item.url);

                                    Task<ResultModel> contentTable = Task.FromResult(db.Insert<ResultModel>("SP_CategoryCrud", new DynamicParameters(new Dictionary<string, object?>
                                    {
                                        { "@processType", 2 },
                                        { "@rowType", 1 },
                                        { "@categoryID", categoryID },
                                        { "@categorySubID", categorySubID },
                                        { "@langID", item.langID },
                                        { "@categoryName", FormRowGet(model, "categoryName") },
                                        { "@categorySubName", item.caption },
                                        { "@title", item.title },
                                        { "@description", item.description },
                                        { "@url", url },
                                    })));

                                    LinkApp(1, url, item.langID, "Page", "Category", groupID, FormRowGet(model, "categoryName"));
                                }
                            }
                            return await Task.FromResult(new ResultModel() { resultType = 1, resultCaption = "Başarılı", resultMessage = "Güncelleme Başarılı" });
                        }
                        else return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Uyarı", resultMessage = "Kayıt Bulunamadı" });
                    }
                    else return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Hata", resultMessage = "Bir hata oluştu lütfen sistem yöneticinize başvurun." });
                }
                else if (categoryID > 0)
                {
                    tblCategoryMain? categoryGet = db.QueryApp<tblCategoryMain>(string.Format("SELECT * FROM tblCategoryMain WHERE CategoryID={0}", categoryID)).FirstOrDefault();
                    if (categoryGet != null)
                    {
                        Task<ResultModel> mainTable = Task.FromResult(db.Insert<ResultModel>("SP_CategoryCrud", new DynamicParameters(new Dictionary<string, object?>
                        {
                            { "@processType", 1 },
                            { "@rowType", 2 },
                            { "@categoryID", categoryGet.CategoryID },
                            { "@categorySubID", categorySubID },
                            { "@langID", langID },
                            { "@categoryName", FormRowGet(model, "categoryName") },
                            { "@categorySubName", "" },
                            { "@title", "" },
                            { "@description", "" },
                            { "@url", "" },
                        })));

                        if (mainTable.IsCompletedSuccessfully)
                        {
                            if (list != null)
                            {
                                foreach (var item in list)
                                {
                                    string? caption = !string.IsNullOrEmpty(item.caption) ? item.caption.Replace("'", "''") : item.caption;
                                    string? content = !string.IsNullOrEmpty(item.content) ? item.content.Replace("'", "''") : item.content;
                                    string? title = !string.IsNullOrEmpty(item.title) ? item.title.Replace("'", "''") : item.title;
                                    string? description = !string.IsNullOrEmpty(item.description) ? item.description.Replace("'", "''") : item.description;

                                    tblCategoryContent? cnt = db.QueryApp<tblCategoryContent>(string.Format("SELECT * FROM tblCategoryContent WHERE CategoryID={0} AND LangID={1}", categoryGet.CategoryID, item.langID)).FirstOrDefault();
                                    if (cnt != null)
                                    {
                                        string groupID = "Category_" + categoryID;
                                        string url = AppFunc.TextLinkReturning(item.url);
                                        LinkApp(2, "", item.langID, "Page", "Category", groupID, FormRowGet(model, "categoryName"));

                                        if (!string.IsNullOrEmpty(item.caption))
                                        {
                                            Task<ResultModel> contentTable = Task.FromResult(db.Insert<ResultModel>("SP_CategoryCrud", new DynamicParameters(new Dictionary<string, object?>
                                            {
                                                { "@processType", 2 },
                                                { "@rowType", 2 },
                                                { "@categoryID", cnt.CategoryID },
                                                { "@categorySubID", cnt.CategorySubID },
                                                { "@langID", item.langID },
                                                { "@categoryName", FormRowGet(model, "categoryName") },
                                                { "@categorySubName", item.caption },
                                                { "@title", item.title },
                                                { "@description", item.description },
                                                { "@url", url },
                                            })));

                                            LinkApp(1, url, item.langID, "Page", "Category", groupID, FormRowGet(model, "categoryName"));
                                        }
                                        else
                                        {
                                            Task<ResultModel?> deleteTable = Task.FromResult(db.QueryApp<ResultModel>(string.Format("DELETE FROM tblCategoryContent WHERE CategoryID={0} AND LangID={1}", categoryGet.CategoryID, item.langID)).FirstOrDefault());
                                        }
                                    }
                                    else
                                    {
                                        if (!string.IsNullOrEmpty(item.caption))
                                        {
                                            string groupID = "Category_" + categoryID;
                                            string url = AppFunc.TextLinkReturning(item.url);

                                            Task<ResultModel> contentTable = Task.FromResult(db.Insert<ResultModel>("SP_CategoryCrud", new DynamicParameters(new Dictionary<string, object?>
                                            {
                                                { "@processType", 2 },
                                                { "@rowType", 1 },
                                                { "@categoryID", categoryID },
                                                { "@categorySubID", categorySubID },
                                                { "@langID", item.langID },
                                                { "@categoryName", FormRowGet(model, "categoryName") },
                                                { "@categorySubName", item.caption },
                                                { "@title", item.title },
                                                { "@description", item.description },
                                                { "@url", url },
                                            })));

                                            LinkApp(1, url, item.langID, "Page", "Category", groupID, FormRowGet(model, "categoryName"));
                                        }
                                    }
                                }
                                return await Task.FromResult(new ResultModel() { resultType = 1, resultCaption = "Başarılı", resultMessage = "Güncelleme Başarılı" });
                            }
                            else return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Uyarı", resultMessage = "Kayıt Bulunamadı" });
                        }
                        else return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Hata", resultMessage = "Bir hata oluştu lütfen sistem yöneticinize başvurun." });
                    }
                    else return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Uyarı", resultMessage = "Kayıt Bulunamadı" });
                }
                else return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Uyarı", resultMessage = "Kayıt Bulunamadı" });
            }
            catch (Exception)
            {
                await Log(false, "CategoryApp");
                return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Hata", resultMessage = "Bir hata oluştu lütfen sistem yöneticinize başvurun." });
            }
        }
        #endregion

        #region Durum ÖneÇıkan Vitrin Güncelle - Sil
        [Route("category-process")]
        [HttpPost]
        public async Task<ResultModel> CategoryProcess(int processType, int categoryID)
        {
            try
            {
                tblCategoryMain? categoryGet = db.QueryApp<tblCategoryMain>(string.Format("SELECT * FROM tblCategoryMain WHERE CategoryID={0}", categoryID)).FirstOrDefault();
                if (categoryGet != null)
                {
                    if (processType == 1)
                    {
                        int isActive = categoryGet.IsActive == true ? 0 : 1;

                        Task<ResultModel?> processApp = Task.FromResult(db.QueryApp<ResultModel>(string.Format("UPDATE tblCategoryMain SET IsActive= {1} WHERE CategoryID={0}", categoryGet.CategoryID, isActive)).FirstOrDefault());

                        await Log(processApp.IsCompletedSuccessfully, "CategoryProcess");
                        if (processApp.IsCompletedSuccessfully)
                            return await Task.FromResult(new ResultModel() { resultType = 1, resultCaption = "Başarılı", resultMessage = "Başarılı Güncelleme" });
                        else
                            return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Uyarı", resultMessage = "Kayıt Bulunamadı" });
                    }
                    else if (processType == 2)
                    {
                        Task<ResultModel?> processApp = Task.FromResult(db.QueryApp<ResultModel>(string.Format("UPDATE tblCategoryMain SET Deleted = 1 WHERE CategoryID={0}", categoryGet.CategoryID)).FirstOrDefault());

                        await Log(processApp.IsCompletedSuccessfully, "CategoryProcess");
                        if (processApp.IsCompletedSuccessfully)
                            return await Task.FromResult(new ResultModel() { resultType = 1, resultCaption = "Başarılı", resultMessage = "Başarılı Güncelleme" });
                        else
                            return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Uyarı", resultMessage = "Kayıt Bulunamadı" });
                    }
                    else if (processType == 3)
                    {
                        int isFeatured = categoryGet.IsFeatured == true ? 0 : 1;
                        Task<ResultModel?> processApp = Task.FromResult(db.QueryApp<ResultModel>(string.Format("UPDATE tblCategoryMain SET IsFeatured= {1} WHERE CategoryID={0}", categoryGet.CategoryID, isFeatured)).FirstOrDefault());
                        await Log(processApp.IsCompletedSuccessfully, "CategoryProcess");
                        if (processApp.IsCompletedSuccessfully)
                            return await Task.FromResult(new ResultModel() { resultType = 1, resultCaption = "Başarılı", resultMessage = "Başarılı Güncelleme" });
                        else
                            return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Uyarı", resultMessage = "Kayıt Bulunamadı" });
                    }
                    else return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption ="Hata", resultMessage = "Bir hata oluştu lütfen sistem yöneticinize başvurun." });
                }
                else return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption ="Hata", resultMessage = "Bir hata oluştu lütfen sistem yöneticinize başvurun." });
            }
            catch (Exception)
            {
                await Log(false, "CategoryProcess");
                return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Uyarı", resultMessage = "Kayıt Bulunamadı" });
            }
        }
        #endregion

        #region Sıralama Güncelle
        [Route("category-arrangement-update")]
        public async Task<ResultModel> CategoryArrangementUpdate(string arrangementBody)
        {
            try
            {
                if (!string.IsNullOrEmpty(arrangementBody))
                {
                    foreach (var item in arrangementBody.Split('_'))
                    {
                        if (item != "" && item != "_" && item != null)
                        {
                            int categoryID = Convert.ToInt32(item.Split(',').First());
                            int arrangement = Convert.ToInt32(item.Split(',').Last());

                            tblCategoryMain? categoryGet = db.QueryApp<tblCategoryMain>(string.Format("SELECT * FROM tblCategoryMain WHERE CategoryID={0}", categoryID)).FirstOrDefault();
                            if (categoryGet != null)
                            {
                                Task<ResultModel?> result = Task.FromResult(db.QueryApp<ResultModel>(string.Format("UPDATE tblCategoryMain SET Arrangement={0} WHERE CategoryID={1}", arrangement, categoryID)).FirstOrDefault());
                            }
                            else return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption ="Uyarı", resultMessage = "Kayıt Bulunamadı" });
                        }
                    }
                    await Log(true, "CategoryArrangementUpdate");
                    return await Task.FromResult(new ResultModel() { resultType = 1, resultCaption = "Başarılı", resultMessage ="Başarılı Güncelleme" });
                }
                else return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption ="Uyarı", resultMessage = "Kayıt Bulunamadı" });
            }
            catch (Exception)
            {
                await Log(false, "CategoryArrangementUpdate");
                return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Hata", resultMessage = "Bir hata oluştu lütfen sistem yöneticinize başvurun." });
            }
        }
        #endregion

        #region Görsel
        [Route("category-image-app")]
        public async Task<ResultModel> CategoryImageApp(int categoryID)
        {
            try
            {
                var files = Request.Form.Files;
                if (files.Count() > 0)
                {
                    string categoryImg = FtpPost(Request.Form.Files[0], "Vossence/Categories", "category");
                    if (categoryImg != "")
                    {
                        Task<ResultModel?> result = Task.FromResult(db.QueryApp<ResultModel>(string.Format("UPDATE tblCategoryContent SET ImageURL='{0}' WHERE CategoryID={1}", categoryImg, categoryID)).FirstOrDefault());
                        await Log(true, "CategoryImageApp");
                        if (result.IsCompletedSuccessfully)
                            return await Task.FromResult(new ResultModel() { resultType = 1, resultCaption = "Başarılı", resultMessage = "Güncelleme Başarılı" });
                        else return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Hata", resultMessage = "Bir hata oluştu lütfen sistem yöneticinize başvurun." });
                    }
                }
                await Log(true, "CategoryImageApp");
                return await Task.FromResult(new ResultModel() { resultType = 1, resultCaption = "Başarılı", resultMessage = "Başarılı Güncelleme" });
            }
            catch (Exception)
            {
                await Log(false, "CategoryImageApp");
                return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Hata", resultMessage = "Bir hata oluştu lütfen sistem yöneticinize başvurun." });
            }
        }
        #endregion

        #endregion

        #region Ürünler

        #region Liste

        [Route("product-management")]
        public IActionResult ProductManagement()
        {
            List<SP_Products> list = db.GetAll<SP_Products>("SP_Products", new DynamicParameters(new Dictionary<string, object>
            {
                { "@LangID", langID },
                { "@sliderID", -1 }
            }));
            List<tblCategoryMain> categoryList = db.QueryApp<tblCategoryMain>("SELECT * FROM tblCategoryMain WHERE IsActive=1 AND Deleted=0 ORDER BY Arrangement");
            List<tblTags> tagList = db.QueryApp<tblTags>("SELECT * FROM tblTags");
            List<tblColors> colorList = db.QueryApp<tblColors>("SELECT * FROM tblColors");
            List<tblStock> stockList = db.QueryApp<tblStock>("Select * From tblStock");
            return View(Tuple.Create(list, categoryList, tagList, colorList, stockList));
        }

        #endregion

        #region Oluştur - Güncelle

        [Route("product-app")]
        public async Task<ResultModel> ProductApp()
        {
            try
            {
                IFormCollection model = Request.Form;
                int mainCat = Convert.ToInt32(FormRowGet(model, "mainCat"));
                int subCat = Convert.ToInt32(FormRowGet(model, "subCat"));
                int subSubCat = Convert.ToInt32(FormRowGet(model, "subSubCat"));
                int categoryID = mainCat;

                if (subCat > 0) { 
                    categoryID = subCat;
                    if(subSubCat > 0)
                        categoryID = subSubCat;
                }

                int productID = Convert.ToInt32(FormRowGet(model, "productID"));

                if (productID == 0)
                {
                    Task<ResultModel> mainTable = Task.FromResult(db.Insert<ResultModel>("SP_ProductCrud", new DynamicParameters(new Dictionary<string, object?>
                    {
                        { "@processType", 1 },
                        { "@rowType", 1 },
                        { "@productID", 0 },
                        { "@categoryID", categoryID },
                        { "@langID", 0 },
                        { "@productName", FormRowGet(model, "productName") },
                        { "@productSubName", "" },
                        { "@productCode", 0},
                        { "@productContent", ""},
                        { "@title", "" },
                        { "@description", "" },
                        { "@url", "" },
                    })));
                    
                    if (mainTable.IsCompletedSuccessfully)
                    {
                        if (model != null)
                        {
                                productID = MainTableMaxID("tblProductMain").Result;
                                string groupID = "Product_" + productID;
                                string url = AppFunc.TextLinkReturning(FormRowGet(model, "url"));
                                string? productCode = ProductCode();
                                string tagIDs = string.Join(",", FormRowGet(model, "tags[]"));
                                string colorIDs = string.Join(",", FormRowGet(model, "colors[]"));

                            Task<ResultModel> contentTable = Task.FromResult(db.Insert<ResultModel>("SP_ProductCrud", new DynamicParameters(new Dictionary<string, object?>
                                {
                                    { "@processType", 2 },
                                    { "@rowType", 1 },
                                    { "@productID", productID },
                                    { "@categoryID", categoryID },
                                    { "@langID", langID },
                                    { "@productName", FormRowGet(model, "productName") },
                                    { "@productSubName",FormRowGet(model, "productName") },
                                    { "@productCode", productCode},
                                    { "@productContent",FormRowGet(model, "productContent")},
                                    { "@title", FormRowGet(model, "title") },
                                    { "@description", FormRowGet(model, "description") },
                                    { "@url", url },
                                })));
                            Task<ResultModel> tagTable = Task.FromResult(db.Insert<ResultModel>("SP_ProductTagsCrud", new DynamicParameters(new Dictionary<string, object?>
                            {
                                { "@processType", 1 },
                                { "@rowType", 0 },
                                { "@productID", productID },
                                { "@tagIDs",tagIDs},
                            })));
                            Task<ResultModel> colorTable = Task.FromResult(db.Insert<ResultModel>("SP_ProductColorsCrud", new DynamicParameters(new Dictionary<string, object?>
                            {
                                { "@processType", 1 },
                                { "@rowType", 0 },
                                { "@productID", productID },
                                { "@colorIDs", colorIDs},
                            })));
                            LinkApp(1, url, langID, "Page", "ProductDetail", groupID, FormRowGet(model, "productName"));
                            
                            return await Task.FromResult(new ResultModel() { resultType = 1, resultCaption = "Başarılı", resultMessage = "Güncelleme Başarılı" });
                        }
                        else return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Uyarı", resultMessage = "Kayıt Bulunamadı" });
                    }
                    else return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Hata", resultMessage = "Bir hata oluştu lütfen sistem yöneticinize başvurun." });
                }
                else if (productID > 0)
                {
                    tblProductMain? productGet = db.QueryApp<tblProductMain>(string.Format("SELECT * FROM tblProductMain WHERE ProductID={0}", productID)).FirstOrDefault();
                    if (productGet != null)
                    {
                        Task<ResultModel> mainTable = Task.FromResult(db.Insert<ResultModel>("SP_ProductCrud", new DynamicParameters(new Dictionary<string, object?>
                        {
                            { "@processType", 1 },
                            { "@rowType", 2 },
                            { "@productID", productGet.ProductID },
                            { "@categoryID", categoryID },
                            { "@langID", 0 },
                            { "@productName", FormRowGet(model, "productName") },
                            { "@productSubName", "" },
                            { "@productCode", 0},
                            { "@productContent", ""},
                            { "@title", "" },
                            { "@description", "" },
                            { "@url", "" },
                        })));

                        if (mainTable.IsCompletedSuccessfully)
                        {
                            if (model != null)
                            {
                                    tblProductContent? cnt = db.QueryApp<tblProductContent>(string.Format("SELECT * FROM tblProductContent WHERE ProductID={0} AND LangID={1}", productGet.ProductID, langID)).FirstOrDefault();
                                    if (cnt != null)
                                    {
                                        string groupID = "Product_" + productID;
                                        string tagIDs = string.Join(",", FormRowGet(model, "tags[]"));
                                        string colorIDs = string.Join(",", FormRowGet(model, "colors[]"));
                                        string url = AppFunc.TextLinkReturning(FormRowGet(model, "url"));
                                        LinkApp(2, "", langID, "Page", "ProductDetail", groupID, FormRowGet(model, "productName"));

                                        if (!string.IsNullOrEmpty(FormRowGet(model, "productName")))
                                        {
                                            Task<ResultModel> contentTable = Task.FromResult(db.Insert<ResultModel>("SP_ProductCrud", new DynamicParameters(new Dictionary<string, object?>
                                            {
                                                { "@processType", 2 },
                                                { "@rowType", 2 },
                                                { "@productID", cnt.ProductID },
                                                { "@categoryID", categoryID },
                                                { "@langID", langID },
                                                { "@productName", FormRowGet(model, "productName") },
                                                { "@productSubName", FormRowGet(model, "productName") },
                                                { "@productCode", 0},
                                                { "@productContent",FormRowGet(model, "productContent")},
                                                { "@title", FormRowGet(model, "title") },
                                                { "@description", FormRowGet(model, "description") },
                                                { "@url", url },
                                            })));
                                        Task<ResultModel> tagTable = Task.FromResult(db.Insert<ResultModel>("SP_ProductTagsCrud", new DynamicParameters(new Dictionary<string, object?>
                        {
                                                { "@processType", 1 },
                                                { "@rowType", 1 },
                                                { "@productID", cnt.ProductID },
                                                { "@tagIDs", tagIDs },
                                            })));
                                        Task<ResultModel> colorTable = Task.FromResult(db.Insert<ResultModel>("SP_ProductColorsCrud", new DynamicParameters(new Dictionary<string, object?>
                        {
                                                { "@processType", 1 },
                                                { "@rowType", 1 },
                                                { "@productID", cnt.ProductID },
                                                { "@colorIDs", colorIDs },
                                            })));
                                        LinkApp(1, url, langID, "Page", "ProductDetail", groupID, FormRowGet(model, "productName"));
                                        }
                                        else
                                        {
                                            Task<ResultModel?> deleteTable = Task.FromResult(db.QueryApp<ResultModel>(string.Format("DELETE FROM tblProductContent WHERE ProductID={0} AND LangID={1}", productGet.ProductID, langID)).FirstOrDefault());
                                        }
                                    }
                                    else
                                    {
                                        if (!string.IsNullOrEmpty(FormRowGet(model, "productName")))
                                        {
                                            string groupID = "Product_" + productID;
                                            string url = AppFunc.TextLinkReturning(FormRowGet(model, "url"));
                                            string? productCode = ProductCode();

                                        Task<ResultModel> contentTable = Task.FromResult(db.Insert<ResultModel>("SP_ProductCrud", new DynamicParameters(new Dictionary<string, object?>
                                            {
                                                { "@processType", 2 },
                                                { "@rowType", 1 },
                                                { "@productID", productID },
                                                { "@categoryID", categoryID },
                                                { "@langID", langID },
                                                { "@productName", FormRowGet(model, "productName") },
                                                { "@productSubName", FormRowGet(model, "productName") },
                                                { "@productCode", productCode},
                                                { "@productContent",FormRowGet(model, "productContent")},
                                                { "@title", FormRowGet(model, "title") },
                                                { "@description", FormRowGet(model, "description") },
                                                { "@url", url },
                                            })));
                                           
                                        LinkApp(1, url, langID, "Page", "ProductDetail", groupID, FormRowGet(model, "productName"));
                                        }
                                    }
                                
                                return await Task.FromResult(new ResultModel() { resultType = 1, resultCaption = "Başarılı", resultMessage = "Güncelleme Başarılı" });
                            }
                            else return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Uyarı", resultMessage = "Kayıt Bulunamadı" });
                        }
                        else return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Hata", resultMessage = "Bir hata oluştu lütfen sistem yöneticinize başvurun." });
                    }
                    else return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Uyarı", resultMessage = "Kayıt Bulunamadı" });
                }
                else return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Uyarı", resultMessage = "Kayıt Bulunamadı" });
            }
            catch (Exception)
            {
                await Log(false, "ProductApp");
                return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Hata", resultMessage = "Bir hata oluştu lütfen sistem yöneticinize başvurun." });
            }
        }
        #endregion

        #region Stok Güncelleme

        [Route("product-stock-process")]
        [HttpPost]
        public async Task<ResultModel> ProductStockProcess()
        {
            IFormCollection model = Request.Form;
            int stockID = Convert.ToInt32(FormRowGet(model, "stockID"));
            int colorID = Convert.ToInt32(FormRowGet(model, "colorID"));
            string? userName = User.Identity!.Name;

            try
            {
                if (stockID == 0)
                {
                    int quantity = Convert.ToInt32(FormRowGet(model, "quantity"));
                    Task<ResultModel> mainTable = Task.FromResult(db.Insert<ResultModel>("SP_StockCrud", new DynamicParameters(new Dictionary<string, object?>
                    {
                        { "@processType", 1 },
                        { "@rowType", 1 },
                        { "@stockID", 0 },
                        { "@productID", FormRowGet(model, "productID") },
                        { "@variant", colorID },
                        { "@userID", "" },
                        { "@type", ""},
                        { "@unitCost", ""},
                        { "@quantity", quantity },
                        { "@note", "" },
                    })));
                    if (mainTable.IsCompletedSuccessfully)
                    {
                        if (userName != null)
                        {
                            stockID = MainTableMaxID("tblStock").Result;
                            string? userID = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                            Task<ResultModel> contentTable = Task.FromResult(db.Insert<ResultModel>("SP_StockCrud", new DynamicParameters(new Dictionary<string, object?>
                                {
                                    { "@processType", 2 },
                                    { "@rowType", 1 },
                                    { "@stockID", stockID },
                                    { "@productID", FormRowGet(model, "productID") },
                                    { "@variant", colorID },
                                    { "@userID", userID },
                                    { "@type", "Giriş"},
                                    { "@unitCost", FormRowGet(model,"price")},
                                    { "@quantity", quantity },
                                    { "@note", FormRowGet(model,"message") },
                                })));

                            return await Task.FromResult(new ResultModel() { resultType = 1, resultCaption = "Başarılı", resultMessage = "Güncelleme Başarılı" });
                        }
                        else return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Uyarı", resultMessage = "Kayıt Bulunamadı" });
                    }
                    else return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Hata", resultMessage = "Bir hata oluştu lütfen sistem yöneticinize başvurun." });
                }
                else if(stockID > 0)
                {
                    tblStock? stockGet = db.QueryApp<tblStock>(string.Format("SELECT * FROM tblStock WHERE Variant={0}",colorID)).FirstOrDefault();
                    if (stockGet != null)
                    {
                        int quantity = Convert.ToInt32(FormRowGet(model, "quantity"));
                        Task<ResultModel> mainTable = Task.FromResult(db.Insert<ResultModel>("SP_StockCrud", new DynamicParameters(new Dictionary<string, object?>
                        {
                            { "@processType", 1 },
                            { "@rowType", 2 },
                            { "@stockID", stockGet.StockID },
                            { "@productID", FormRowGet(model, "productID") },
                            { "@variant", colorID },
                            { "@userID", "" },
                            { "@type", ""},
                            { "@unitCost", ""},
                            { "@quantity", quantity },
                            { "@note", "" },
                        })));
                        if (mainTable.IsCompletedSuccessfully)
                        {
                            if (userName != null)
                            {
                                string? userID = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                                Task<ResultModel> contentTable = Task.FromResult(db.Insert<ResultModel>("SP_StockCrud", new DynamicParameters(new Dictionary<string, object?>
                                    {
                                        { "@processType", 2 },
                                        { "@rowType", 1 },
                                        { "@stockID", stockGet.StockID },
                                        { "@productID", FormRowGet(model, "productID") },
                                        { "@variant", colorID },
                                        { "@userID", userID },
                                        { "@type", "Giriş"},
                                        { "@unitCost", FormRowGet(model,"price")},
                                        { "@quantity", quantity },
                                        { "@note", FormRowGet(model,"message") },
                                    })));
                                return await Task.FromResult(new ResultModel() { resultType = 1, resultCaption = "Başarılı", resultMessage = "Güncelleme Başarılı" });
                            }
                            else return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Uyarı", resultMessage = "Kayıt Bulunamadı" });
                        }
                        else return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Hata", resultMessage = "Bir hata oluştu lütfen sistem yöneticinize başvurun." });
                    }
                    else 
                    {
                        int quantity = Convert.ToInt32(FormRowGet(model, "quantity"));
                        Task<ResultModel> mainTable = Task.FromResult(db.Insert<ResultModel>("SP_StockCrud", new DynamicParameters(new Dictionary<string, object?>
                    {
                        { "@processType", 1 },
                        { "@rowType", 1 },
                        { "@stockID", 0 },
                        { "@productID", FormRowGet(model, "productID") },
                        { "@variant", colorID },
                        { "@userID", "" },
                        { "@type", ""},
                        { "@unitCost", ""},
                        { "@quantity", quantity },
                        { "@note", "" },
                    })));
                        if (mainTable.IsCompletedSuccessfully)
                        {
                            if (userName != null)
                            {
                                stockID = MainTableMaxID("tblStock").Result;
                                string? userID = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                                Task<ResultModel> contentTable = Task.FromResult(db.Insert<ResultModel>("SP_StockCrud", new DynamicParameters(new Dictionary<string, object?>
                                {
                                    { "@processType", 2 },
                                    { "@rowType", 1 },
                                    { "@stockID", stockID },
                                    { "@productID", FormRowGet(model, "productID") },
                                    { "@variant", colorID },
                                    { "@userID", userID },
                                    { "@type", "Giriş"},
                                    { "@unitCost", FormRowGet(model,"price")},
                                    { "@quantity", quantity },
                                    { "@note", FormRowGet(model,"message") },
                                })));

                                return await Task.FromResult(new ResultModel() { resultType = 1, resultCaption = "Başarılı", resultMessage = "Güncelleme Başarılı" });
                            }
                            else return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Uyarı", resultMessage = "Kayıt Bulunamadı" });
                        }
                    }
                    return await Task.FromResult(new ResultModel() { resultType = 1, resultCaption = "Başarılı", resultMessage = "Güncelleme Başarılı" });

                }
                else 
                { 
                    return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Uyarı", resultMessage = "Kayıt Bulunamadı" });
                }
            }
            catch (Exception)
            {
                await Log(false, "ProductStockProcess");
                return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Uyarı", resultMessage = "Kayıt Bulunamadı" });
            }
        }
        #endregion

        #region Fiyat Güncelleme

        [Route("product-price-process")]
        [HttpPost]
        public async Task<ResultModel> ProductPriceProcess()
        {
            IFormCollection model = Request.Form;
            int productID = Convert.ToInt32(FormRowGet(model, "productID"));
            try
            {
                if (productID > 0)
                {
                    Task<ResultModel> priceTable = Task.FromResult(db.Insert<ResultModel>("SP_PriceCrud", new DynamicParameters(new Dictionary<string, object?>
                    {
                        { "@productID", productID },
                        { "@price", FormRowGet(model, "salePrice") }
                    })));
                    if (priceTable.IsCompletedSuccessfully)
                    {
                        return await Task.FromResult(new ResultModel() { resultType = 1, resultCaption = "Başarılı", resultMessage = "Güncelleme Başarılı" });
                    }
                    else return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Hata", resultMessage = "Bir hata oluştu lütfen sistem yöneticinize başvurun." });
                }
                else return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Uyarı", resultMessage = "Kayıt Bulunamadı" });
            }
            catch (Exception)
            {
                await Log(false, "ProductPriceProcess");
                return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Uyarı", resultMessage = "Kayıt Bulunamadı" });
            }
        }
        #endregion

        #region Durum ÖneÇıkan Güncelle - Sil
        [Route("product-process")]
        [HttpPost]
        public async Task<ResultModel> ProductProcess(int processType, int productID)
        {
            try
            {
                tblProductMain? productGet = db.QueryApp<tblProductMain>(string.Format("SELECT * FROM tblProductMain WHERE ProductID={0}", productID)).FirstOrDefault();
                if (productGet != null)
                {
                    if (processType == 1)
                    {
                        int isActive = productGet.IsActive == true ? 0 : 1;

                        Task<ResultModel?> processApp = Task.FromResult(db.QueryApp<ResultModel>(string.Format("UPDATE tblProductMain SET IsActive= {1} WHERE ProductID={0}", productGet.ProductID, isActive)).FirstOrDefault());

                        await Log(processApp.IsCompletedSuccessfully, "ProductProcess");
                        if (processApp.IsCompletedSuccessfully)
                            return await Task.FromResult(new ResultModel() { resultType = 1, resultCaption = "Başarılı", resultMessage = "Başarılı Güncelleme" });
                        else
                            return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Uyarı", resultMessage = "Kayıt Bulunamadı" });
                    }
                    else if (processType == 2)
                    {
                        Task<ResultModel?> processApp = Task.FromResult(db.QueryApp<ResultModel>(string.Format("UPDATE tblProductMain SET Deleted = 1 WHERE ProductID={0}", productGet.ProductID)).FirstOrDefault());

                        await Log(processApp.IsCompletedSuccessfully, "ProductProcess");
                        if (processApp.IsCompletedSuccessfully)
                            return await Task.FromResult(new ResultModel() { resultType = 1, resultCaption = "Başarılı", resultMessage = "Başarılı Güncelleme" });
                        else
                            return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Uyarı", resultMessage = "Kayıt Bulunamadı" });
                    }
                    else if (processType == 3)
                    {
                        int isFeatured = productGet.IsFeatured == true ? 0 : 1;
                        Task<ResultModel?> processApp = Task.FromResult(db.QueryApp<ResultModel>(string.Format("UPDATE tblProductMain SET IsFeatured= {1} WHERE ProductID={0}", productGet.ProductID, isFeatured)).FirstOrDefault());
                        await Log(processApp.IsCompletedSuccessfully, "ProductProcess");
                        if (processApp.IsCompletedSuccessfully)
                            return await Task.FromResult(new ResultModel() { resultType = 1, resultCaption = "Başarılı", resultMessage = "Başarılı Güncelleme" });
                        else
                            return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Uyarı", resultMessage = "Kayıt Bulunamadı" });
                    }
                    else return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Hata", resultMessage = "Bir hata oluştu lütfen sistem yöneticinize başvurun." });
                }
                else return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Hata", resultMessage = "Bir hata oluştu lütfen sistem yöneticinize başvurun." });
            }
            catch (Exception)
            {
                await Log(false, "ProductProcess");
                return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Uyarı", resultMessage = "Kayıt Bulunamadı" });
            }
        }
        #endregion

        #region Sıralama Güncelle
        [Route("product-arrangement-update")]
        public async Task<ResultModel> ProductArrangementUpdate(string arrangementBody)
        {
            try
            {
                if (!string.IsNullOrEmpty(arrangementBody))
                {
                    foreach (var item in arrangementBody.Split('_'))
                    {
                        if (item != "" && item != "_" && item != null)
                        {
                            int productID = Convert.ToInt32(item.Split(',').First());
                            int arrangement = Convert.ToInt32(item.Split(',').Last());

                            tblProductMain? productGet = db.QueryApp<tblProductMain>(string.Format("SELECT * FROM tblProductMain WHERE ProductID={0}", productID)).FirstOrDefault();
                            if (productGet != null)
                            {
                                Task<ResultModel?> result = Task.FromResult(db.QueryApp<ResultModel>(string.Format("UPDATE tblProductMain SET Arrangement={0} WHERE ProductID={1}", arrangement, productID)).FirstOrDefault());
                            }
                            else return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Uyarı", resultMessage = "Kayıt Bulunamadı" });
                        }
                    }
                    await Log(true, "ProductArrangementUpdate");
                    return await Task.FromResult(new ResultModel() { resultType = 1, resultCaption = "Başarılı", resultMessage = "Başarılı Güncelleme" });
                }
                else return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Uyarı", resultMessage = "Kayıt Bulunamadı" });
            }
            catch (Exception)
            {
                await Log(false, "ProductArrangementUpdate");
                return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Hata", resultMessage = "Bir hata oluştu lütfen sistem yöneticinize başvurun." });
            }
        }
        #endregion
        
        #region Görsel
        [Route("product-image-app")]
        public async Task<ResultModel> ProductImageApp(int productID)
        {
            try
            {
                var files = Request.Form.Files;
                if (files.Count() > 0)
                {
                    string productImg = FtpPost(Request.Form.Files[0], "Vossence/Products", "product");
                    if (productImg != "")
                    {
                        Task<ResultModel?> result = Task.FromResult(db.QueryApp<ResultModel>(string.Format("UPDATE tblProductContent SET ImgURL='{0}' WHERE ProductID={1}",productImg, productID)).FirstOrDefault());      
                        await Log(true, "ProductImageApp");
                        if (result.IsCompletedSuccessfully)
                            return await Task.FromResult(new ResultModel() { resultType = 1, resultCaption = "Başarılı", resultMessage = "Güncelleme Başarılı" });
                        else return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Hata", resultMessage = "Bir hata oluştu lütfen sistem yöneticinize başvurun." });
                    }
                }
                await Log(true, "ProductImageApp");
                return await Task.FromResult(new ResultModel() { resultType = 1, resultCaption = "Başarılı", resultMessage = "Başarılı Güncelleme" });
            }
            catch (Exception)
            {
                await Log(false, "ProductImageApp");
                return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Hata", resultMessage = "Bir hata oluştu lütfen sistem yöneticinize başvurun." });
            }
        }
        #endregion

        #endregion

        #region Varyantlar

        #region Liste

        [Route("variant-management")]
        public IActionResult VariantsManagement()
        {
            List<SP_Variants> colors = db.GetAll<SP_Variants>("SP_Variants", new DynamicParameters(new Dictionary<string, object>{ { "@variantType", 1 },{ "@variantID",-1} })).ToList();
            List<SP_Variants> tags = db.GetAll<SP_Variants>("SP_Variants", new DynamicParameters(new Dictionary<string, object> { { "@variantType", 2 }, { "@variantID", -1 } })).ToList();

            return View(Tuple.Create(colors, tags));
        }

        #endregion

        #region Oluştur - Güncelle

        [Route("variant-app")]
        public async Task<ResultModel> VariantApp()
        {
            try
            {
                IFormCollection model = Request.Form;
                
                int variantType = Convert.ToInt32(FormRowGet(model, "variantType"));
                int variantID = Convert.ToInt32(FormRowGet(model, "variantID"));

                if (variantType == 1)
                {
                    if (variantID == 0)
                    {
                        Task<ResultModel> colorTable = Task.FromResult(db.Insert<ResultModel>("SP_VariantCrud", new DynamicParameters(new Dictionary<string, object?>
                        {
                            { "@processType", 1 },
                            { "@rowType", 1 },
                            { "@variantID", variantID },
                            { "@variantName", FormRowGet(model, "variantName") }

                        })));

                        if (colorTable.IsCompletedSuccessfully)
                        {
                            return await Task.FromResult(new ResultModel() { resultType = 1, resultCaption = "Başarılı", resultMessage = "Güncelleme Başarılı" });
                        }
                        else return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Hata", resultMessage = "Bir hata oluştu lütfen sistem yöneticinize başvurun." });
                    } else if (variantID > 0) 
                    {
                        Task<ResultModel> colorTable = Task.FromResult(db.Insert<ResultModel>("SP_VariantCrud", new DynamicParameters(new Dictionary<string, object?>
                        {
                            { "@processType", 1 },
                            { "@rowType", 2 },
                            { "@variantID", variantID },
                            { "@variantName", FormRowGet(model, "variantName") }

                        })));

                        if (colorTable.IsCompletedSuccessfully)
                        {
                            return await Task.FromResult(new ResultModel() { resultType = 1, resultCaption = "Başarılı", resultMessage = "Güncelleme Başarılı" });
                        }
                        else return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Hata", resultMessage = "Bir hata oluştu lütfen sistem yöneticinize başvurun." });
                    }
                    else return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Uyarı", resultMessage = "Kayıt Bulunamadı." });
                }
                else if (variantType == 2)
                {
                    if (variantID == 0)
                    {
                        Task<ResultModel> tagTable = Task.FromResult(db.Insert<ResultModel>("SP_VariantCrud", new DynamicParameters(new Dictionary<string, object?>
                    {
                        { "@processType", 2 },
                        { "@rowType", 1 },
                        { "@variantID", variantID },
                        { "@variantName", FormRowGet(model, "variantName") }
                    })));

                        if (tagTable.IsCompletedSuccessfully)
                        {
                            return await Task.FromResult(new ResultModel() { resultType = 1, resultCaption = "Başarılı", resultMessage = "Güncelleme Başarılı" });
                        }
                        else return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Hata", resultMessage = "Bir hata oluştu lütfen sistem yöneticinize başvurun." });
                    }
                    else if (variantID > 0) 
                    {
                        Task<ResultModel> tagTable = Task.FromResult(db.Insert<ResultModel>("SP_VariantCrud", new DynamicParameters(new Dictionary<string, object?>
                    {
                        { "@processType", 2 },
                        { "@rowType", 2 },
                        { "@variantID", variantID },
                        { "@variantName", FormRowGet(model, "variantName") }
                    })));

                        if (tagTable.IsCompletedSuccessfully)
                        {
                            return await Task.FromResult(new ResultModel() { resultType = 1, resultCaption = "Başarılı", resultMessage = "Güncelleme Başarılı" });
                        }
                        else return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Hata", resultMessage = "Bir hata oluştu lütfen sistem yöneticinize başvurun." });
                    }
                    else return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Uyarı", resultMessage = "Kayıt Bulunamadı" });

                }
                else return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Uyarı", resultMessage = "Kayıt Bulunamadı" });
            }
            catch (Exception)
            {
                await Log(false, "VariantApp");
                return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Hata", resultMessage = "Bir hata oluştu lütfen sistem yöneticinize başvurun." });
            }
        }

        #endregion

        #region Durum ÖneÇıkan Güncelle - Sil

        [Route("variant-process")]
        [HttpPost]
        public async Task<ResultModel> VariantProcess(int processType, int variantID, int variantType)
        {
            try
            {
                if (variantType == 1)
                {
                    tblColors? colorGet = db.QueryApp<tblColors>(string.Format("SELECT * FROM tblColors WHERE ColorID={0}", variantID)).FirstOrDefault();

                    if (processType == 1)
                    {   
                        if (colorGet != null) 
                        {
                            int isActive = colorGet.IsActive == true ? 0 : 1;

                            Task<ResultModel?> processApp = Task.FromResult(db.QueryApp<ResultModel>(string.Format("UPDATE tblColors SET IsActive= {1} WHERE ColorID={0}", colorGet.ColorID, isActive)).FirstOrDefault());

                            await Log(processApp.IsCompletedSuccessfully, "VariantProcess");
                            if (processApp.IsCompletedSuccessfully)
                                return await Task.FromResult(new ResultModel() { resultType = 1, resultCaption = "Başarılı", resultMessage = "Başarılı Güncelleme" });
                            else
                                return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Uyarı", resultMessage = "Kayıt Bulunamadı" });
                        }
                        else return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Uyarı", resultMessage = "Kayıt Bulunamadı" });
                    }
                    else if (processType == 2)
                    {
                        Task<ResultModel?> processApp = Task.FromResult(db.QueryApp<ResultModel>(string.Format("UPDATE tblColors SET Deleted = 1 WHERE ColorID={0}", colorGet.ColorID)).FirstOrDefault());

                        await Log(processApp.IsCompletedSuccessfully, "VariantProcess");
                        if (processApp.IsCompletedSuccessfully)
                            return await Task.FromResult(new ResultModel() { resultType = 1, resultCaption = "Başarılı", resultMessage = "Başarılı Güncelleme" });
                        else
                            return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Uyarı", resultMessage = "Kayıt Bulunamadı" });
                    }
                    else return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Uyarı", resultMessage = "Kayıt Bulunamadı" });
                }
                else if (variantType == 2) 
                {
                    tblTags? tagGet = db.QueryApp<tblTags>(string.Format("SELECT * FROM tblTags WHERE TagID = {0}",variantID)).FirstOrDefault();
                    if (processType == 1)
                    {
                        if (tagGet != null)
                        {
                            int isActive = tagGet.IsActive == true ? 0 : 1;

                            Task<ResultModel?> processApp = Task.FromResult(db.QueryApp<ResultModel>(string.Format("UPDATE tblTags SET IsActive= {1} WHERE TagID={0}", tagGet.TagID, isActive)).FirstOrDefault());

                            await Log(processApp.IsCompletedSuccessfully, "VariantProcess");
                            if (processApp.IsCompletedSuccessfully)
                                return await Task.FromResult(new ResultModel() { resultType = 1, resultCaption = "Başarılı", resultMessage = "Başarılı Güncelleme" });
                            else
                                return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Uyarı", resultMessage = "Kayıt Bulunamadı" });
                        }
                        else return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Uyarı", resultMessage = "Kayıt Bulunamadı" });
                    }
                    else if (processType == 2)
                    {
                        Task<ResultModel?> processApp = Task.FromResult(db.QueryApp<ResultModel>(string.Format("UPDATE tblTags SET Deleted = 1 WHERE TagID={0}", tagGet.TagID)).FirstOrDefault());

                        await Log(processApp.IsCompletedSuccessfully, "VariantProcess");
                        if (processApp.IsCompletedSuccessfully)
                            return await Task.FromResult(new ResultModel() { resultType = 1, resultCaption = "Başarılı", resultMessage = "Başarılı Güncelleme" });
                        else
                            return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Uyarı", resultMessage = "Kayıt Bulunamadı" });
                    }
                    return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Uyarı", resultMessage = "Kayıt Bulunamadı" });

                }
                else return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Uyarı", resultMessage = "Kayıt Bulunamadı" });
            }
            catch (Exception)
            {
                await Log(false, "VariantProcess");
                return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Uyarı", resultMessage = "Kayıt Bulunamadı" });
            }
        }

        #endregion

        #endregion

        #region Manuel Satış Ekranı

        [HttpGet]
        [Route("manual-sale")]
        public IActionResult ManualSales()
        {
            
            return View();
        }

        #endregion
    }





}
