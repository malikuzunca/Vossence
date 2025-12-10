using Vossence.DATA.ORM;
using Vossence.DATA.Procedure;
using Vossence.DATA.Table;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;

namespace Vossence.ADMIN.Controllers
{
    public class AppJsonController : SharedController
    {
        private readonly IDapper db;
        private readonly IConfiguration? configuration;

        #region Ctor   
        public AppJsonController(IDapper dapper, IConfiguration? configuration, ILogger<SharedController> logger) : base(dapper, configuration, logger)
        {
            this.db = dapper;
            this.configuration = configuration;
        }
        #endregion

        #region Kullanıcı Bilgi Getir
        [Route("user-detail-get")]
        public async Task<JsonResult> JsonUserDetailGet(string userID, string userName, string userEmailAddress)
        {
            try
            {
                tblUserDetail? userGet = db.QueryApp<tblUserDetail>(string.Format("SELECT * FROM tblUserDetail WHERE UserID='{0}'", userID)).FirstOrDefault();
                if (userGet != null)
                    return await Task.FromResult(Json(new { userID = userGet.UserID, userName = userName, userEmailAddress = userEmailAddress, nameSurname = userGet.NameSurname, phoneNumber = userGet.PhoneNumber, }));
                return Json(null);
            }
            catch (Exception)
            {
                return await Task.FromResult(Json(null));
            }
        }
        #endregion

        #region Cms Item Getir
        [HttpGet]
        [Route("cms-item-get")]
        public async Task<JsonResult>? CmsItemGet(int cmsID)
        {
            try
            {
                tblCmsMain? cmsMain = db.QueryApp<tblCmsMain>(string.Format("SELECT * FROM tblCmsMain WHERE CmsID={0}", cmsID)).FirstOrDefault();
                List<tblCmsContent>? cmsContent = db.QueryApp<tblCmsContent>(string.Format("SELECT * FROM tblCmsContent WHERE CmsID={0}", cmsID)).ToList();

                return await Task.FromResult(Json(Tuple.Create(cmsMain, cmsContent)));
            }
            catch (Exception ex)
            {
                await Log(false, "CmsItemGet");
                return await Task.FromResult(Json(ex));
            }
        }
        #endregion

        #region SSS Kategori Getir
        [HttpGet]
        [Route("sss-category-get")]
        public async Task<JsonResult>? SSSCategoryGet(int sssCategoryID)
        {
            try
            {
                tblSSSCategoryMain? sssCategoryMain = db.QueryApp<tblSSSCategoryMain>(string.Format("SELECT * FROM tblSSSCategoryMain WHERE SSSCategoryID={0}", sssCategoryID)).FirstOrDefault();
                List<tblSSSCategoryContent>? sssCategoryContent = db.QueryApp<tblSSSCategoryContent>(string.Format("SELECT * FROM tblSSSCategoryContent WHERE SSSCategoryID={0}", sssCategoryID)).ToList();

                return await Task.FromResult(Json(Tuple.Create(sssCategoryMain, sssCategoryContent)));
            }
            catch (Exception ex)
            {
                await Log(false, "SSSCategoryGet");
                return await Task.FromResult(Json(ex));
            }
        }
        #endregion

        #region SSS Post Getir
        [HttpGet]
        [Route("sss-post-get")]
        public async Task<JsonResult>? SSSPostGet(int sssPostID)
        {
            try
            {
                tblSSSPostMain? sssPostMain = db.QueryApp<tblSSSPostMain>(string.Format("SELECT * FROM tblSSSPostMain WHERE SSSPostID={0}", sssPostID)).FirstOrDefault();
                List<tblSSSPostContent>? sssPostContent = db.QueryApp<tblSSSPostContent>(string.Format("SELECT * FROM tblSSSPostContent WHERE SSSPostID={0}", sssPostID)).ToList();

                return await Task.FromResult(Json(Tuple.Create(sssPostMain, sssPostContent)));
            }
            catch (Exception ex)
            {
                await Log(false, "SSSPostGet");
                return await Task.FromResult(Json(ex));
            }
        }
        #endregion

        #region Blog Kategori Getir
        [HttpGet]
        [Route("blog-category-get")]
        public async Task<JsonResult>? BlogCategoryGet(int blogCategoryID)
        {
            try
            {
                tblBlogCategoryMain? blogCategoryMain = db.QueryApp<tblBlogCategoryMain>(string.Format("SELECT * FROM tblBlogCategoryMain WHERE BlogCategoryID={0}", blogCategoryID)).FirstOrDefault();
                List<tblBlogCategoryContent>? blogCategoryContent = db.QueryApp<tblBlogCategoryContent>(string.Format("SELECT * FROM tblBlogCategoryContent WHERE BlogCategoryID={0}", blogCategoryID)).ToList();

                return await Task.FromResult(Json(Tuple.Create(blogCategoryMain, blogCategoryContent)));
            }
            catch (Exception ex)
            {
                await Log(false, "BlogCategoryGet");
                return await Task.FromResult(Json(ex));
            }
        }
        #endregion

        #region Blog Post Getir
        [HttpGet]
        [Route("blog-post-get")]
        public async Task<JsonResult>? BlogPostGet(int blogPostID)
        {
            try
            {
                tblBlogPostMain? blogPostMain = db.QueryApp<tblBlogPostMain>(string.Format("SELECT * FROM tblBlogPostMain WHERE BlogPostID={0}", blogPostID)).FirstOrDefault();
                List<tblBlogPostContent>? blogPostContent = db.QueryApp<tblBlogPostContent>(string.Format("SELECT * FROM tblBlogPostContent WHERE BlogPostID={0}", blogPostID)).ToList();

                return await Task.FromResult(Json(Tuple.Create(blogPostMain, blogPostContent)));
            }
            catch (Exception ex)
            {
                await Log(false, "BlogPostGet");
                return await Task.FromResult(Json(ex));
            }
        }
        #endregion

        #region Hizmet Getir
        [HttpGet]
        [Route("service-get")]
        public async Task<JsonResult>? ServiceGet(int serviceID)
        {
            try
            {
                tblServiceMain? serviceMain = db.QueryApp<tblServiceMain>(string.Format("SELECT * FROM tblServiceMain WHERE ServiceID={0}", serviceID)).FirstOrDefault();
                List<tblServiceContent>? serviceContent = db.QueryApp<tblServiceContent>(string.Format("SELECT * FROM tblServiceContent WHERE ServiceID={0}", serviceID)).ToList();

                return await Task.FromResult(Json(Tuple.Create(serviceMain, serviceContent)));
            }
            catch (Exception ex)
            {
                await Log(false, "ServiceGet");
                return await Task.FromResult(Json(ex));
            }
        }
        #endregion

        #region Kategori Getir

        [HttpGet]
        [Route("category-get")]
        public async Task<JsonResult>? CategoryGet(int categoryID)
        {
            try
            {
                tblCategoryMain? categoryMain = db.QueryApp<tblCategoryMain>(string.Format("SELECT * FROM tblCategoryMain WHERE CategoryID={0}", categoryID)).FirstOrDefault();
                List<tblCategoryContent>? categoryContent = db.QueryApp<tblCategoryContent>(string.Format("SELECT * FROM tblCategoryContent WHERE CategoryID={0}", categoryID)).ToList();
                tblCategoryMain? categorySubMain = db.QueryApp<tblCategoryMain>(string.Format("SELECT * FROM tblCategoryMain WHERE CategoryID={0}", categoryMain.CategorySubID)).FirstOrDefault();
                return await Task.FromResult(Json(Tuple.Create(categoryMain, categoryContent, categorySubMain)));
            }
            catch (Exception ex)
            {
                await Log(false, "CategoryGet");
                return await Task.FromResult(Json(ex));
            }
        }

        #endregion

        #region Ürün Kategori Getir
        [HttpGet]
        [Route("product-cat-get")]
        public async Task<JsonResult>? ProductCatGet(int categoryID)
        {
            try
            {
                List<tblCategoryMain>? categorySubMain = db.QueryApp<tblCategoryMain>(string.Format("SELECT * FROM tblCategoryMain WHERE CategorySubID={0}", categoryID)).ToList();
                return await Task.FromResult(Json(categorySubMain));
            }
            catch (Exception ex)
            {
                await Log(false, "ProductCatGet");
                return await Task.FromResult(Json(ex));
            }
        }
        #endregion

        #region Ürün Stock Getir
        [HttpGet]
        [Route("stock-get")]
        public async Task<JsonResult>? StockGet(int productID)
        {
            try
            {
                List<tblStock>? stockGet = db.QueryApp<tblStock>(string.Format("SELECT * FROM tblStock WHERE ProductID={0}", productID)).ToList();
                return await Task.FromResult(Json(stockGet));
            }
            catch (Exception ex)
            {
                await Log(false, "StockGet");
                return await Task.FromResult(Json(ex));
            }
        }
        #endregion

        #region Product Getir

        [HttpGet]
        [Route("product-get")]
        public async Task<JsonResult>? ProductGet(int productID)
        {
            try
            {

                tblProductMain? productMain = db.QueryApp<tblProductMain>(string.Format("SELECT * FROM tblProductMain WHERE ProductID={0}", productID)).FirstOrDefault();
                tblProductContent? productContent = db.QueryApp<tblProductContent>(string.Format("SELECT * FROM tblProductContent WHERE ProductID={0}", productID)).FirstOrDefault();
                List<tblTags> tagList = db.QueryApp<tblTags>(string.Format("SELECT T.TagID, T.TagName FROM tblProductMain PM LEFT JOIN tblProductTags PT ON PM.ProductID = PT.ProductID LEFT JOIN tblTags T ON PT.TagID = T.TagID WHERE PM.ProductID={0}", productMain.ProductID)).ToList();
                List<tblColors> colorList = db.QueryApp<tblColors>(string.Format("SELECT C.ColorID, C.ColorName FROM tblProductMain PM LEFT JOIN tblProductColors PC ON PM.ProductID = PC.ProductID LEFT JOIN tblColors C ON PC.ColorID = C.ColorID WHERE PM.ProductID={0}", productMain.ProductID)).ToList();
                List<tblStock> stockList = db.QueryApp<tblStock>(string.Format("SELECT * FROM tblStock WHERE ProductID={0}",productMain.ProductID)).ToList();
                return await Task.FromResult(Json(Tuple.Create(productMain, productContent, tagList, colorList, stockList /*categoryMain, categorySubMain, categorySubSubMain,*/ )));
            }
            catch (Exception ex)
            {
                await Log(false, "ProductGet");
                return await Task.FromResult(Json(ex));
            }
        }

        #endregion
    
        #region Slider Getir

        [HttpGet]

        [Route("slide-get")]
        public async Task<JsonResult>? SlideGet(int sliderID)
        {
            try
            {
                tblSliderMain? sliderMain = db.QueryApp<tblSliderMain>(string.Format("SELECT * FROM tblSliderMain WHERE SliderID={0}", sliderID)).FirstOrDefault();
                List<tblSliderContent>? sliderContent = db.QueryApp<tblSliderContent>(string.Format("SELECT * FROM tblSliderContent WHERE SliderID={0}", sliderID)).ToList();
                List<SP_Products?> productList = db.GetAll<SP_Products?>("SP_Products",new DynamicParameters(new Dictionary<string, object>{{ "@langID", langID }, {"@sliderID", sliderID}}));
                return await Task.FromResult(Json(Tuple.Create(sliderMain, sliderContent, productList)));
            }
            catch (Exception ex)
            {
                await Log(false, "SlideGet");
                return await Task.FromResult(Json(ex));
            }
        }
        #endregion

        #region Variyant Getir

        [HttpGet]
        [Route("variant-get")]
        public async Task<JsonResult>? VariantGet(int variantID, int variantType)
        {
            try
            {
                if (variantType == 1)
                {
                    SP_Variants? colorGet = db.GetAll<SP_Variants>("SP_Variants", new DynamicParameters(new Dictionary<string, object> { { "@variantType", 1 }, { "@variantID", variantID } })).FirstOrDefault();
                    if (colorGet != null)
                        return await Task.FromResult(Json(colorGet));

                }
                else if (variantType == 2)
                {
                    SP_Variants? tagGet = db.GetAll<SP_Variants>("SP_Variants", new DynamicParameters(new Dictionary<string, object> { { "@variantType", 2 }, { "@variantID", variantID } })).FirstOrDefault();
                    if(tagGet !=null)
                        return await Task.FromResult(Json(tagGet));

                }
                return await Task.FromResult(Json(null));
            }
            catch (Exception ex)
            {
                await Log(false, "VariantGet");
                return await Task.FromResult(Json(ex));
            }
        }

        #endregion
    }
}