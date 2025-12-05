using Vossence.DATA.Helper;
using Vossence.DATA.ORM;
using Vossence.DATA.Table;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using System.Data;
using System.Net;
using System.Text;
using static Vossence.DATA.Validation.Cls;

namespace Vossence.ADMIN.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SharedController : Controller
    {
        private readonly IDapper db;
        private readonly IConfiguration? configuration;
        private readonly ILogger<SharedController> logger;

        public int langID { get; set; } = 1;

        #region Ctor
        public SharedController(IDapper dapper, IConfiguration? configuration, ILogger<SharedController> logger)
        {
            this.db = dapper;
            this.configuration = configuration;
            this.logger = logger;
        }
        #endregion

        #region Açılışta Bu Çalışır
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (SessionGet("LangID") == null) SessionNull();
            langID = Convert.ToInt32(SessionGet("LangID"));

            tblSystemSetting? systemGet = db.QueryApp<tblSystemSetting>(string.Format("SELECT * FROM tblSystemSetting WHERE SystemID = {0}", 1)).FirstOrDefault();
            if (systemGet != null)
            {
                ViewBag.CdnURL = systemGet.CdnURL;
                ViewBag.ProductCodeStart = systemGet.ProductStart + "_";
            }
            else
            {
                context.Result = new RedirectResult("/404");
                return;
            }
        }
        #endregion

        #region Kullanıcı Getir
        public string? UserName()
        {
            if (User != null)
                if (User.Identity != null)
                    return User.Identity.Name;
            return "";
        }
        //public string UserID()
        //{
        //    AspNetUsers? userGet = db.QueryApp<AspNetUsers>(string.Format("SELECT * FROM AspNetUsers WHERE UserName='{0}'", UserName())).FirstOrDefault();
        //    return userGet != null ? userGet.Id : "##ERROR##";
        //}
        #endregion

        #region Log
        public async Task<string> Log(bool isCompleted, string action)
        {
            if (isCompleted)
            {
                logger.LogInformation(eventId: new EventId(1, action), "Kullanıcı : " + UserName());
            }
            else
            {
                logger.LogError(eventId: new EventId(1, action), new Exception("Exception"), "Kullanıcı : " + UserName());
            }
            return await Task.FromResult("");
        }
        #endregion

        #region Form Row Get
        public string FormRowGet(IFormCollection form, string thisRow)
        {
            return form[thisRow].ToString() == "undefined" ? null : form[thisRow].ToString();
        }
        #endregion

        #region Application.Json Value Getir
        public string? ApplicationJsonPromptGet(string columnName)
        {
            return configuration!.GetValue<string>(columnName);
        }
        #endregion

        #region Ftp Url
        public string? FtpUrl()
        {
            tblSystemSetting? systemGet = db.QueryApp<tblSystemSetting>(string.Format("SELECT * FROM tblSystemSetting WHERE SystemID=1")).FirstOrDefault();
            if (systemGet != null)
                return systemGet.FtpURL;
            return "";
        }
        #endregion

        #region IP Numarası
        public string IpNm()
        {
            var ipGet = HttpContext.Connection.RemoteIpAddress?.ToString();
            if (ipGet != null)
                return ipGet;
            return "";
        }
        #endregion

        #region CdnURL
        public string? CdnUrl()
        {
            tblSystemSetting? systemGet = db.QueryApp<tblSystemSetting>(string.Format("SELECT * FROM tblSystemSetting WHERE SystemID=1")).FirstOrDefault();
            if (systemGet != null)
            {
                return systemGet.CdnURL;
            }
            return "";
        }
        #endregion

        #region FTP

        #region Ftp Ayar Getir
        public tblFtpSetting? FtpSetting(int ftpID)
        {
            tblFtpSetting? ftpSettingGet = db.QueryApp<tblFtpSetting>(string.Format("SELECT * FROM tblFtpSetting WHERE FtpID={0}", ftpID)).FirstOrDefault();
            if (ftpSettingGet != null)
                return ftpSettingGet;
            return null;
        }
        #endregion

        #region FTP Post
        public string FtpPost(IFormFile file, string folderName, string fileStart)
        {
            try
            {
                if (file.Length > 0)
                {
                    string extension = file.FileName.Split('.').Last();
                    string fileName = fileStart + "_" + Guid.NewGuid().ToString() + "." + extension;
                    tblFtpSetting ftpSetting = FtpSetting(1)!;
                    IFormFile fileConvert = file;
                    byte[]? fileData = null;
                    using (var binaryReader = new BinaryReader(fileConvert.OpenReadStream()))
                    {
                        fileData = binaryReader.ReadBytes((int)file.Length);
                    }

                    System.Net.FtpWebRequest request = (System.Net.FtpWebRequest)System.Net.WebRequest.Create(ftpSetting.FtpIP + FtpUrl() + folderName + "/" + fileName);
                    request.Method = WebRequestMethods.Ftp.UploadFile;
                    request.Credentials = new NetworkCredential(ftpSetting.FtpName, ftpSetting.FtpPassword);
                    request.KeepAlive = true;
                    request.ContentLength = fileData.Length;
                    request.UsePassive = true;
                    request.UseBinary = true;

                    using (Stream requestStream = request.GetRequestStream())
                    {
                        requestStream.Write(fileData, 0, fileData.Length);
                        requestStream.Close();
                    }
                    FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                    response.Close();
                    using (StreamReader fileStream = new StreamReader(file.OpenReadStream()))
                    {
                        fileData = Encoding.UTF8.GetBytes(fileStream.ReadToEnd());
                        fileStream.Close();
                    }
                    return fileName;
                }
                return "";
            }
            catch (Exception)
            {
                return "";
            }
        }
        #endregion

        #endregion

        #region Ürün Kodu Başlangıcı Getir
        public string? ProductStartCode()
        {
            tblSystemSetting? systemGet = db.QueryApp<tblSystemSetting>(string.Format("SELECT * FROM tblSystemSetting WHERE SystemID=1")).FirstOrDefault();
            if (systemGet != null)
            {
                return systemGet.ProductStart + "_";
            }
            return "";
        }
        #endregion

        #region Ürün Kodu
        public string? ProductCode()
        {
            bool IsTrue = true;
            while (IsTrue)
            {
                string prdCode = AppFunc.ProductCodeApp(ProductStartCode());
                tblProductContent? productGet = db.QueryApp<tblProductContent>(string.Format("SELECT * FROM tblProductContent WHERE ProductCode='{0}'", prdCode)).FirstOrDefault();
                if (productGet != null)
                {
                    IsTrue = true;
                }
                else
                {
                    IsTrue = false;
                    return prdCode;
                }
            }
            return "";
        }
        #endregion

        #region Session

        #region Get
        public string? SessionGet(string sessionName)
        {
            return HttpContext.Session.GetString(sessionName);
        }
        #endregion

        #region Set
        public string? SessionSet(string sessionName, object sessionColumn)
        {
            string? sessionValue = sessionColumn.ToString();
            HttpContext.Session.SetString(sessionName, sessionValue);
            return sessionValue;
        }
        #endregion

        #region Session Null
        public void SessionNull()
        {
            tblLanguage? langGet = Task.FromResult(db.GetAll<tblLanguage>("SP_LanguageList", new DynamicParameters(
            new Dictionary<string, object>
            {
                { "@langID", -1 }
            }),
            commandType: CommandType.StoredProcedure)).Result.FirstOrDefault(x => x.LangDefault == true && x.GroupID == 1);

            if (langGet == null)
                langGet = Task.FromResult(db.GetAll<tblLanguage>("SP_LanguageList", new DynamicParameters(
                new Dictionary<string, object>
                {
                    { "@langID", -1 }
                }),
                commandType: CommandType.StoredProcedure)).Result.FirstOrDefault(x => x.GroupID == 1);
            if (langGet != null)
            {
                if (langGet.LangID != 0 && langGet.LangCode != null)
                {
                    SessionSet("LangID", langGet.LangID);
                    SessionSet("LangCode", langGet.LangCode);
                }
            }
        }
        #endregion

        #endregion

        #region Main Tablo Max ID
        public async Task<int> MainTableMaxID(string tableName)
        {
            return await Task.FromResult(db.Get<int>("SP_MainTableMax", new DynamicParameters(new Dictionary<string, object?> { { "@tableName", tableName } })));
        }
        #endregion

        #region Main Tablo Arrangement
        public async Task<int> MainTableArrangement(string columnName, string tableName)
        {
            string? maxID = db.QueryApp<string>(string.Format("SELECT MAX({0}) FROM {1}", columnName, tableName)).FirstOrDefault();
            if (maxID == null)
                return 1;
            return await Task.FromResult(Convert.ToInt32(maxID) + 1);
        }
        #endregion

        #region Link
        public void LinkApp(int processType, string link, int langID, string controller, string action, string groupID, string? infoLink)
        {
            tblLanguage? langGet = db.QueryApp<tblLanguage>(string.Format("SELECT * FROM tblLanguage WHERE LangID={0}", langID)).FirstOrDefault();
            if (langGet != null)
            {
                if (processType == 1)
                {
                    link = AppFunc.TextLinkReturning(link);

                    tblLinkInfo? infoCnt = db.QueryApp<tblLinkInfo>(string.Format("SELECT * FROM tblLinkInfo WHERE Text='{0}'", AppFunc.TextLinkReturning(infoLink))).FirstOrDefault();
                    if (infoCnt != null)
                    {
                        controller = infoCnt.Controller;
                        action = infoCnt.Action;
                    }
                    Task<ResultModel?> mainTable = Task.FromResult(db.QueryApp<ResultModel>(string.Format("INSERT INTO tblLink (LangID, LangCode, Link, Controller, Action, GroupID) VALUES ({0}, '{1}', '{2}', '{3}','{4}','{5}')", langID, langGet.LangCode, link, controller, action, groupID)).FirstOrDefault());
                }
                else if (processType == 2)
                {
                    Task<ResultModel?> mainTable = Task.FromResult(db.QueryApp<ResultModel>(string.Format("DELETE FROM tblLink WHERE GroupID='{0}' AND LangID={1}", groupID, langID)).FirstOrDefault());
                }
            }
        }

        #endregion

        #region Dil

        #region Default Dil Getir
        public tblLanguage DefaultLanguageGet()
        {
            var langGet = db.GetAll<tblLanguage>("SP_LanguageList",
                new DynamicParameters(
                    new Dictionary<string, object>
                    {
                        { "@langID", -1 }
                    }),
                commandType: CommandType.StoredProcedure).FirstOrDefault(x => x.LangDefault == true);
            return langGet;
        }
        #endregion

        #region Diller
        public List<tblLanguage> Langs()
        {
            return db.GetAll<tblLanguage>("SP_LanguageList",
                new DynamicParameters(
                    new Dictionary<string, object>
                        {
                            { "@langID", -1 }
                        }),
            commandType: CommandType.StoredProcedure);
        }
        #endregion

        #endregion
    }
}
