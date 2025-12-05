using Vossence.DATA.Helper;
using Vossence.DATA.ORM;
using Vossence.DATA.Table;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using static Vossence.DATA.Validation.Cls;

namespace Vossence.ADMIN.Controllers
{
    public class HomeController : SharedController
    {

        private readonly IDapper db;
        private readonly IConfiguration? configuration;
        private readonly ILogger<SharedController> logger;

        #region Ctor   
        public HomeController(IDapper dapper, IConfiguration? configuration, ILogger<SharedController> logger) : base(dapper, configuration, logger)
        {
            this.db = dapper;
            this.configuration = configuration;
            this.logger = logger;
        }
        #endregion

        #region Index
        public IActionResult Index()
        {
            IndexModel model = new IndexModel()
            {
                countService = db.QueryApp<tblServiceMain>("SELECT * FROM tblServiceMain WHERE Deleted=0").Count(),
                countBlogCategory = db.QueryApp<tblBlogCategoryMain>("SELECT * FROM tblBlogCategoryMain WHERE Deleted=0").Count(),
                countSSSCategory = db.QueryApp<tblSSSCategoryMain>("SELECT * FROM tblSSSCategoryMain WHERE Deleted=0").Count(),
                countBlogPost = db.QueryApp<tblBlogPostMain>("SELECT * FROM tblBlogPostMain WHERE Deleted=0").Count(),
                countSSSPost = db.QueryApp<tblSSSPostMain>("SELECT * FROM tblSSSPostMain WHERE Deleted=0").Count(),
                countSlider = db.QueryApp<tblSliderMain>("SELECT * FROM tblSliderMain WHERE Deleted=0").Count(),
                countCategory = db.QueryApp<tblCategoryMain>("SELECT * FROM tblCategoryMain WHERE Deleted=0").Count(),
                countProduct = db.QueryApp<tblProductMain>("SELECT * FROM tblProductMain WHERE Deleted=0").Count(),
                countCms = db.QueryApp<tblCmsMain>("SELECT * FROM tblCmsMain WHERE Deleted=0").Count(),
                
            };

            return View(model);
        }
        #endregion

        #region Hata Sayfas�
        [Route("404")]
        public IActionResult ErrorPage()
        {
            return View();
        }
        #endregion

        #region URL Getir
        [Route("url-return")]
        public string UrlReturn(string link, int lang, int sType, string pathURL = "")
        {
            link = AppFunc.TextLinkReturning(link, lang);
            tblLanguage? languageGet = db.QueryApp<tblLanguage>(string.Format("SELECT * FROM tblLanguage WHERE LangID={0}", langID)).FirstOrDefault();
            if (languageGet != null)
            {
                bool linkResult = sType == 1 ? LinkControl(link, languageGet.LangID, 1, pathURL) : LinkControl(link, languageGet.LangID, 2, pathURL);
                if (linkResult)
                    return link;
            }
            return "";
        }
        #endregion

        #region Link Kay�t Kontrol�
        public bool LinkControl(string link, int langID, int sType, string pathURL)
        {
            string[] links = new string[] { "item-management", "sss-categories", "sss-posts", "blog-categories", "blog-posts", "service-management" };
            if (links.FirstOrDefault(x => x == pathURL) != null)
            {
                if (sType == 1)
                    return db.QueryApp<tblLink>(string.Format("SELECT * FROM tblLink WHERE Deleted=0 AND LangID={0} AND Link='{1}'", langID, link)).FirstOrDefault() == null ? true : false;
                else if (sType == 2)


                    //return db.QueryApp<tblLink>($"SELECT * FROM tblLink WHERE Deleted=0 AND LangID={langID} AND Link='{link}'").Count() > 0 ? true : false;


                return db.QueryApp<tblLink>(string.Format("SELECT * FROM tblLink WHERE Deleted=0 AND LangID={0} AND Link='{1}'}", langID, link)).Count() > 0 ? true : false;
                else return false;
            }
            else
            {
                if (sType == 1)
                    return db.QueryApp<tblLink>(string.Format("SELECT * FROM tblLink WHERE Deleted=0 AND LangID={0} AND Link='{1}'", langID, link)).FirstOrDefault() == null ? true : false;
                else if (sType == 2)
                    return db.QueryApp<tblLink>(string.Format("SELECT * FROM tblLink WHERE Deleted=0 AND LangID={0} AND Link='{1}'", langID, link)).Count() > 0 ? true : false;
                else return false;
            }
        }
        #endregion


    }
}
