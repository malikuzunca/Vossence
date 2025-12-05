using Vossence.DATA.ORM;
using Vossence.DATA.Table;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace Vossence.ADMIN.Controllers
{
    public class HelperController : SharedController
    {
        private readonly IDapper db;
        private readonly IConfiguration? configuration;
        private IHttpContextAccessor? httpContext;

        #region Ctor
        public HelperController(IDapper dapper, IConfiguration? configuration, IHttpContextAccessor? httpContext, ILogger<SharedController> logger) : base(dapper, configuration, logger)
        {
            this.db = dapper;
            this.httpContext = httpContext;
            this.configuration = configuration;

            if (httpContext != null)
                if (httpContext.HttpContext != null)
                    langID = Convert.ToInt32(httpContext.HttpContext.Session.GetString("LangID"));
        }
        #endregion

        #region Dil
        public List<tblLanguage> LangList()
        {
            return db.GetAll<tblLanguage>("SP_LanguageList", new DynamicParameters(new Dictionary<string, object> { { "@langID", 1 } }));
        }

        [Route("lang-body-app")]
        public string LangBodyApp(int processType, int tableGroup = -1)
        {
            int iRow = 1;
            int qRow = 1;

            string returnBody = "";
            string processBool = "";
            string rowActive = "";
            string processID = processType == 1 ? "langCreateTab" : "langUpdateTab";
            string showActive = "";

            returnBody += "<ul class='nav nav-underline fs-9' id='" + processID + "' role='tablist'>";
            foreach (var item in LangList())
            {
                rowActive = iRow == 1 ? "active" : "";
                processBool = iRow == 1 ? "true" : "false";

                returnBody += "<li class='nav-item'><a class='nav-link " + rowActive + "' id='lang-tab-" + item.LangID + "' data-bs-toggle='tab' href='#tab-lang-" + item.LangID + "' role='tab' aria-controls='tab-lang-" + item.LangID + "' aria-selected='" + processBool + "'>" + item.LangName + "</a></li>";

                iRow++;
            }
            returnBody += " </ul>";

            returnBody += "<div class='tab-content mt-3' id='myTabContent'>";
            if (processType == 1)
            {
                foreach (var item in LangList())
                {
                    showActive = qRow == 1 ? "show active" : "";

                    returnBody += "<div class='tab-pane fade " + showActive + "' id='tab-lang-" + item.LangID + "' role='tabpanel' aria-labelledby='lang-tab-" + item.LangID + "'>" + item.LangName + "</div>";

                    qRow++;
                }
            }
            else if (processType == 2)
            {

            }
            returnBody += " </div>";

            return returnBody;

        }


        #endregion

        #region Sistem Getir
        public tblSystemSetting? SystemGet(int systemID)
        {
            return db.QueryApp<tblSystemSetting>(string.Format("SELECT * FROM tblSystemSetting WHERE SystemID=1")).FirstOrDefault();
        }
        #endregion

       

    }
}
