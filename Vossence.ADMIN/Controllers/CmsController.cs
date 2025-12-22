using Vossence.DATA.Helper;
using Vossence.DATA.ORM;
using Vossence.DATA.Procedure;
using Vossence.DATA.Table;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using static Vossence.DATA.Validation.Cls;

namespace Vossence.ADMIN.Controllers
{
    public class CmsController : SharedController
    { 
        private readonly IDapper db;
        private readonly IConfiguration? configuration;

        #region Ctor
        public CmsController(IDapper dapper, IConfiguration? configuration, ILogger<SharedController> logger) : base(dapper, configuration, logger)
        {
            this.db = dapper;
            this.configuration = configuration;
        }
        #endregion

        #region İçerik Yönetimi

        #region Liste
        [Route("item-management")]
        public IActionResult ItemManagement(int cmsType, int cmsID = 0, int trControl = -1)
        {
            List<SP_CmsList> list = db.GetAll<SP_CmsList>("SP_CmsList", new DynamicParameters(new Dictionary<string, object>
            {
                { "@langID", langID },
                { "@cmsType", cmsType },
                { "@cmsID", cmsID },
                { "@trControl", trControl }
            }));
            List<tblCmsMain>? cmsMains = db.QueryApp<tblCmsMain>(string.Format("SELECT * FROM tblCmsMain")).ToList();

            return View(Tuple.Create(list, cmsMains));
        }
        #endregion

        #region Oluştur - Güncelle
        [Route("item-app")]
        [HttpPost]
        public async Task<ResultModel> ItemApp(LangItem[] list)
        {
            try
            {
                IFormCollection model = Request.Form;

                int cmsID = Convert.ToInt32(FormRowGet(model, "cmsID"));
                if (cmsID == 0)
                {
                    Task<ResultModel> mainTable = Task.FromResult(db.Insert<ResultModel>("SP_CmsCrud", new DynamicParameters(new Dictionary<string, object?>
                    {
                        { "@processType", 1 },
                        { "@rowType", 1 },
                        { "@cmsID", 0 },
                        { "@langID", 0 },
                        { "@cmsType", FormRowGet(model, "cmsType") },
                        { "@cmsSub", FormRowGet(model, "cmsSub") },
                        { "@cmsName", FormRowGet(model, "cmsName") },
                        { "@externalURL", FormRowGet(model, "externalURL") },
                        { "@cmsSubName", "" },
                        { "@cmsContent", "" },
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
                                    string groupID = "Cms_" + cmsID;
                                    string url = AppFunc.TextLinkReturning(item.url);
                                    Task<ResultModel> contentTable = Task.FromResult(db.Insert<ResultModel>("SP_CmsCrud", new DynamicParameters(new Dictionary<string, object?>
                                    {
                                        { "@processType", 2 },
                                        { "@rowType", 1 },
                                        { "@cmsID", MainTableMaxID("tblCmsMain").Result },
                                        { "@langID", item.langID },
                                        { "@cmsType", FormRowGet(model, "cmsType") },
                                        { "@cmsSub", FormRowGet(model, "cmsSub") },
                                        { "@cmsName", FormRowGet(model, "cmsName") },
                                        { "@externalURL", FormRowGet(model, "externalURL") },
                                        { "@cmsSubName", item.caption },
                                        { "@cmsContent", item.content },
                                        { "@title", item.title },
                                        { "@description", item.description },
                                        { "@url", item.url },
                                    })));

                                    LinkApp(1, url, item.langID, "Page", "CmsDetail", groupID, FormRowGet(model, "cmsName"));
                                }
                            }
                            return await Task.FromResult(new ResultModel() { resultType = 1, resultCaption = "Başarılı", resultMessage = "Başarılı Güncelleme" });
                        }
                        else return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Uyarı", resultMessage = "Kayıt Bulunamadı" });
                    }
                    else return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Hata", resultMessage = "Bir hata oluştu lütfen sistem yöneticinize başvurun" });
                }
                else if (cmsID > 0)
                {
                    tblCmsMain? cmsGet = db.QueryApp<tblCmsMain>(string.Format("SELECT * FROM tblCmsMain WHERE CmsID={0}", cmsID)).FirstOrDefault();
                    if (cmsGet != null)
                    {
                        Task<ResultModel> mainTable = Task.FromResult(db.Insert<ResultModel>("SP_CmsCrud", new DynamicParameters(new Dictionary<string, object?>
                        {
                            { "@processType", 1 },
                            { "@rowType", 2 },
                            { "@cmsID", cmsGet.CmsID },
                            { "@langID", 0 },
                            { "@cmsType", FormRowGet(model, "cmsType") },
                            { "@cmsSub", FormRowGet(model, "cmsSub") },
                            { "@cmsName", FormRowGet(model, "cmsName") },
                            { "@externalURL", FormRowGet(model, "externalURL") },
                            { "@cmsSubName", "" },
                            { "@cmsContent", "" },
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

                                    tblCmsContent? cnt = db.QueryApp<tblCmsContent>(string.Format("SELECT * FROM tblCmsContent WHERE CmsID={0} AND LangID={1}", cmsGet.CmsID, item.langID)).FirstOrDefault();
                                    if (cnt != null)
                                    {
                                        string groupID = "Cms_" + cmsID;
                                        string url = AppFunc.TextLinkReturning(item.url);
                                        LinkApp(2, "", item.langID, "Page", "CmsDetail", groupID, FormRowGet(model, "cmsName"));

                                        if (!string.IsNullOrEmpty(item.caption))
                                        {
                                            Task<ResultModel> contentTable = Task.FromResult(db.Insert<ResultModel>("SP_CmsCrud", new DynamicParameters(new Dictionary<string, object?>
                                            {
                                                { "@processType", 2 },
                                                { "@rowType", 2 },
                                                { "@cmsID", cnt.CmsID },
                                                { "@langID", item.langID },
                                                { "@cmsType", FormRowGet(model, "cmsType") },
                                                { "@cmsSub", FormRowGet(model, "cmsSub") },
                                                { "@cmsName", FormRowGet(model, "cmsName") },
                                                { "@externalURL", FormRowGet(model, "externalURL") },
                                                { "@cmsSubName", item.caption },
                                                { "@cmsContent", item.content },
                                                { "@title", item.title },
                                                { "@description", item.description },
                                                { "@url", item.url },
                                            })));
                                            LinkApp(1, url, item.langID, "Page", "CmsDetail", groupID, FormRowGet(model, "cmsName"));
                                        }
                                        else
                                        {
                                            Task<ResultModel?> deleteTable = Task.FromResult(db.QueryApp<ResultModel>(string.Format("DELETE FROM tblCmsContent WHERE CmsID={0} AND LangID={1}", cmsGet.CmsID, item.langID)).FirstOrDefault());
                                        }
                                    }
                                    else
                                    {
                                        if (!string.IsNullOrEmpty(item.caption))
                                        {
                                            string groupID = "Cms_" + cmsID;
                                            string url = AppFunc.TextLinkReturning(item.url);
                                            Task<ResultModel> contentTable = Task.FromResult(db.Insert<ResultModel>("SP_CmsCrud", new DynamicParameters(new Dictionary<string, object?>
                                            {
                                                { "@processType", 2 },
                                                { "@rowType", 1 },
                                                { "@cmsID", cmsID },
                                                { "@langID", item.langID },
                                                { "@cmsType", FormRowGet(model, "cmsType") },
                                                { "@cmsSub", FormRowGet(model, "cmsSub") },
                                                { "@cmsName", FormRowGet(model, "cmsName") },
                                                { "@externalURL", FormRowGet(model, "externalURL") },
                                                { "@cmsSubName", item.caption },
                                                { "@cmsContent", item.content },
                                                { "@title", item.title },
                                                { "@description", item.description },
                                                { "@url", item.url },
                                            })));

                                            LinkApp(1, url, item.langID, "Page", "CmsDetail", groupID, FormRowGet(model, "cmsName"));
                                        }
                                    }
                                }
                                return await Task.FromResult(new ResultModel() { resultType = 1, resultCaption = "Başarılı", resultMessage = "Başarılı Güncelleme" });
                            }
                            else return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Uyarı", resultMessage = "Kayıt Bulunamadı" });
                        }
                        else return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Hata", resultMessage = "Bir hata oluştu lütfen sistem yöneticinize başvurun" });
                    }
                    else return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Uyarı", resultMessage = "Kayıt Bulunamadı" });
                }
                else return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Uyarı", resultMessage = "Kayıt Bulunamadı" });
            }
            catch (Exception)
            {
                await Log(false, "ItemApp");
                return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Hata", resultMessage = "Bir hata oluştu lütfen sistem yöneticinize başvurun" });
            }
        }
        #endregion

        #region Durum Güncelle - Sil
        [Route("item-process")]
        [HttpPost]
        public async Task<ResultModel> ItemProcess(int processType, int cmsID)
        {
            try
            {
                tblCmsMain? cmsGet = db.QueryApp<tblCmsMain>(string.Format("SELECT * FROM tblCmsMain WHERE CmsID={0}", cmsID)).FirstOrDefault();
                if (cmsGet != null)
                {
                    if (processType == 1)
                    {
                        int isActive = cmsGet.IsActive == true ? 0 : 1;

                        Task<ResultModel?> processApp = Task.FromResult(db.QueryApp<ResultModel>(string.Format("UPDATE tblCmsMain SET IsActive= {1} WHERE CmsID={0}", cmsGet.CmsID, isActive)).FirstOrDefault());

                        await Log(processApp.IsCompletedSuccessfully, "ItemProcess");
                        if (processApp.IsCompletedSuccessfully)
                            return await Task.FromResult(new ResultModel() { resultType = 1, resultCaption = "Başarılı", resultMessage = "Güncelleme Başarılı" });
                        else
                            return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Uyarı", resultMessage = "Kayıt Bulunamadı" });
                    }
                    else if (processType == 2)
                    {
                        Task<ResultModel?> processApp = Task.FromResult(db.QueryApp<ResultModel>(string.Format("UPDATE tblCmsMain SET Deleted = 1 WHERE CmsID={0}", cmsGet.CmsID)).FirstOrDefault());

                        await Log(processApp.IsCompletedSuccessfully, "ItemProcess");
                        if (processApp.IsCompletedSuccessfully)
                            return await Task.FromResult(new ResultModel() { resultType = 1, resultCaption = "Başarılı", resultMessage = "Güncelleme Başarılı" });
                        else
                            return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Uyarı", resultMessage = "Kayıt Bulunamadı" });
                    }
                    else return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Hata", resultMessage = "Bir hata oluştu lütfen sistem yöneticinize başvurun" });
                }
                else return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Hata", resultMessage = "Bir hata oluştu lütfen sistem yöneticinize başvurun" });
            }
            catch (Exception)
            {
                await Log(false, "ItemProcess");
                return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Uyarı", resultMessage = "Kayıt Bulunamadı" });
            }
        }
        #endregion

        #region Sıralama Güncelle
        [Route("cms-arrangement-update")]
        public async Task<ResultModel> CmsArrangementUpdate(string arrangementBody)
        {
            try
            {
                if (!string.IsNullOrEmpty(arrangementBody))
                {
                    foreach (var item in arrangementBody.Split('_'))
                    {
                        if (item != "" && item != "_" && item != null)
                        {
                            int cmsID = Convert.ToInt32(item.Split(',').First());
                            int arrangement = Convert.ToInt32(item.Split(',').Last());

                            tblCmsMain? cmsGet = db.QueryApp<tblCmsMain>(string.Format("SELECT * FROM tblCmsMain WHERE CmsID={0}", cmsID)).FirstOrDefault();
                            if (cmsGet != null)
                            {
                                Task<ResultModel?> result = Task.FromResult(db.QueryApp<ResultModel>(string.Format("UPDATE tblCmsMain SET Arrangement={0} WHERE CmsID={1}", arrangement, cmsID)).FirstOrDefault());
                            }
                            else return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Hata", resultMessage = "Bir hata oluştu lütfen sistem yöneticinize başvurun" });
                        }
                    }
                    await Log(true, "CmsArrangementUpdate");
                    return await Task.FromResult(new ResultModel() { resultType = 1, resultCaption = "Başarılı", resultMessage = "Başarılı Güncelleme" });
                }
                else return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Hata", resultMessage = "Bir hata oluştu lütfen sistem yöneticinize başvurun" });
            }
            catch (Exception)
            {
                await Log(false, "CmsArrangementUpdate");
                return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Uyarı", resultMessage = "Kayıt Bulunamadı" });
            }
        }
        #endregion

        #region Galeri

        #region Liste
        [Route("item-gallery")]
        public IActionResult ItemGallery(int cmsID)
        {
            tblCmsMain? cmsGet = db.QueryApp<tblCmsMain>(string.Format("SELECT * FROM tblCmsMain WHERE CmsID={0}", cmsID)).FirstOrDefault();
            if (cmsGet != null)
            {
                List<tblCmsGallery>? files = db.QueryApp<tblCmsGallery>(string.Format("SELECT * FROM tblCmsGallery WHERE CmsID={0}", cmsID)).ToList();
                return View(Tuple.Create(cmsGet, files));
            }
            return Redirect("/404");
        }
        #endregion

        #region Yükle
        [Route("image-app")]
        public async Task<ResultModel> ImageApp(int cmsID)
        {
            try
            {
                var files = Request.Form.Files;
                string? cdnURL = CdnUrl();
                for (int i = 0; i < files.Count; i++)
                {
                    string postResult = FtpPost(Request.Form.Files[i], "Transfer/CmsImages", "cms");
                    if (postResult != "")
                    {
                        Task<ResultModel?> result = Task.FromResult(db.QueryApp<ResultModel>(string.Format("INSERT INTO tblCmsGallery (CmsID, Arrangement, FileURL) VALUES ({0}, 0, '{1}')", cmsID, "Transfer/CmsImages/" + postResult)).FirstOrDefault());
                    }
                }

                await Log(true, "ImageApp");
                return await Task.FromResult(new ResultModel() { resultType = 1, resultCaption = "Başarılı", resultMessage = "Başarılı Güncelleme" });
            }
            catch (Exception)
            {
                await Log(false, "ImageApp");
                return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Uyarı", resultMessage = "Kayıt Bulunamadı" });
            }
        }
        #endregion

        #region İşlemler
        [Route("cms-gallery-operation")]
        public async Task<ResultModel> CmsGalleryOperation(string[] inputList, string[] deletedList)
        {
            try
            {
                int inputItemID = 0, h = 0;
                for (int i = 0; i < inputList.Count() / 2; i++)
                {
                    inputItemID = Convert.ToInt32(inputList[h]);

                    tblCmsGallery? fileGet = db.QueryApp<tblCmsGallery>(string.Format("SELECT * FROM tblCmsGallery WHERE FileID={0}", inputItemID)).FirstOrDefault();
                    if (fileGet != null)
                    {
                        h = h + 1;
                        Task<ResultModel?> result = Task.FromResult(db.QueryApp<ResultModel>(string.Format("UPDATE tblCmsGallery SET Arrangement={0} WHERE FileID={1}", inputList[h], inputItemID)).FirstOrDefault());
                        h = h + 1;
                    }
                    else h = h + 2;
                }

                if (deletedList != null)
                {
                    for (int i = 0; i < deletedList.Count(); i++)
                    {
                        tblCmsGallery? fileGet = db.QueryApp<tblCmsGallery>(string.Format("SELECT * FROM tblCmsGallery WHERE FileID={0}", deletedList[i])).FirstOrDefault();
                        if (fileGet != null)
                        {
                            Task<ResultModel?> result = Task.FromResult(db.QueryApp<ResultModel>(string.Format("DELETE FROM tblCmsGallery WHERE FileID={0}", fileGet.FileID)).FirstOrDefault());
                        }
                    }
                }

                return await Task.FromResult(new ResultModel() { resultType = 1, resultCaption = "Başarılı", resultMessage = "Başarılı Güncelleme" });
            }
            catch (Exception)
            {
                await Log(false, "CmsGalleryOperation");
                return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Uyarı", resultMessage = "Kayıt Bulunamadı" });
            }
        }
        #endregion

        #endregion
        #endregion

        #region SSS

        #region Kategori

        #region Liste
        [Route("sss-categories")]
        public IActionResult SSSCategories()
        {
            List<SP_SSSCategories> list = db.GetAll<SP_SSSCategories>("SP_SSSCategories", new DynamicParameters(new Dictionary<string, object> { { "@langID", langID } }));
            return View(Tuple.Create(list, 0));
        }
        #endregion

        #region Oluştur - Güncelle
        [Route("sss-category-app")]
        [HttpPost]
        public async Task<ResultModel> SSSCategoryApp(LangItem[] list)
        {
            try
            {
                IFormCollection model = Request.Form;

                int sssCategoryID = Convert.ToInt32(FormRowGet(model, "sssCategoryID"));
                if (sssCategoryID == 0)
                {
                    Task<ResultModel> mainTable = Task.FromResult(db.Insert<ResultModel>("SP_SSSCategoryCrud", new DynamicParameters(new Dictionary<string, object?>
                    {
                        { "@processType", 1 },
                        { "@rowType", 1 },
                        { "@sssCategoryID", 0 },
                        { "@langID", 0 },
                        { "@categoryName", FormRowGet(model, "sssCategoryName") },
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
                                if (!string.IsNullOrEmpty(item.caption))
                                {
                                    sssCategoryID = MainTableMaxID("tblSSSCategoryMain").Result;
                                    string groupID = "SSSCategory_" + sssCategoryID;
                                    string url = AppFunc.TextLinkReturning(item.url);

                                    Task<ResultModel> contentTable = Task.FromResult(db.Insert<ResultModel>("SP_SSSCategoryCrud", new DynamicParameters(new Dictionary<string, object?>
                                    {
                                        { "@processType", 2 },
                                        { "@rowType", 1 },
                                        { "@sssCategoryID", sssCategoryID },
                                        { "@langID", item.langID },
                                        { "@categoryName", FormRowGet(model, "categoryName") },
                                        { "@categorySubName", item.caption },
                                        { "@title", item.title },
                                        { "@description", item.description },
                                        { "@url", url },
                                    })));

                                    LinkApp(1, url, item.langID, "Page", "SSSCategory", groupID, FormRowGet(model, "sssCategoryName"));
                                }
                            }
                            return await Task.FromResult(new ResultModel() { resultType = 1, resultCaption = "Başarılı", resultMessage = "Güncelleme Başarılı" });
                        }
                        else return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Uyarı", resultMessage = "Kayıt Bulunamadı" });
                    }
                    else return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Hata", resultMessage = "Bir hata oluştu lütfen sistem yöneticinize başvurun" });
                }
                else if (sssCategoryID > 0)
                {
                    tblSSSCategoryMain? sssCategoryGet = db.QueryApp<tblSSSCategoryMain>(string.Format("SELECT * FROM tblSSSCategoryMain WHERE SSSCategoryID={0}", sssCategoryID)).FirstOrDefault();
                    if (sssCategoryGet != null)
                    {
                        Task<ResultModel> mainTable = Task.FromResult(db.Insert<ResultModel>("SP_SSSCategoryCrud", new DynamicParameters(new Dictionary<string, object?>
                        {
                            { "@processType", 1 },
                            { "@rowType", 2 },
                            { "@sssCategoryID", sssCategoryGet.SSSCategoryID },
                            { "@langID", 0 },
                            { "@categoryName", FormRowGet(model, "sssCategoryName") },
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
                                    tblSSSCategoryContent? cnt = db.QueryApp<tblSSSCategoryContent>(string.Format("SELECT * FROM tblSSSCategoryContent WHERE SSSCategoryID={0} AND LangID={1}", sssCategoryGet.SSSCategoryID, item.langID)).FirstOrDefault();
                                    if (cnt != null)
                                    {
                                        string groupID = "SSSCategory_" + sssCategoryGet.SSSCategoryID;
                                        string url = AppFunc.TextLinkReturning(item.url);
                                        LinkApp(2, "", item.langID, "Page", "SSSCategory", groupID, FormRowGet(model, "sssCategoryName"));

                                        if (!string.IsNullOrEmpty(item.caption))
                                        {
                                            Task<ResultModel> contentTable = Task.FromResult(db.Insert<ResultModel>("SP_SSSCategoryCrud", new DynamicParameters(new Dictionary<string, object?>
                                            {
                                                { "@processType", 2 },
                                                { "@rowType", 2 },
                                                { "@sssCategoryID", cnt.SSSCategoryID },
                                                { "@langID", item.langID },
                                                { "@categoryName", FormRowGet(model, "categoryName") },
                                                { "@categorySubName", item.caption },
                                                { "@title", item.title },
                                                { "@description", item.description },
                                                { "@url", url },
                                            })));

                                            LinkApp(1, url, item.langID, "Page", "SSSCategory", groupID, FormRowGet(model, "sssCategoryName"));
                                        }
                                        else
                                        {
                                            Task<ResultModel?> deleteTable = Task.FromResult(db.QueryApp<ResultModel>(string.Format("DELETE FROM tblSSSCategoryContent WHERE SSSCategoryID={0} AND LangID={1}", sssCategoryGet.SSSCategoryID, item.langID)).FirstOrDefault());
                                        }
                                    }
                                    else
                                    {
                                        if (!string.IsNullOrEmpty(item.caption))
                                        {
                                            string groupID = "SSSCategory_" + sssCategoryID;
                                            string url = AppFunc.TextLinkReturning(item.url);

                                            Task<ResultModel> contentTable = Task.FromResult(db.Insert<ResultModel>("SP_SSSCategoryCrud", new DynamicParameters(new Dictionary<string, object?>
                                            {
                                                { "@processType", 2 },
                                                { "@rowType", 1 },
                                                { "@sssCategoryID", sssCategoryID },
                                                { "@langID", item.langID },
                                                { "@categoryName", FormRowGet(model, "categoryName") },
                                                { "@categorySubName", item.caption },
                                                { "@title", item.title },
                                                { "@description", item.description },
                                                { "@url", url },
                                            })));

                                            LinkApp(1, url, item.langID, "Page", "SSSCategory", groupID, FormRowGet(model, "sssCategoryName"));
                                        }
                                    }
                                }
                                return await Task.FromResult(new ResultModel() { resultType = 1, resultCaption = "Başarılı", resultMessage = "Güncelleme Başarılı" });
                            }
                            else return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Uyarı", resultMessage = "Kayıt Bulunamadı" });
                        }
                        else return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Hata", resultMessage = "Bir hata oluştu lütfen sistem yöneticinize başvurun" });
                    }
                    else return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Hata", resultMessage = "Kayıt Bulunamadı" });
                }
                else return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Hata", resultMessage = "Kayıt Bulunamadı" });
            }
            catch (Exception)
            {
                await Log(false, "SSSCategoryApp");
                return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Hata", resultMessage = "Bir hata oluştu lütfen sistem yöneticinize başvurun" });
            }
        }
        #endregion

        #region Durum Güncelle - Sil
        [Route("sss-category-process")]
        [HttpPost]
        public async Task<ResultModel> SSSCategoryProcess(int processType, int sssCategoryID)
        {
            try
            {
                tblSSSCategoryMain? sssCategoryGet = db.QueryApp<tblSSSCategoryMain>(string.Format("SELECT * FROM tblSSSCategoryMain WHERE SSSCategoryID={0}", sssCategoryID)).FirstOrDefault();
                if (sssCategoryGet != null)
                {
                    if (processType == 1)
                    {
                        int isActive = sssCategoryGet.IsActive == true ? 0 : 1;

                        Task<ResultModel?> processApp = Task.FromResult(db.QueryApp<ResultModel>(string.Format("UPDATE tblSSSCategoryMain SET IsActive= {1} WHERE SSSCategoryID={0}", sssCategoryGet.SSSCategoryID, isActive)).FirstOrDefault());

                        await Log(processApp.IsCompletedSuccessfully, "SSSCategoryProcess");
                        if (processApp.IsCompletedSuccessfully)
                            return await Task.FromResult(new ResultModel() { resultType = 1, resultCaption = "Başarılı", resultMessage = "Güncelleme Başarılı" });
                        else
                            return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Hata", resultMessage = "Bir hata oluştu lütfen sistem yöneticinize başvurun" });
                    }
                    else if (processType == 2)
                    {
                        Task<ResultModel?> processApp = Task.FromResult(db.QueryApp<ResultModel>(string.Format("UPDATE tblSSSCategoryMain SET Deleted = 1 WHERE SSSCategoryID={0}", sssCategoryGet.SSSCategoryID)).FirstOrDefault());

                        await Log(processApp.IsCompletedSuccessfully, "SSSCategoryProcess");
                        if (processApp.IsCompletedSuccessfully)
                            return await Task.FromResult(new ResultModel() { resultType = 1, resultCaption = "Başarılı", resultMessage = "Güncelleme Başarılı" });
                        else
                            return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Hata", resultMessage = "Bir hata oluştu lütfen sistem yöneticinize başvurun" });
                    }
                    else return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Hata", resultMessage = "Kayıt Bulunamadı" });
                }
                else return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Hata", resultMessage = "Kayıt Bulunamadı" });
            }
            catch (Exception)
            {
                await Log(false, "SSSCategoryProcess");
                return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Hata", resultMessage = "Bir hata oluştu lütfen sistem yöneticinize başvurun" });
            }
        }
        #endregion

        #region Sıralama Güncelle
        [Route("sss-category-arrangement-update")]
        public async Task<ResultModel> SSSCategoryArrangementUpdate(string arrangementBody)
        {
            try
            {
                if (!string.IsNullOrEmpty(arrangementBody))
                {
                    foreach (var item in arrangementBody.Split('_'))
                    {
                        if (item != "" && item != "_" && item != null)
                        {
                            int sssCategoryID = Convert.ToInt32(item.Split(',').First());
                            int arrangement = Convert.ToInt32(item.Split(',').Last());

                            tblSSSCategoryMain? sssCategoryGet = db.QueryApp<tblSSSCategoryMain>(string.Format("SELECT * FROM tblSSSCategoryMain WHERE SSSCategoryID={0}", sssCategoryID)).FirstOrDefault();
                            if (sssCategoryGet != null)
                            {
                                Task<ResultModel?> result = Task.FromResult(db.QueryApp<ResultModel>(string.Format("UPDATE tblSSSCategoryMain SET Arrangement={0} WHERE SSSCategoryID={1}", arrangement, sssCategoryID)).FirstOrDefault());
                            }
                            else return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Hata", resultMessage = "Kayıt Bulunamadı" });
                        }
                    }
                    await Log(true, "SSSCategoryArrangementUpdate");
                    return await Task.FromResult(new ResultModel() { resultType = 1, resultCaption = "Başarılı", resultMessage = "Güncelleme Başarılı" });
                }
                else return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Hata", resultMessage = "Kayıt Bulunamadı" });
            }
            catch (Exception)
            {
                await Log(false, "SSSCategoryArrangementUpdate");
                return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Hata", resultMessage = "Bir hata oluştu lütfen sistem yöneticinize başvurun" });
            }
        }
        #endregion

        #region Liste Görsel
        [Route("sss-category-image-app")]
        public async Task<ResultModel> SSSCategoriesListImageApp(int sssCategoryID)
        {
            try
            {
                var files = Request.Form.Files;
                if (files.Count() > 0)
                {
                    string postResult = FtpPost(Request.Form.Files[0], "Vossence/SSSCategories", "sss-categories");
                    if (postResult != "")
                    {
                        Task<ResultModel?> result = Task.FromResult(db.QueryApp<ResultModel>(string.Format("UPDATE tblSSSCategoryMain SET ListImage='{0}' WHERE SSSCategoryID={1}", postResult, sssCategoryID)).FirstOrDefault());
                    }
                }

                await Log(true, "SSSCategoriesListImageApp");
                return await Task.FromResult(new ResultModel() { resultType = 1, resultCaption = "Başarılı", resultMessage = "Güncelleme Başarılı" });
            }
            catch (Exception)
            {
                await Log(false, "SSSCategoriesListImageApp");
                return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Hata", resultMessage = "Bir hata oluştu lütfen sistem yöneticinize başvurun" });
            }
        }
        #endregion

        #endregion

        #region Post

        #region Liste
        [Route("sss-posts")]
        public IActionResult SSSPosts()
        {
            List<SP_SSSPosts> list = db.GetAll<SP_SSSPosts>("SP_SSSPosts", new DynamicParameters(new Dictionary<string, object>
            {
                { "@langID", langID }
            }));
            List<SP_SSSCategories> categories = db.GetAll<SP_SSSCategories>("SP_SSSCategories", new DynamicParameters(new Dictionary<string, object>
            {
                { "@langID", langID }
            }));
            return View(Tuple.Create(list, categories));
        }
        #endregion

        #region Oluştur - Güncelle
        [Route("sss-post-app")]
        [HttpPost]
        public async Task<ResultModel> SSSPostApp(LangItem[] list)
        {
            try
            {
                IFormCollection model = Request.Form;

                int sssPostID = Convert.ToInt32(FormRowGet(model, "sssPostID"));
                if (sssPostID == 0)
                {
                    Task<ResultModel> mainTable = Task.FromResult(db.Insert<ResultModel>("SP_SSSPostCrud", new DynamicParameters(new Dictionary<string, object?>
                    {
                        { "@processType", 1 },
                        { "@rowType", 1 },
                        { "@sssPostID", 0 },
                        { "@categoryID", FormRowGet(model, "sssCategoryID") },
                        { "@langID", 0 },
                        { "@sssPostName", FormRowGet(model, "sssPostName") },
                        { "@subQuestion", "" },
                        { "@subAnswer", "" },
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
                                if (!string.IsNullOrEmpty(item.caption))
                                {
                                    sssPostID = MainTableMaxID("tblSSSPostMain").Result;
                                    string groupID = "SSSPost_" + sssPostID;
                                    string url = AppFunc.TextLinkReturning(item.url);

                                    Task<ResultModel> contentTable = Task.FromResult(db.Insert<ResultModel>("SP_SSSPostCrud", new DynamicParameters(new Dictionary<string, object?>
                                    {
                                        { "@processType", 2 },
                                        { "@rowType", 1 },
                                        { "@sssPostID", sssPostID },
                                        { "@categoryID", FormRowGet(model, "sssCategoryID") },
                                        { "@langID", item.langID },
                                        { "@sssPostName", FormRowGet(model, "sssPostName") },
                                        { "@subQuestion", item.caption },
                                        { "@subAnswer", item.content },
                                        { "@title", item.title },
                                        { "@description", item.description },
                                        { "@url", url },
                                    })));

                                    LinkApp(1, url, item.langID, "Page", "SSSPost", groupID, FormRowGet(model, "sssPostName"));
                                }
                            }
                            return await Task.FromResult(new ResultModel() { resultType = 1, resultCaption = "Başarılı", resultMessage = "Güncelleme Başarılı" });
                        }
                        else return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Hata", resultMessage = "Kayıt Bulunamadı" });
                    }
                    else return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Hata", resultMessage = "Bir hata oluştu lütfen sistem yöneticinize başvurun" });
                }
                else if (sssPostID > 0)
                {
                    tblSSSPostMain? sssPostGet = db.QueryApp<tblSSSPostMain>(string.Format("SELECT * FROM tblSSSPostMain WHERE SSSPostID={0}", sssPostID)).FirstOrDefault();
                    if (sssPostGet != null)
                    {
                        Task<ResultModel> mainTable = Task.FromResult(db.Insert<ResultModel>("SP_SSSPostCrud", new DynamicParameters(new Dictionary<string, object?>
                        {
                            { "@processType", 1 },
                            { "@rowType", 2 },
                            { "@sssPostID", sssPostGet.SSSPostID },
                            { "@categoryID", FormRowGet(model, "sssCategoryID") },
                            { "@langID", 0 },
                            { "@sssPostName", FormRowGet(model, "sssPostName") },
                            { "@subQuestion", "" },
                            { "@subAnswer", "" },
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
                                    tblSSSPostContent? cnt = db.QueryApp<tblSSSPostContent>(string.Format("SELECT * FROM tblSSSPostContent WHERE SSSPostID={0} AND LangID={1}", sssPostGet.SSSPostID, item.langID)).FirstOrDefault();
                                    if (cnt != null)
                                    {
                                        string groupID = "SSSPost_" + sssPostGet.SSSPostID;
                                        string url = AppFunc.TextLinkReturning(item.url);
                                        LinkApp(2, "", item.langID, "Page", "SSSPost", groupID, FormRowGet(model, "sssPostName"));

                                        if (!string.IsNullOrEmpty(item.caption))
                                        {
                                            Task<ResultModel> contentTable = Task.FromResult(db.Insert<ResultModel>("SP_SSSPostCrud", new DynamicParameters(new Dictionary<string, object?>
                                            {
                                                { "@processType", 2 },
                                                { "@rowType", 2 },
                                                { "@sssPostID", cnt.SSSPostID },
                                                { "@categoryID", FormRowGet(model, "sssCategoryID") },
                                                { "@langID", item.langID },
                                                { "@sssPostName", FormRowGet(model, "sssPostName") },
                                                { "@subQuestion", item.caption },
                                                { "@subAnswer", item.content },
                                                { "@title", item.title },
                                                { "@description", item.description },
                                                { "@url", url },
                                            })));

                                            LinkApp(1, url, item.langID, "Page", "SSSPost", groupID, FormRowGet(model, "sssPostName"));
                                        }
                                        else
                                        {
                                            Task<ResultModel?> deleteTable = Task.FromResult(db.QueryApp<ResultModel>(string.Format("DELETE FROM tblSSSPostContent WHERE SSSPostID={0} AND LangID={1}", sssPostGet.SSSPostID, item.langID)).FirstOrDefault());
                                        }
                                    }
                                    else
                                    {
                                        if (!string.IsNullOrEmpty(item.caption))
                                        {
                                            string groupID = "SSSPost_" + sssPostGet.SSSPostID;
                                            string url = AppFunc.TextLinkReturning(item.url);

                                            Task<ResultModel> contentTable = Task.FromResult(db.Insert<ResultModel>("SP_SSSPostCrud", new DynamicParameters(new Dictionary<string, object?>
                                            {
                                                { "@processType", 2 },
                                                { "@rowType", 1 },
                                                { "@sssPostID", sssPostID },
                                                { "@categoryID", FormRowGet(model, "sssCategoryID") },
                                                { "@langID", item.langID },
                                                { "@sssPostName", FormRowGet(model, "sssPostName") },
                                                { "@subQuestion", item.caption },
                                                { "@subAnswer", item.content },
                                                { "@title", item.title },
                                                { "@description", item.description },
                                                { "@url", url },
                                            })));

                                            LinkApp(1, url, item.langID, "Page", "SSSPost", groupID, FormRowGet(model, "sssPostName"));
                                        }
                                    }
                                }
                                return await Task.FromResult(new ResultModel() { resultType = 1, resultCaption = "Başarılı", resultMessage = "Güncelleme Başarılı" });
                            }
                            else return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Hata", resultMessage = "Kayıt Bulunamadı" });
                        }
                        else return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Hata", resultMessage = "Bir hata oluştu lütfen sistem yöneticinize başvurun" });
                    }
                    else return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Hata", resultMessage = "Kayıt Bulunamadı" });
                }
                else return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Hata", resultMessage = "Kayıt Bulunamadı" });
            }
            catch (Exception)
            {
                await Log(false, "SSSPostApp");
                return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Hata", resultMessage = "Bir hata oluştu lütfen sistem yöneticinize başvurun" });
            }
        }
        #endregion

        #region Durum Güncelle - Sil
        [Route("sss-post-process")]
        [HttpPost]
        public async Task<ResultModel> SSSPostProcess(int processType, int sssPostID)
        {
            try
            {
                tblSSSPostMain? sssPostGet = db.QueryApp<tblSSSPostMain>(string.Format("SELECT * FROM tblSSSPostMain WHERE SSSPostID={0}", sssPostID)).FirstOrDefault();
                if (sssPostGet != null)
                {
                    if (processType == 1)
                    {
                        int isActive = sssPostGet.IsActive == true ? 0 : 1;

                        Task<ResultModel?> processApp = Task.FromResult(db.QueryApp<ResultModel>(string.Format("UPDATE tblSSSPostMain SET IsActive= {1} WHERE SSSPostID={0}", sssPostGet.SSSPostID, isActive)).FirstOrDefault());

                        await Log(processApp.IsCompletedSuccessfully, "SSSPostProcess");
                        if (processApp.IsCompletedSuccessfully)
                            return await Task.FromResult(new ResultModel() { resultType = 1, resultCaption = "Başarılı", resultMessage = "Güncelleme Başarılı" });
                        else
                            return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Hata", resultMessage = "Bir hata oluştu lütfen sistem yöneticinize başvurun" });
                    }
                    else if (processType == 2)
                    {
                        Task<ResultModel?> processApp = Task.FromResult(db.QueryApp<ResultModel>(string.Format("UPDATE tblSSSPostMain SET Deleted = 1 WHERE SSSPostID={0}", sssPostGet.SSSPostID)).FirstOrDefault());

                        await Log(processApp.IsCompletedSuccessfully, "SSSPostProcess");
                        if (processApp.IsCompletedSuccessfully)
                            return await Task.FromResult(new ResultModel() { resultType = 1, resultCaption = "Başarılı", resultMessage = "Güncelleme Başarılı" });
                        else
                            return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Hata", resultMessage = "Bir hata oluştu lütfen sistem yöneticinize başvurun" });
                    }
                    else return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Hata", resultMessage = "Kayıt Bulunamadı" });
                }
                else return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Hata", resultMessage = "Kayıt Bulunamadı" });
            }
            catch (Exception)
            {
                await Log(false, "SSSPostProcess");
                return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Hata", resultMessage = "Bir hata oluştu lütfen sistem yöneticinize başvurun" });
            }
        }
        #endregion

        #region Sıralama Güncelle
        [Route("sss-post-arrangement-update")]
        public async Task<ResultModel> SSSPostArrangementUpdate(string arrangementBody)
        {
            try
            {
                if (!string.IsNullOrEmpty(arrangementBody))
                {
                    foreach (var item in arrangementBody.Split('_'))
                    {
                        if (item != "" && item != "_" && item != null)
                        {
                            int sssPostID = Convert.ToInt32(item.Split(',').First());
                            int arrangement = Convert.ToInt32(item.Split(',').Last());

                            tblSSSPostMain? sssPostGet = db.QueryApp<tblSSSPostMain>(string.Format("SELECT * FROM tblSSSPostMain WHERE SSSPostID={0}", sssPostID)).FirstOrDefault();
                            if (sssPostGet != null)
                            {
                                Task<ResultModel?> result = Task.FromResult(db.QueryApp<ResultModel>(string.Format("UPDATE tblSSSPostMain SET Arrangement={0} WHERE SSSPostID={1}", arrangement, sssPostID)).FirstOrDefault());
                            }
                            else return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Hata", resultMessage = "Kayıt Bulunamadı" });
                        }
                    }
                    await Log(true, "SSSPostArrangementUpdate");
                    return await Task.FromResult(new ResultModel() { resultType = 1, resultCaption = "Başarılı", resultMessage = "Güncelleme Başarılı" });
                }
                else return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Hata", resultMessage = "Kayıt Bulunamadı" });
            }
            catch (Exception)
            {
                await Log(false, "SSSPostArrangementUpdate");
                return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Hata", resultMessage = "Bir hata oluştu lütfen sistem yöneticinize başvurun" });
            }
        }
        #endregion

        #region Galeri

        #region Liste
        [Route("sss-post-gallery")]
        public IActionResult SSSPostGallery(int sssPostID)
        {
            tblSSSPostMain? sssPostGet = db.QueryApp<tblSSSPostMain>(string.Format("SELECT * FROM tblSSSPostMain WHERE SSSPostID={0}", sssPostID)).FirstOrDefault();
            if (sssPostGet != null)
            {
                List<tblSSSPostGallery>? files = db.QueryApp<tblSSSPostGallery>(string.Format("SELECT * FROM tblSSSPostGallery WHERE SSSPostID={0}", sssPostID)).ToList();
                return View(Tuple.Create(sssPostGet, files));
            }
            return Redirect("/404");
        }
        #endregion

        #region Yükle
        [Route("sss-post-gallery-app")]
        public async Task<ResultModel> SSSPostGalleryApp(int sssPostID)
        {
            try
            {
                var files = Request.Form.Files;
                string? cdnURL = CdnUrl();
                for (int i = 0; i < files.Count; i++)
                {
                    string postResult = FtpPost(Request.Form.Files[i], "Transfer/SSSPosts", "sss");
                    if (postResult != "")
                    {
                        Task<ResultModel?> result = Task.FromResult(db.QueryApp<ResultModel>(string.Format("INSERT INTO tblSSSPostGallery (SSSPostID, Arrangement, FileURL) VALUES ({0}, 0, '{1}')", sssPostID, "Transfer/SSSPosts/" + postResult)).FirstOrDefault());
                    }
                }

                await Log(true, "ImageApp");
                return await Task.FromResult(new ResultModel() { resultType = 1, resultCaption = "Başarılı", resultMessage = "Güncelleme Başarılı" });
            }
            catch (Exception)
            {
                await Log(false, "ImageApp");
                return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Hata", resultMessage = "Bir hata oluştu lütfen sistem yöneticinize başvurun" });
            }
        }
        #endregion

        #region İşlemler
        [Route("sss-post-gallery-operation")]
        public async Task<ResultModel> SSSPostGalleryOperation(string[] inputList, string[] deletedList)
        {
            try
            {
                int inputItemID = 0, h = 0;
                for (int i = 0; i < inputList.Count() / 2; i++)
                {
                    inputItemID = Convert.ToInt32(inputList[h]);

                    tblSSSPostGallery? fileGet = db.QueryApp<tblSSSPostGallery>(string.Format("SELECT * FROM tblSSSPostGallery WHERE FileID={0}", inputItemID)).FirstOrDefault();
                    if (fileGet != null)
                    {
                        h = h + 1;
                        Task<ResultModel?> result = Task.FromResult(db.QueryApp<ResultModel>(string.Format("UPDATE tblSSSPostGallery SET Arrangement={0} WHERE FileID={1}", inputList[h], inputItemID)).FirstOrDefault());
                        h = h + 1;
                    }
                    else h = h + 2;
                }

                if (deletedList != null)
                {
                    for (int i = 0; i < deletedList.Count(); i++)
                    {
                        tblSSSPostGallery? fileGet = db.QueryApp<tblSSSPostGallery>(string.Format("SELECT * FROM tblSSSPostGallery WHERE FileID={0}", deletedList[i])).FirstOrDefault();
                        if (fileGet != null)
                        {
                            Task<ResultModel?> result = Task.FromResult(db.QueryApp<ResultModel>(string.Format("DELETE FROM tblSSSPostGallery WHERE FileID={0}", fileGet.FileID)).FirstOrDefault());
                        }
                    }
                }

                return await Task.FromResult(new ResultModel() { resultType = 1, resultCaption = "Başarılı", resultMessage = "Güncelleme Başarılı" });
            }
            catch (Exception)
            {
                await Log(false, "SSSPostGalleryOperation");
                return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Hata", resultMessage = "Bir hata oluştu lütfen sistem yöneticinize başvurun" });
            }
        }
        #endregion

        #endregion

        #endregion

        #endregion

        #region Blog

        #region Kategori

        #region Liste
        [Route("blog-categories")]
        public IActionResult BlogCategories()
        {
            List<SP_BlogCategories> list = db.GetAll<SP_BlogCategories>("SP_BlogCategories", new DynamicParameters(new Dictionary<string, object>
            {
                { "@langID", langID }
            }));
            return View(Tuple.Create(list, 0));
        }
        #endregion

        #region Oluştur - Güncelle
        [Route("blog-category-app")]
        [HttpPost]
        public async Task<ResultModel> BlogCategoryApp(LangItem[] list)
        {
            try
            {
                IFormCollection model = Request.Form;

                int blogCategoryID = Convert.ToInt32(FormRowGet(model, "blogCategoryID"));
                if (blogCategoryID == 0)
                {
                    Task<ResultModel> mainTable = Task.FromResult(db.Insert<ResultModel>("SP_BlogCategoryCrud", new DynamicParameters(new Dictionary<string, object?>
                    {
                        { "@processType", 1 },
                        { "@rowType", 1 },
                        { "@blogCategoryID", 0 },
                        { "@langID", 0 },
                        { "@blogCategoryName", FormRowGet(model, "blogCategoryName") },
                        { "@blogCategorySubName", "" },
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
                                if (!string.IsNullOrEmpty(item.caption))
                                {
                                    blogCategoryID = MainTableMaxID("tblBlogCategoryMain").Result;
                                    string groupID = "BlogCategory_" + blogCategoryID;
                                    string url = AppFunc.TextLinkReturning(item.url);

                                    Task<ResultModel> contentTable = Task.FromResult(db.Insert<ResultModel>("SP_BlogCategoryCrud", new DynamicParameters(new Dictionary<string, object?>
                                    {
                                        { "@processType", 2 },
                                        { "@rowType", 1 },
                                        { "@blogCategoryID", blogCategoryID },
                                        { "@langID", item.langID },
                                        { "@blogCategoryName", FormRowGet(model, "blogCategoryName") },
                                        { "@blogCategorySubName", item.caption },
                                        { "@title", item.title },
                                        { "@description", item.description },
                                        { "@url", item.url },
                                    })));

                                    LinkApp(1, url, item.langID, "Page", "BlogCategory", groupID, FormRowGet(model, "blogCategoryName"));
                                }
                            }
                            return await Task.FromResult(new ResultModel() { resultType = 1, resultCaption = "Başarılı", resultMessage = "Güncelleme Başarılı" });
                        }
                        else return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Uyarı", resultMessage = "Kayıt Bulunamadı" });
                    }
                    else return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Hata", resultMessage = "Bir hata oluştu lütfen sistem yöneticinize başvurun" });
                }
                else if (blogCategoryID > 0)
                {
                    tblBlogCategoryMain? blogCategoryGet = db.QueryApp<tblBlogCategoryMain>(string.Format("SELECT * FROM tblBlogCategoryMain WHERE BlogCategoryID={0}", blogCategoryID)).FirstOrDefault();
                    if (blogCategoryGet != null)
                    {
                        Task<ResultModel> mainTable = Task.FromResult(db.Insert<ResultModel>("SP_BlogCategoryCrud", new DynamicParameters(new Dictionary<string, object?>
                        {
                            { "@processType", 1 },
                            { "@rowType", 2 },
                            { "@blogCategoryID", blogCategoryGet.BlogCategoryID },
                            { "@langID", 0 },
                            { "@blogCategoryName", FormRowGet(model, "blogCategoryName") },
                            { "@blogCategorySubName", "" },
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
                                    tblBlogCategoryContent? cnt = db.QueryApp<tblBlogCategoryContent>(string.Format("SELECT * FROM tblBlogCategoryContent WHERE BlogCategoryID={0} AND LangID={1}", blogCategoryGet.BlogCategoryID, item.langID)).FirstOrDefault();
                                    if (cnt != null)
                                    {
                                        string groupID = "BlogCategory_" + blogCategoryGet.BlogCategoryID;
                                        string url = AppFunc.TextLinkReturning(item.url);
                                        LinkApp(2, "", item.langID, "Page", "BlogCategory", groupID, FormRowGet(model, "blogCategoryName"));

                                        if (!string.IsNullOrEmpty(item.caption))
                                        {
                                            Task<ResultModel> contentTable = Task.FromResult(db.Insert<ResultModel>("SP_BlogCategoryCrud", new DynamicParameters(new Dictionary<string, object?>
                                            {
                                                { "@processType", 2 },
                                                { "@rowType", 2 },
                                                { "@blogCategoryID", cnt.BlogCategoryID },
                                                { "@langID", item.langID },
                                                { "@blogCategoryName", FormRowGet(model, "blogCategoryName") },
                                                { "@blogCategorySubName", item.caption },
                                                { "@title", item.title },
                                                { "@description", item.description },
                                                { "@url", item.url },
                                            })));

                                            LinkApp(1, url, item.langID, "Page", "BlogCategory", groupID, FormRowGet(model, "blogCategoryName"));
                                        }
                                        else
                                        {
                                            Task<ResultModel?> deleteTable = Task.FromResult(db.QueryApp<ResultModel>(string.Format("DELETE FROM tblBlogCategoryContent WHERE BlogCategoryID={0} AND LangID={1}", blogCategoryID, item.langID)).FirstOrDefault());
                                        }
                                    }
                                    else
                                    {
                                        if (!string.IsNullOrEmpty(item.caption))
                                        {
                                            string groupID = "BlogCategory_" + blogCategoryID;
                                            string url = AppFunc.TextLinkReturning(item.url);

                                            Task<ResultModel> contentTable = Task.FromResult(db.Insert<ResultModel>("SP_BlogCategoryCrud", new DynamicParameters(new Dictionary<string, object?>
                                            {
                                                { "@processType", 2 },
                                                { "@rowType", 1 },
                                                { "@blogCategoryID", blogCategoryID },
                                                { "@langID", item.langID },
                                                { "@blogCategoryName", FormRowGet(model, "blogCategoryName") },
                                                { "@blogCategorySubName", item.caption },
                                                { "@title", item.title },
                                                { "@description", item.description },
                                                { "@url", item.url },
                                            })));

                                            LinkApp(1, url, item.langID, "Page", "BlogCategory", groupID, FormRowGet(model, "blogCategoryName"));
                                        }
                                    }
                                }
                                return await Task.FromResult(new ResultModel() { resultType = 1, resultCaption = "Başarılı", resultMessage = "Güncelleme Başarılı" });
                            }
                            else return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Uyarı", resultMessage = "Kayıt Bulunamadı" });
                        }
                        else return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Hata", resultMessage = "Bir hata oluştu lütfen sistem yöneticinize başvurun" });
                    }
                    else return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Uyarı", resultMessage = "Kayıt Bulunamadı" });
                }
                else return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Uyarı", resultMessage = "Kayıt Bulunamadı" });
            }
            catch (Exception)
            {
                await Log(false, "BlogCategoryApp");
                return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Hata", resultMessage = "Bir hata oluştu lütfen sistem yöneticinize başvurun" });
            }
        }
        #endregion

        #region Durum Güncelle - Sil
        [Route("blog-category-process")]
        [HttpPost]
        public async Task<ResultModel> BlogCategoryProcess(int processType, int blogCategoryID)
        {
            try
            {
                tblBlogCategoryMain? blogCategoryGet = db.QueryApp<tblBlogCategoryMain>(string.Format("SELECT * FROM tblBlogCategoryMain WHERE BlogCategoryID={0}", blogCategoryID)).FirstOrDefault();
                if (blogCategoryGet != null)
                {
                    if (processType == 1)
                    {
                        int isActive = blogCategoryGet.IsActive == true ? 0 : 1;

                        Task<ResultModel?> processApp = Task.FromResult(db.QueryApp<ResultModel>(string.Format("UPDATE tblBlogCategoryMain SET IsActive= {1} WHERE BlogCategoryID={0}", blogCategoryGet.BlogCategoryID, isActive)).FirstOrDefault());

                        await Log(processApp.IsCompletedSuccessfully, "BlogCategoryProcess");
                        if (processApp.IsCompletedSuccessfully)
                            return await Task.FromResult(new ResultModel() { resultType = 1, resultCaption = "Başarılı", resultMessage = "Güncelleme Başarılı" });
                        else
                            return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Hata", resultMessage = "Bir hata oluştu lütfen sistem yöneticinize başvurun" });
                    }
                    else if (processType == 2)
                    {
                        Task<ResultModel?> processApp = Task.FromResult(db.QueryApp<ResultModel>(string.Format("UPDATE tblBlogCategoryMain SET Deleted = 1 WHERE BlogCategoryID={0}", blogCategoryGet.BlogCategoryID)).FirstOrDefault());

                        await Log(processApp.IsCompletedSuccessfully, "BlogCategoryProcess");
                        if (processApp.IsCompletedSuccessfully)
                            return await Task.FromResult(new ResultModel() { resultType = 1, resultCaption = "Başarılı", resultMessage = "Güncelleme Başarılı" });
                        else
                            return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Hata", resultMessage = "Bir hata oluştu lütfen sistem yöneticinize başvurun" });
                    }
                    else return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Uyarı", resultMessage = "Kayıt Bulunamadı" });
                }
                else return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Uyarı", resultMessage = "Kayıt Bulunamadı" });
            }
            catch (Exception)
            {
                await Log(false, "BlogCategoryProcess");
                return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Hata", resultMessage = "Bir hata oluştu lütfen sistem yöneticinize başvurun" });
            }
        }
        #endregion

        #region Sıralama Güncelle
        [Route("blog-category-arrangement-update")]
        public async Task<ResultModel> BlogCategoryArrangementUpdate(string arrangementBody)
        {
            try
            {
                if (!string.IsNullOrEmpty(arrangementBody))
                {
                    foreach (var item in arrangementBody.Split('_'))
                    {
                        if (item != "" && item != "_" && item != null)
                        {
                            int blogCategoryID = Convert.ToInt32(item.Split(',').First());
                            int arrangement = Convert.ToInt32(item.Split(',').Last());

                            tblBlogCategoryMain? blogCategoryGet = db.QueryApp<tblBlogCategoryMain>(string.Format("SELECT * FROM tblBlogCategoryMain WHERE BlogCategoryID={0}", blogCategoryID)).FirstOrDefault();
                            if (blogCategoryGet != null)
                            {
                                Task<ResultModel?> result = Task.FromResult(db.QueryApp<ResultModel>(string.Format("UPDATE tblBlogCategoryMain SET Arrangement={0} WHERE BlogCategoryID={1}", arrangement, blogCategoryID)).FirstOrDefault());
                            }
                            else return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Uyarı", resultMessage = "Kayıt Bulunamadı" });
                        }
                    }
                    await Log(true, "BlogCategoryArrangementUpdate");
                    return await Task.FromResult(new ResultModel() { resultType = 1, resultCaption = "Başarılı", resultMessage = "Güncelleme Başarılı" });
                }
                else return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Uyarı", resultMessage = "Kayıt Bulunamadı" });
            }
            catch (Exception)
            {
                await Log(false, "BlogCategoryArrangementUpdate");
                return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Hata", resultMessage = "Bir hata oluştu lütfen sistem yöneticinize başvurun" });
            }
        }
        #endregion

        #region Galeri

        #region Liste
        [Route("blog-category-gallery")]
        public IActionResult BlogCategoryGallery(int blogCategoryID)
        {
            tblBlogCategoryMain? blogCategoryGet = db.QueryApp<tblBlogCategoryMain>(string.Format("SELECT * FROM tblBlogCategoryMain WHERE BlogCategoryID={0}", blogCategoryID)).FirstOrDefault();
            if (blogCategoryGet != null)
            {
                List<tblBlogCategoryGallery>? files = db.QueryApp<tblBlogCategoryGallery>(string.Format("SELECT * FROM tblBlogCategoryGallery WHERE BlogCategoryID={0}", blogCategoryID)).ToList();
                return View(Tuple.Create(blogCategoryGet, files));
            }
            return Redirect("/404");
        }
        #endregion

        #region Yükle
        [Route("blog-category-image-app")]
        public async Task<ResultModel> BlogCategoryImageApp(int blogCategoryID)
        {
            try
            {
                var files = Request.Form.Files;
                string? cdnURL = CdnUrl();
                for (int i = 0; i < files.Count; i++)
                {
                    string postResult = FtpPost(Request.Form.Files[i], "Transfer/BlogCategories", "blog-category");
                    if (postResult != "")
                    {
                        Task<ResultModel?> result = Task.FromResult(db.QueryApp<ResultModel>(string.Format("INSERT INTO tblBlogCategoryGallery (BlogCategoryID, Arrangement, FileURL) VALUES ({0}, 0, '{1}')", blogCategoryID, "Transfer/BlogCategories/" + postResult)).FirstOrDefault());
                    }
                }

                await Log(true, "BlogCategoryImageApp");
                return await Task.FromResult(new ResultModel() { resultType = 1, resultCaption = "Başarılı", resultMessage = "Güncelleme Başarılı" });
            }
            catch (Exception)
            {
                await Log(false, "BlogCategoryImageApp");
                return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Hata", resultMessage = "Bir hata oluştu lütfen sistem yöneticinize başvurun" });
            }
        }
        #endregion

        #region İşlemler
        [Route("blog-category-gallery-operation")]
        public async Task<ResultModel> BlogCategoryGalleryOperation(string[] inputList, string[] deletedList)
        {
            try
            {
                int inputItemID = 0, h = 0;
                for (int i = 0; i < inputList.Count() / 2; i++)
                {
                    inputItemID = Convert.ToInt32(inputList[h]);

                    tblBlogCategoryGallery? fileGet = db.QueryApp<tblBlogCategoryGallery>(string.Format("SELECT * FROM tblBlogCategoryGallery WHERE FileID={0}", inputItemID)).FirstOrDefault();
                    if (fileGet != null)
                    {
                        h = h + 1;
                        Task<ResultModel?> result = Task.FromResult(db.QueryApp<ResultModel>(string.Format("UPDATE tblBlogCategoryGallery SET Arrangement={0} WHERE FileID={1}", inputList[h], inputItemID)).FirstOrDefault());
                        h = h + 1;
                    }
                    else h = h + 2;
                }

                if (deletedList != null)
                {
                    for (int i = 0; i < deletedList.Count(); i++)
                    {
                        tblBlogCategoryGallery? fileGet = db.QueryApp<tblBlogCategoryGallery>(string.Format("SELECT * FROM tblBlogCategoryGallery WHERE FileID={0}", deletedList[i])).FirstOrDefault();
                        if (fileGet != null)
                        {
                            Task<ResultModel?> result = Task.FromResult(db.QueryApp<ResultModel>(string.Format("DELETE FROM tblBlogCategoryGallery WHERE FileID={0}", fileGet.FileID)).FirstOrDefault());
                        }
                    }
                }

                return await Task.FromResult(new ResultModel() { resultType = 1, resultCaption = "Başarılı", resultMessage = "Güncelleme Başarılı" });
            }
            catch (Exception)
            {
                await Log(false, "BlogCategoryGalleryOperation");
                return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Hata", resultMessage = "Bir hata oluştu lütfen sistem yöneticinize başvurun" });
            }
        }
        #endregion

        #endregion

        #endregion

        #region Post

        #region Liste
        [Route("blog-posts")]
        public IActionResult BlogPosts()
        {
            List<SP_BlogPosts> list = db.GetAll<SP_BlogPosts>("SP_BlogPosts", new DynamicParameters(new Dictionary<string, object>
            {
                { "@langID", langID }
            }));
            List<SP_BlogCategories> categories = db.GetAll<SP_BlogCategories>("SP_BlogCategories", new DynamicParameters(new Dictionary<string, object>
            {
                { "@langID", langID }
            }));
            return View(Tuple.Create(list, categories));
        }
        #endregion

        #region Oluştur - Güncelle
        [Route("blog-post-app")]
        [HttpPost]
        public async Task<ResultModel> BlogPostApp(LangItem[] list)
        {
            try
            {
                IFormCollection model = Request.Form;

                int blogPostID = Convert.ToInt32(FormRowGet(model, "blogPostID"));
                if (blogPostID == 0)
                {
                    Task<ResultModel> mainTable = Task.FromResult(db.Insert<ResultModel>("SP_BlogPostCrud", new DynamicParameters(new Dictionary<string, object?>
                    {
                        { "@processType", 1 },
                        { "@rowType", 1 },
                        { "@blogPostID", 0 },
                        { "@blogCategoryID", FormRowGet(model, "blogCategoryID") },
                        { "@langID", 0 },
                        { "@blogPostName", FormRowGet(model, "blogPostName") },
                        { "@blogPostSubName", "" },
                        { "@blogPostShortText", "" },
                        { "@blogPostContent", "" },
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
                                if (!string.IsNullOrEmpty(item.caption))
                                {
                                    blogPostID = MainTableMaxID("tblBlogPostMain").Result;
                                    string groupID = "BlogPost_" + blogPostID;
                                    string url = AppFunc.TextLinkReturning(item.url);

                                    Task<ResultModel> contentTable = Task.FromResult(db.Insert<ResultModel>("SP_BlogPostCrud", new DynamicParameters(new Dictionary<string, object?>
                                    {
                                        { "@processType", 2 },
                                        { "@rowType", 1 },
                                        { "@blogPostID", blogPostID },
                                        { "@blogCategoryID", FormRowGet(model, "blogCategoryID") },
                                        { "@langID", item.langID },
                                        { "@blogPostName", FormRowGet(model, "blogPostName") },
                                        { "@blogPostSubName", item.caption },
                                        { "@blogPostShortText", item.shortText },
                                        { "@blogPostContent", item.content },
                                        { "@title", item.title },
                                        { "@description", item.description },
                                        { "@url", url },
                                    })));

                                    LinkApp(1, url, item.langID, "Page", "BlogPost", groupID, FormRowGet(model, "blogPostName"));
                                }
                            }
                            return await Task.FromResult(new ResultModel() { resultType = 1, resultCaption = "Başarılı", resultMessage = "Güncelleme Başarılı" });
                        }
                        else return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Uyarı", resultMessage = "Kayıt Bulunamadı" });
                    }
                    else return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Hata", resultMessage = "Bir hata oluştu lütfen sistem yöneticinize başvurun" });
                }
                else if (blogPostID > 0)
                {
                    tblBlogPostMain? blogPostGet = db.QueryApp<tblBlogPostMain>(string.Format("SELECT * FROM tblBlogPostMain WHERE BlogPostID={0}", blogPostID)).FirstOrDefault();
                    if (blogPostGet != null)
                    {
                        Task<ResultModel> mainTable = Task.FromResult(db.Insert<ResultModel>("SP_BlogPostCrud", new DynamicParameters(new Dictionary<string, object?>
                        {
                            { "@processType", 1 },
                            { "@rowType", 2 },
                            { "@blogPostID", blogPostID },
                            { "@blogCategoryID", FormRowGet(model, "blogCategoryID") },
                            { "@langID", 0 },
                            { "@blogPostName", FormRowGet(model, "blogPostName") },
                            { "@blogPostSubName", "" },
                            { "@blogPostShortText", "" },
                            { "@blogPostContent", "" },
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
                                    tblBlogPostContent? cnt = db.QueryApp<tblBlogPostContent>(string.Format("SELECT * FROM tblBlogPostContent WHERE BlogPostID={0} AND LangID={1}", blogPostGet.BlogPostID, item.langID)).FirstOrDefault();
                                    if (cnt != null)
                                    {
                                        string groupID = "BlogPost_" + blogPostGet.BlogPostID;
                                        string url = AppFunc.TextLinkReturning(item.url);
                                        LinkApp(2, "", item.langID, "Page", "BlogPost", groupID, FormRowGet(model, "blogPostName"));

                                        if (!string.IsNullOrEmpty(item.caption))
                                        {
                                            Task<ResultModel> contentTable = Task.FromResult(db.Insert<ResultModel>("SP_BlogPostCrud", new DynamicParameters(new Dictionary<string, object?>
                                            {
                                                { "@processType", 2 },
                                                { "@rowType", 2 },
                                                { "@blogPostID", cnt.BlogPostID },
                                                { "@blogCategoryID", FormRowGet(model, "blogCategoryID") },
                                                { "@langID", item.langID },
                                                { "@blogPostName", FormRowGet(model, "blogPostName") },
                                                { "@blogPostSubName", item.caption },
                                                { "@blogPostShortText", item.shortText },
                                                { "@blogPostContent", item.content },
                                                { "@title", item.title },
                                                { "@description", item.description },
                                                { "@url", url },
                                            })));

                                            LinkApp(1, url, item.langID, "Page", "BlogPost", groupID, FormRowGet(model, "blogPostName"));
                                        }
                                        else
                                        {
                                            Task<ResultModel?> deleteTable = Task.FromResult(db.QueryApp<ResultModel>(string.Format("DELETE FROM tblBlogPostContent WHERE BlogPostID={0} AND LangID={1}", blogPostGet.BlogPostID, item.langID)).FirstOrDefault());
                                        }
                                    }
                                    else
                                    {
                                        if (!string.IsNullOrEmpty(item.caption))
                                        {
                                            string groupID = "BlogPost_" + blogPostGet.BlogPostID;
                                            string url = AppFunc.TextLinkReturning(item.url);

                                            Task<ResultModel> contentTable = Task.FromResult(db.Insert<ResultModel>("SP_BlogPostCrud", new DynamicParameters(new Dictionary<string, object?>
                                            {
                                                { "@processType", 2 },
                                                { "@rowType", 1 },
                                                { "@blogPostID", blogPostID },
                                                { "@blogCategoryID", FormRowGet(model, "blogCategoryID") },
                                                { "@langID", item.langID },
                                                { "@blogPostName", FormRowGet(model, "blogPostName") },
                                                { "@blogPostSubName", item.caption },
                                                { "@blogPostShortText", item.shortText },
                                                { "@blogPostContent", item.content },
                                                { "@title", item.title },
                                                { "@description", item.description },
                                                { "@url", url },
                                            })));

                                            LinkApp(1, url, item.langID, "Page", "BlogPost", groupID, FormRowGet(model, "blogPostName"));
                                        }
                                    }
                                }
                                return await Task.FromResult(new ResultModel() { resultType = 1, resultCaption = "Başarılı", resultMessage = "Güncelleme Başarılı" });
                            }
                            else return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Uyarı", resultMessage = "Kayıt Bulunamadı" });
                        }
                        else return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Hata", resultMessage = "Bir hata oluştu lütfen sistem yöneticinize başvurun" });
                    }
                    else return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Uyarı", resultMessage = "Kayıt Bulunamadı" });
                }
                else return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Uyarı", resultMessage = "Kayıt Bulunamadı" });
            }
            catch (Exception)
            {
                await Log(false, "BlogPostApp");
                return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Hata", resultMessage = "Bir hata oluştu lütfen sistem yöneticinize başvurun" });
            }
        }
        #endregion

        #region Durum Güncelle - Sil
        [Route("blog-post-process")]
        [HttpPost]
        public async Task<ResultModel> BlogPostProcess(int processType, int blogPostID)
        {
            try
            {
                tblBlogPostMain? blogPostGet = db.QueryApp<tblBlogPostMain>(string.Format("SELECT * FROM tblBlogPostMain WHERE BlogPostID={0}", blogPostID)).FirstOrDefault();
                if (blogPostGet != null)
                {
                    if (processType == 1)
                    {
                        int isActive = blogPostGet.IsActive == true ? 0 : 1;

                        Task<ResultModel?> processApp = Task.FromResult(db.QueryApp<ResultModel>(string.Format("UPDATE tblBlogPostMain SET IsActive= {1} WHERE BlogPostID={0}", blogPostGet.BlogPostID, isActive)).FirstOrDefault());

                        await Log(processApp.IsCompletedSuccessfully, "BlogPostProcess");
                        if (processApp.IsCompletedSuccessfully)
                            return await Task.FromResult(new ResultModel() { resultType = 1, resultCaption = "Başarılı", resultMessage = "Güncelleme Başarılı" });
                        else
                            return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Hata", resultMessage = "Bir hata oluştu lütfen sistem yöneticinize başvurun" });
                    }
                    else if (processType == 2)
                    {
                        Task<ResultModel?> processApp = Task.FromResult(db.QueryApp<ResultModel>(string.Format("UPDATE tblBlogPostMain SET Deleted = 1 WHERE BlogPostID={0}", blogPostGet.BlogPostID)).FirstOrDefault());

                        await Log(processApp.IsCompletedSuccessfully, "BlogPostProcess");
                        if (processApp.IsCompletedSuccessfully)
                            return await Task.FromResult(new ResultModel() { resultType = 1, resultCaption = "Başarılı", resultMessage = "Güncelleme Başarılı" });
                        else
                            return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Hata", resultMessage = "Bir hata oluştu lütfen sistem yöneticinize başvurun" });
                    }
                    else return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Uyarı", resultMessage = "Kayıt Bulunamadı" });
                }
                else return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Uyarı", resultMessage = "Kayıt Bulunamadı" });
            }
            catch (Exception)
            {
                await Log(false, "BlogPostProcess");
                return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Hata", resultMessage = "Bir hata oluştu lütfen sistem yöneticinize başvurun" });
            }
        }
        #endregion

        #region Sıralama Güncelle
        [Route("blog-post-arrangement-update")]
        public async Task<ResultModel> BlogPostArrangementUpdate(string arrangementBody)
        {
            try
            {
                if (!string.IsNullOrEmpty(arrangementBody))
                {
                    foreach (var item in arrangementBody.Split('_'))
                    {
                        if (item != "" && item != "_" && item != null)
                        {
                            int blogPostID = Convert.ToInt32(item.Split(',').First());
                            int arrangement = Convert.ToInt32(item.Split(',').Last());

                            tblBlogPostMain? blogPostGet = db.QueryApp<tblBlogPostMain>(string.Format("SELECT * FROM tblBlogPostMain WHERE BlogPostID={0}", blogPostID)).FirstOrDefault();
                            if (blogPostGet != null)
                            {
                                Task<ResultModel?> result = Task.FromResult(db.QueryApp<ResultModel>(string.Format("UPDATE tblBlogPostMain SET Arrangement={0} WHERE BlogPostID={1}", arrangement, blogPostID)).FirstOrDefault());
                            }
                            else return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Uyarı", resultMessage = "Kayıt Bulunamadı" });
                        }
                    }
                    await Log(true, "BlogPostArrangementUpdate");
                    return await Task.FromResult(new ResultModel() { resultType = 1, resultCaption = "Başarılı", resultMessage = "Güncelleme Başarılı" });
                }
                else return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Uyarı", resultMessage = "Kayıt Bulunamadı" });
            }
            catch (Exception)
            {
                await Log(false, "BlogPostArrangementUpdate");
                return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Hata", resultMessage = "Bir hata oluştu lütfen sistem yöneticinize başvurun" });
            }
        }
        #endregion

        #region Liste Görsel
        [Route("blog-post-image-app")]
        public async Task<ResultModel> BlogPostsListImageApp(int blogPostID)
        {
            try
            {
                var files = Request.Form.Files;
                if (files.Count() > 0)
                {
                    string postResult = FtpPost(Request.Form.Files[0], "Vossence/BlogPosts", "blog");
                    if (postResult != "")
                    {
                        Task<ResultModel?> result = Task.FromResult(db.QueryApp<ResultModel>(string.Format("UPDATE tblBlogPostMain SET ListImage='{0}' WHERE BlogPostID={1}", postResult, blogPostID)).FirstOrDefault());
                    }
                }

                await Log(true, "BlogPostsListImageApp");
                return await Task.FromResult(new ResultModel() { resultType = 1, resultCaption = "Başarılı", resultMessage = "Güncelleme Başarılı" });
            }
            catch (Exception)
            {
                await Log(false, "BlogPostsListImageApp");
                return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Hata", resultMessage = "Bir hata oluştu lütfen sistem yöneticinize başvurun" });
            }
        }
        #endregion

        #endregion

        #endregion

        #region Hizmet Yönetimi

        #region Liste
        [Route("service-management")]
        public IActionResult ServiceManagement()
        {
            List<SP_Services> list = db.GetAll<SP_Services>("SP_Services",
                new DynamicParameters(
                    new Dictionary<string, object>
                    {
                        { "@langID", langID }
                    }));

            return View(Tuple.Create(list, 0));
        }
        #endregion

        #region Oluştur - Güncelle
        [Route("service-app")]
        [HttpPost]
        public async Task<ResultModel> ServiceApp(LangItem[] list)
        {
            try
            {
                IFormCollection model = Request.Form;

                int serviceID = Convert.ToInt32(FormRowGet(model, "serviceID"));
                if (serviceID == 0)
                {
                    Task<ResultModel> mainTable = Task.FromResult(db.Insert<ResultModel>("SP_ServiceCrud", new DynamicParameters(new Dictionary<string, object?>
                    {
                        { "@processType", 1 },
                        { "@rowType", 1 },
                        { "@serviceID", 0 },
                        { "@langID", 0 },
                        { "@serviceName", FormRowGet(model, "serviceName") },
                        { "@serviceSubName", "" },
                        { "@serviceContent", "" },
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
                                    serviceID = MainTableMaxID("tblServiceMain").Result;
                                    string groupID = "Service_" + serviceID;
                                    string url = AppFunc.TextLinkReturning(item.url);

                                    Task<ResultModel> contentTable = Task.FromResult(db.Insert<ResultModel>("SP_ServiceCrud", new DynamicParameters(new Dictionary<string, object?>
                                    {
                                        { "@processType", 2 },
                                        { "@rowType", 1 },
                                        { "@serviceID", serviceID },
                                        { "@langID", item.langID },
                                        { "@serviceName", FormRowGet(model, "serviceName") },
                                        { "@serviceSubName", item.caption },
                                        { "@serviceContent", item.content },
                                        { "@title", item.title },
                                        { "@description", item.description },
                                        { "@url", url },
                                    })));

                                    LinkApp(1, url, item.langID, "Page", "ServiceDetail", groupID, FormRowGet(model, "serviceName"));
                                }
                            }
                            return await Task.FromResult(new ResultModel() { resultType = 1, resultCaption = "Başarılı", resultMessage = "Güncelleme Başarılı" });
                        }
                        else return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Uyarı", resultMessage = "Kayıt Bulunamadı" });
                    }
                    else return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Hata", resultMessage = "Bir hata oluştu lütfen sistem yöneticinize başvurun." });
                }
                else if (serviceID > 0)
                {
                    tblServiceMain? serviceGet = db.QueryApp<tblServiceMain>(string.Format("SELECT * FROM tblServiceMain WHERE ServiceID={0}", serviceID)).FirstOrDefault();
                    if (serviceGet != null)
                    {
                        Task<ResultModel> mainTable = Task.FromResult(db.Insert<ResultModel>("SP_ServiceCrud", new DynamicParameters(new Dictionary<string, object?>
                        {
                            { "@processType", 1 },
                            { "@rowType", 2 },
                            { "@serviceID", serviceGet.ServiceID },
                            { "@langID", 0 },
                            { "@serviceName", FormRowGet(model, "serviceName") },
                            { "@serviceSubName", "" },
                            { "@serviceContent", "" },
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

                                    tblServiceContent? cnt = db.QueryApp<tblServiceContent>(string.Format("SELECT * FROM tblServiceContent WHERE ServiceID={0} AND LangID={1}", serviceGet.ServiceID, item.langID)).FirstOrDefault();
                                    if (cnt != null)
                                    {
                                        string groupID = "Service_" + serviceID;
                                        string url = AppFunc.TextLinkReturning(item.url);
                                        LinkApp(2, "", item.langID, "Page", "ServiceDetail", groupID, FormRowGet(model, "serviceName"));

                                        if (!string.IsNullOrEmpty(item.caption))
                                        {
                                            Task<ResultModel> contentTable = Task.FromResult(db.Insert<ResultModel>("SP_ServiceCrud", new DynamicParameters(new Dictionary<string, object?>
                                            {
                                                { "@processType", 2 },
                                                { "@rowType", 2 },
                                                { "@serviceID", cnt.ServiceID },
                                                { "@langID", item.langID },
                                                { "@serviceName", FormRowGet(model, "serviceName") },
                                                { "@serviceSubName", item.caption },
                                                { "@serviceContent", item.content },
                                                { "@title", item.title },
                                                { "@description", item.description },
                                                { "@url", url },
                                            })));

                                            LinkApp(1, url, item.langID, "Page", "ServiceDetail", groupID, FormRowGet(model, "serviceName"));
                                        }
                                        else
                                        {
                                            Task<ResultModel?> deleteTable = Task.FromResult(db.QueryApp<ResultModel>(string.Format("DELETE FROM tblServiceContent WHERE ServiceID={0} AND LangID={1}", serviceGet.ServiceID, item.langID)).FirstOrDefault());
                                        }
                                    }
                                    else
                                    {
                                        if (!string.IsNullOrEmpty(item.caption))
                                        {
                                            string groupID = "Service_" + serviceID;
                                            string url = AppFunc.TextLinkReturning(item.url);

                                            Task<ResultModel> contentTable = Task.FromResult(db.Insert<ResultModel>("SP_ServiceCrud", new DynamicParameters(new Dictionary<string, object?>
                                            {
                                                { "@processType", 2 },
                                                { "@rowType", 1 },
                                                { "@serviceID", serviceID },
                                                { "@langID", item.langID },
                                                { "@serviceName", FormRowGet(model, "serviceName") },
                                                { "@serviceSubName", item.caption },
                                                { "@serviceContent", item.content },
                                                { "@title", item.title },
                                                { "@description", item.description },
                                                { "@url", url },
                                            })));

                                            LinkApp(1, url, item.langID, "Page", "ServiceDetail", groupID, FormRowGet(model, "serviceName"));
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
                await Log(false, "ServiceApp");
                return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Hata", resultMessage = "Bir hata oluştu lütfen sistem yöneticinize başvurun." });
            }
        }
        #endregion

        #region Durum Güncelle - Sil
        [Route("service-process")]
        [HttpPost]
        public async Task<ResultModel> ServiceProcess(int processType, int serviceID)
        {
            try
            {
                tblServiceMain? serviceGet = db.QueryApp<tblServiceMain>(string.Format("SELECT * FROM tblServiceMain WHERE ServiceID={0}", serviceID)).FirstOrDefault();
                if (serviceGet != null)
                {
                    if (processType == 1)
                    {
                        int isActive = serviceGet.IsActive == true ? 0 : 1;

                        Task<ResultModel?> processApp = Task.FromResult(db.QueryApp<ResultModel>(string.Format("UPDATE tblServiceMain SET IsActive= {1} WHERE ServiceID={0}", serviceGet.ServiceID, isActive)).FirstOrDefault());

                        await Log(processApp.IsCompletedSuccessfully, "ServiceProcess");
                        if (processApp.IsCompletedSuccessfully)
                            return await Task.FromResult(new ResultModel() { resultType = 1, resultCaption = "Başarılı", resultMessage = "Güncelleme Başarılı" });
                        else
                            return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Hata", resultMessage = "Bir hata oluştu lütfen sistem yöneticinize başvurun." });
                    }
                    else if (processType == 2)
                    {
                        Task<ResultModel?> processApp = Task.FromResult(db.QueryApp<ResultModel>(string.Format("UPDATE tblServiceMain SET Deleted = 1 WHERE ServiceID={0}", serviceGet.ServiceID)).FirstOrDefault());

                        await Log(processApp.IsCompletedSuccessfully, "ServiceProcess");
                        if (processApp.IsCompletedSuccessfully)
                            return await Task.FromResult(new ResultModel() { resultType = 1, resultCaption = "Başarılı", resultMessage = "Güncelleme Başarılı" });
                        else
                            return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Hata", resultMessage = "Bir hata oluştu lütfen sistem yöneticinize başvurun." });
                    }
                    else return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Uyarı", resultMessage = "Kayıt Bulunamadı" });
                }
                else return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Uyarı", resultMessage = "Kayıt Bulunamadı" });
            }
            catch (Exception)
            {
                await Log(false, "ServiceProcess");
                return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Hata", resultMessage = "Bir hata oluştu lütfen sistem yöneticinize başvurun." });
            }
        }
        #endregion

        #region Sıralama Güncelle
        [Route("service-arrangement-update")]
        public async Task<ResultModel> ServiceArrangementUpdate(string arrangementBody)
        {
            try
            {
                if (!string.IsNullOrEmpty(arrangementBody))
                {
                    foreach (var item in arrangementBody.Split('_'))
                    {
                        if (item != "" && item != "_" && item != null)
                        {
                            int serviceID = Convert.ToInt32(item.Split(',').First());
                            int arrangement = Convert.ToInt32(item.Split(',').Last());

                            tblServiceMain? serviceGet = db.QueryApp<tblServiceMain>(string.Format("SELECT * FROM tblServiceMain WHERE ServiceID={0}", serviceID)).FirstOrDefault();
                            if (serviceGet != null)
                            {
                                Task<ResultModel?> result = Task.FromResult(db.QueryApp<ResultModel>(string.Format("UPDATE tblServiceMain SET Arrangement={0} WHERE ServiceID={1}", arrangement, serviceID)).FirstOrDefault());
                            }
                            else return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Uyarı", resultMessage = "Kayıt Bulunamadı" });
                        }
                    }
                    await Log(true, "ServiceArrangementUpdate");
                    return await Task.FromResult(new ResultModel() { resultType = 1, resultCaption = "Başarılı", resultMessage = "Güncelleme Başarılı" });
                }
                else return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Uyarı", resultMessage = "Kayıt Bulunamadı" });
            }
            catch (Exception)
            {
                await Log(false, "ServiceArrangementUpdate");
                return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Hata", resultMessage = "Bir hata oluştu lütfen sistem yöneticinize başvurun." });
            }
        }
        #endregion

        #region Liste Görsel
        [Route("service-image-app")]
        public async Task<ResultModel> ServiceImageApp(int serviceID)
        {
            try
            {
                var files = Request.Form.Files;
                if (files.Count() > 0)
                {
                    string serviceImg = FtpPost(Request.Form.Files[0], "Vossence/Services", "service");
                    if (serviceImg != "")
                    {
                        Task<ResultModel?> result = Task.FromResult(db.QueryApp<ResultModel>(string.Format("UPDATE tblServiceContent SET ImageURL='{0}' WHERE ServiceID={1}", serviceImg, serviceID)).FirstOrDefault());

                        await Log(true, "ServiceImageApp");
                        if (result.IsCompletedSuccessfully)
                            return await Task.FromResult(new ResultModel() { resultType = 1, resultCaption = "Başarılı", resultMessage = "Güncelleme Başarılı" });
                        else return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Hata", resultMessage = "Bir hata oluştu lütfen sistem yöneticinize başvurun." });
                    }
                    else return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Hata", resultMessage = "Bir hata oluştu lütfen sistem yöneticinize başvurun." });
                }
                else return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Uyarı", resultMessage = "Kayıt Bulunamadı" });
            }
            catch (Exception)
            {
                await Log(false, "ServiceImageApp");
                return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Hata", resultMessage = "Bir hata oluştu lütfen sistem yöneticinize başvurun." });
            }
        }
        #endregion

        #endregion
    }
}