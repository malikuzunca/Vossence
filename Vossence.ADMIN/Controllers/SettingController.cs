using Vossence.DATA.Helper;
using Vossence.DATA.Identity;
using Vossence.DATA.ORM;
using Vossence.DATA.Procedure;
using Vossence.DATA.Table;
using Dapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Collections.Generic;
using System.Diagnostics;
using static Vossence.DATA.Validation.Cls;

namespace Vossence.ADMIN.Controllers
{
    public class SettingController : SharedController
    {
        private readonly IDapper db;
        private readonly IConfiguration? configuration;
        private readonly UserManager<AppUser> userManager;
        private readonly RoleManager<AppRole> roleManager;
        private readonly SignInManager<AppUser> signManager;

        #region Ctor   
        public SettingController(IDapper dapper, IConfiguration? configuration, ILogger<SharedController> logger, UserManager<AppUser> userManager, SignInManager<AppUser> signManager, RoleManager<AppRole> roleManager) : base(dapper, configuration, logger)
        {
            this.db = dapper;
            this.configuration = configuration;
            this.userManager = userManager;
            this.signManager = signManager;
            this.roleManager = roleManager;
        }
        #endregion

        #region Sistem Ayarları
        [Route("system-setting")]
        public IActionResult SystemSetting()
        {
            tblSystemSetting? systemGet = db.QueryApp<tblSystemSetting>(string.Format("SELECT * FROM tblSystemSetting WHERE SystemID=1")).FirstOrDefault();
            if (systemGet != null)
                return View(Tuple.Create(systemGet,0));
            return Redirect("/404");
        }

        [Route("system-setting-update")]
        public async Task<ResultModel> SystemSettingUpdate(IFormCollection model)
        {
            try
            {
                tblSystemSetting? systemGet = db.QueryApp<tblSystemSetting>(string.Format("SELECT * FROM tblSystemSetting WHERE SystemID=1")).FirstOrDefault();
                if (systemGet != null)
                {
                    string logoApp = systemGet.LogoURL!;
                    if (model.Files.Count() > 0)
                        logoApp = FtpPost(Request.Form.Files[0], "Vossence/Default", "logo");

                    Task<ResultModel?> result = Task.FromResult(db.QueryApp<ResultModel>(string.Format("UPDATE tblSystemSetting SET BusinessName = '{0}', EmailAddress = '{1}', PhoneNumber = '{2}', WpNumber = '{3}', Adress = '{4}', MapURL = '{5}', ProductStart = '{6}', HeadCode = '{7}', BodyCode = '{8}', LogoURL='{9}', WorkTime='{10}' WHERE SystemID=1", model["businessName"], model["emailAddress"], model["phoneNumber"], model["wpNumber"], model["adress"], model["mapURL"], model["productStart"], model["headCode"], model["bodyCode"], logoApp, model["workTime"])).FirstOrDefault());

                    await Log(result.IsCompletedSuccessfully, "SystemSettingUpdate");
                    if (result.IsCompletedSuccessfully)
                        return await Task.FromResult(new ResultModel() { resultType = 1, resultCaption = "Başarı", resultMessage = "Güncelleme Başarılı" });
                    else
                        return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption ="Hata", resultMessage = "Bir Hata Oluştu. Lütfen sitem Yöneticisine Başvurunuz" });
                }
                else return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Uyarı", resultMessage = "Kayıt Bulunamadı" });
            }
            catch (Exception)
            {
                await Log(false, "SystemSettingUpdate");
                return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption ="Hata", resultMessage = "Bir Hata Oluştu. Lütfen sitem Yöneticisine Başvurunuz" });
            }
        }
        #endregion

        #region Kullanıcı

        #region Kullanıcı Listesi
        [Route("user-management")]
        public async Task<IActionResult> UserManagement(int loginType = -1)
        {
            List<UserList> model = new List<UserList>();
            foreach (var item in VirtualUserList(loginType).OrderBy(x => x.loginType).ToList())
            {
                AppUser userApp = new AppUser { UserName = item.userName, Email = item.email };

                model.Add(new UserList()
                {
                    userID = item.userID,
                    loginType = item.loginType,
                    userName = item.userName,
                    email = item.email,
                    nameSurname = item.nameSurname,
                    phone = item.phone,
                    lockoutEnabled = item.lockoutEnabled,
                    lockoutEnd = item.lockoutEnd
                });
            }

            return await Task.FromResult(View(Tuple.Create(model,"")));
        }
        #endregion

        #region Kullanıcı Oluştur
        [Route("user-create")]
        public async Task<ResultModel> UserCreate(IFormCollection model)
        {
            try
            {
                if (!string.IsNullOrEmpty(model["userEmailAddress"]) && !string.IsNullOrEmpty(model["userPassword"]))
                {
                    string? userEmailAddress = FormRowGet(model, "userEmailAddress");
                    string? userPassword = FormRowGet(model, "userPassword");
                    string? userNameSurname = FormRowGet(model, "userNameSurname");
                    string? userPhoneNumber = FormRowGet(model, "userPhoneNumber");
                    string? loginType = FormRowGet(model, "loginType");

                    Random rnd = new Random();
                    string username = "u_" + rnd.Next(10000, 99999);

                    AppUser userApp = new AppUser { UserName = username, Email = model["userEmailAddress"] };
                    IdentityResult identityResult = await userManager.CreateAsync(userApp, model["userPassword"]!);

                    if (identityResult.Succeeded)
                    {
                        AppUser? userGet = await userManager.FindByEmailAsync(model["userEmailAddress"]!);
                        
                        if (userGet != null)
                        {
                            string roleStart = loginType == "1" ? "Admin" : loginType == "2" ? "Son Kullanıcı" : "";
                            IdentityResult roleResult = await userManager.AddToRoleAsync(userApp, roleStart);
                            
                            Task<ResultModel> result = Task.FromResult(db.Insert<ResultModel>("SP_UserNextTableApp",
                                new DynamicParameters(
                                    new Dictionary<string, object?>
                                    {
                                        { "@userID", userGet.Id },
                                        { "@loginType", loginType },
                                        { "@nameSurname", userNameSurname },
                                        { "@emailAddress", userEmailAddress },
                                        { "@password", userPassword },
                                        { "@phoneNumber", userPhoneNumber }
                                    })));

                            await Log(result.IsCompletedSuccessfully, "UserCreate");
                            if (result.IsCompletedSuccessfully)
                                return await Task.FromResult(new ResultModel() { resultType = 1, resultCaption = "Başarılı", resultMessage = "Kullanıcı Oluşturma Başarılı" });
                            else
                                return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Hata", resultMessage = "Bir Hata Oluştu. Lütfen sitem Yöneticisine Başvurunuz" });
                        }
                        else return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Uyarı", resultMessage = "Kayıt Bulunamadı" });
                    }
                    else return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Hata", resultMessage = "Bir Hata Oluştu. Lütfen sitem Yöneticisine Başvurunuz" });
                }
                else return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Hata", resultMessage = "Bir Hata Oluştu. Lütfen sitem Yöneticisine Başvurunuz" });
            }
            catch (Exception)
            {
                await Log(false, "UserCreate");
                return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Hata", resultMessage = "Bir Hata Oluştu. Lütfen sitem Yöneticisine Başvurunuz" });
            }
        }
        #endregion

        #region Kullanıcı Kilitleme - Açma
        [Route("user-locked-change")]
        public async Task<ResultModel> UserIsLockedChange(string userID)
        {
            try
            {
                AppUser? user = userManager.FindByIdAsync(userID).Result;
                if (user != null)
                {
                    user.LockoutEnabled = user.LockoutEnabled ? false : true;
                    user.LockoutEnd = user.LockoutEnabled ? null : DateTime.UtcNow.AddDays(730);
                    await userManager.UpdateAsync(user);

                    await Log(true, "UserIsLockedChange");
                    return await Task.FromResult(new ResultModel() { resultType = 1, resultCaption = "Başarılı", resultMessage = "Güncelleme Başarılı" });
                }
                else return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Hata", resultMessage = "Bir Hata Oluştu. Lütfen sitem Yöneticisine Başvurunuz" });
            }
            catch (Exception)
            {
                await Log(false, "UserIsLockedChange");
                return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Hata", resultMessage = "Bir Hata Oluştu. Lütfen sitem Yöneticisine Başvurunuz" });
            }
        }
        #endregion

        #region Kullanıcı Şifre Değiştirme
        [Route("user-password-update")]
        public async Task<ResultModel> PasswordChange(string passwordEmail, string passwordReset)
        {
            try
            {
                AppUser? user = userManager.FindByEmailAsync(passwordEmail).Result;
                if (user != null)
                {
                    var token = await userManager.GeneratePasswordResetTokenAsync(user);
                    IdentityResult result = await userManager.ResetPasswordAsync(user, token, passwordReset);

                    if (result.Succeeded)
                    {
                        await Log(true, "PasswordChange");
                        return await Task.FromResult(new ResultModel() { resultType = 1, resultCaption = "Başarılı", resultMessage = "Güncelleme Başarılı" });
                    }
                    else return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption ="Hata", resultMessage = "Bir Hata Oluştu. Lütfen sitem Yöneticisine Başvurunuz" });
                }
                else return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Uyarı", resultMessage = "Kayıt Bulunamadı" });
            }
            catch (Exception)
            {
                await Log(false, "PasswordChange");
                return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption ="Hata", resultMessage = "Bir Hata Oluştu. Lütfen sitem Yöneticisine Başvurunuz" });
            }
        }
        #endregion

        #region Kullanıcı Bilgi Güncelle
        [Route("user-detail-update")]
        public async Task<ResultModel> UserDetailUpdate(IFormCollection model)
        {
            try
            {
                tblUserDetail? userGet = db.QueryApp<tblUserDetail>(string.Format("SELECT * FROM tblUserDetail WHERE UserID='{0}'", FormRowGet(model, "userID"))).FirstOrDefault();
                if (userGet != null)
                {
                    Task<ResultModel> result = Task.FromResult(db.Update<ResultModel>("SP_UserDetailUpdate",
                        new DynamicParameters(
                            new Dictionary<string, object?>
                            {
                                { "@userID", FormRowGet(model, "userID") },
                                { "@nameSurname", FormRowGet(model, "nameSurname") },
                                { "@phoneNumber", FormRowGet(model, "phoneNumber") }
                            })));

                    await Log(result.IsCompletedSuccessfully, "UserDetailUpdate");
                    if (result.IsCompletedSuccessfully)
                        return await Task.FromResult(new ResultModel() { resultType = 1, resultCaption = "Başarılı", resultMessage = "Güncelleme Başarılı" });
                    else
                        return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption ="Hata", resultMessage = "Bir Hata Oluştu. Lütfen sitem Yöneticisine Başvurunuz" });
                }
                else return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Uyarı", resultMessage = "Kayıt Bulunamadı" });
            }
            catch (Exception)
            {
                await Log(false, "UserDetailUpdate");
                return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption ="Hata", resultMessage = "Bir Hata Oluştu. Lütfen sitem Yöneticisine Başvurunuz" });
            }
        }
        #endregion

        #region Kullanıcı Sil
        [Route("user-delete")]
        public async Task<ResultModel> UserDelete(string userID)
        {
            try
            {
                var userGet = await userManager.FindByIdAsync(userID);
                if (userGet != null)
                {
                    var result = await userManager.DeleteAsync(userGet);
                    if (result.Succeeded)
                    {
                        await Log(true, "UserDelete");
                        return await Task.FromResult(new ResultModel() { resultType = 1, resultCaption = "Başarılı", resultMessage = "Kullanıcı Silindi" });
                    }
                    else return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption ="Hata", resultMessage = "Bir Hata Oluştu. Lütfen sitem Yöneticisine Başvurunuz" });
                }
                else return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Uyarı", resultMessage = "Kayıt Bulunamadı" });
            }
            catch (Exception)
            {
                await Log(false, "UserDelete");
                return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption ="Hata", resultMessage = "Bir Hata Oluştu. Lütfen sitem Yöneticisine Başvurunuz" });
            }
        }
        #endregion

        #region Sanal Tablo

        #region Kullanıcı Listesi
        public IEnumerable<UserList> VirtualUserList(int loginType)
        {
            List<UserList> list = db.GetAll<UserList>("SP_VirtualUserList", new DynamicParameters(new Dictionary<string, object> { { "@loginType", loginType } }));
            return list;
        }
        #endregion

        #endregion

        #endregion

        #region Sosyal Medya

        #region Liste
        [Route("social-medias")]
        public IActionResult SocialMedias()
        {
            List<tblSocialMedia> list = db.QueryApp<tblSocialMedia>(string.Format("SELECT * FROM tblSocialMedia")).ToList();
            return View(Tuple.Create(list, ""));
        }
        #endregion

        #region Sosyal Medya Oluştur

        [Route("social-media-create")]
        public async Task<ResultModel> SocialMediaCreate(IFormCollection model)
        {
            try
            {
                string title = model["_title"]!;
                string link = model["_link"]!;
                string icon = model["_icon"]!;
                Task<tblSocialMedia?> socialMediaDB = Task.FromResult(db.QueryApp<tblSocialMedia>(string.Format("INSERT INTO tblSocialMedia(Title, Icon, Link) VALUES('{0}', '{1}', '{2}')", title, icon, link)).FirstOrDefault());
                   
                await Log(socialMediaDB.IsCompletedSuccessfully, "SocialMediaCreate");
                if (socialMediaDB.IsCompletedSuccessfully)
                    return await Task.FromResult(new ResultModel() { resultType = 1, resultCaption = "Başarılı", resultMessage = "Kayıt Başarılı" });
                else
                    return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Hata", resultMessage = "Bir Hata Oluştu. Lütfen sitem Yöneticisine Başvurunuz" });
            }
            catch (Exception)
            {
                await Log(false, "SocialMediaCreate");
                return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Hata", resultMessage = "Bir Hata Oluştu. Lütfen sitem Yöneticisine Başvurunuz" });
            }
        }

        #endregion

        #region Sosyal Medya Güncelle
        [Route("social-media-update")]
        public async Task<ResultModel> SocialMediaUpdate(IFormCollection model)
        {
            try
            {
                tblSocialMedia? socialMediaGet = db.QueryApp<tblSocialMedia>(string.Format("SELECT * FROM tblSocialMedia WHERE SocialID='{0}'", FormRowGet(model, "socialID"))).FirstOrDefault();
                if (socialMediaGet != null)
                {
                    Task<ResultModel?> result = Task.FromResult(db.QueryApp<ResultModel>(string.Format("UPDATE tblSocialMedia SET Link='{0}' WHERE SocialID = {1}", FormRowGet(model, "link"), FormRowGet(model, "socialID"))).FirstOrDefault());

                    await Log(result.IsCompletedSuccessfully, "SocialMediaUpdate");
                    if (result.IsCompletedSuccessfully)
                        return await Task.FromResult(new ResultModel() { resultType = 1, resultCaption = "Başarılı", resultMessage ="Güncelleme Başarılı" });
                    else
                        return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Hata", resultMessage ="Bir Hata Oluştu. Lütfen sitem Yöneticisine Başvurunuz"});
                }
                else return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Uyarı", resultMessage = "Kayıt Bulunamadı." });
            }
            catch (Exception)
            {
                await Log(false, "SocialMediaUpdate");
                return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Hata", resultMessage ="Bir Hata Oluştu. Lütfen sitem Yöneticisine Başvurunuz"});
            }
        }
        #endregion

        #region Sosyal Medya Silme

        [Route("social-media-delete")]
        public async Task<ResultModel> SocialMediaDelete(int socialMediaID)
        {
            try
            {
                Task<ResultModel?> result = Task.FromResult(db.QueryApp<ResultModel>(string.Format("DELETE FROM tblSocialMedia WHERE SocialID = {0}", socialMediaID)).FirstOrDefault());

                await Log(result.IsCompletedSuccessfully, "SocialMediaDelete");
                if (result.IsCompletedSuccessfully)
                    return await Task.FromResult(new ResultModel() { resultType = 1, resultCaption = "Başarılı", resultMessage = "Silme Başarılı" });
                else
                    return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Hata", resultMessage = "Bir Hata Oluştu. Lütfen sitem Yöneticisine Başvurunuz" });
            }
            catch (Exception)
            {
                await Log(false, "SocialMediaDelete");
                return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Hata", resultMessage = "Bir Hata Oluştu. Lütfen sitem Yöneticisine Başvurunuz" });
            }
        }

        #endregion

        #endregion

        #region Slider

        #region Liste

        [Route("slide-management")]
        public IActionResult SlideManagement()
        {
            List<SP_Sliders> list = db.GetAll<SP_Sliders>("SP_Sliders",new DynamicParameters(new Dictionary<string, object>{{ "@langID", langID }}));
            List<SP_Products?> productList = db.GetAll<SP_Products?>("SP_Products",new DynamicParameters(new Dictionary<string, object>{{ "@langID", langID },{"@sliderID", -1}}));
            return View(Tuple.Create(list, productList));
        }

        #endregion

        #region Oluştur - Güncelle

        [Route("slide-app")]
        public async Task<ResultModel> SlideApp(LangItem[] list)
        {
            try
            {
                IFormCollection model = Request.Form;
                int sliderID = Convert.ToInt32(FormRowGet(model, "sliderID"));
                if (sliderID == 0)
                {
                    Task<ResultModel?> mainTable = Task.FromResult(db.QueryApp<ResultModel>(string.Format("INSERT INTO tblSliderMain (SliderName, IsActive, Deleted, Arrangement) VALUES ('{0}',{1},{2},{3})", FormRowGet(model, "sliderName").Replace("'", "''"), 0, 0, MainTableArrangement("Arrangement", "tblSliderMain").Result)).FirstOrDefault());
                    if (mainTable.IsCompletedSuccessfully)
                    {
                        if (list != null)
                        {
                            sliderID = MainTableMaxID("tblSliderMain").Result;
                            foreach (var item in list)
                            {
                                if (!string.IsNullOrEmpty(item.caption))
                                {
                                    string groupID = "Slider_" + sliderID;
                                    string url = AppFunc.TextLinkReturning(item.url);
                                    string? caption = !string.IsNullOrEmpty(item.caption) ? item.caption.Replace("'", "''") : item.caption;
                                    string? content = !string.IsNullOrEmpty(item.content) ? item.content.Replace("'", "''") : item.content;

                                    Task<ResultModel?> contentTable = Task.FromResult(db.QueryApp<ResultModel>(string.Format("INSERT INTO tblSliderContent (SliderID, LangID, SlideTitle, SlideText, ButtonText, URL) VALUES ({0}, {1}, '{2}', '{3}', '{4}', '{5}')", sliderID, item.langID, caption, item.title, item.description, url)).FirstOrDefault());

                                    LinkApp(1, url, item.langID, "Page", "Campaign", groupID, FormRowGet(model, "sliderName"));
                                }
                            }
                            var products = model.ToList();
                            foreach (var item in products)
                            {
                                if (item.Key == "selectedValues[]")
                                {
                                    var prds = item.Value.ToList();
                                    if (prds != null)
                                    {
                                        foreach (var prd in prds)
                                        {
                                            tblProductMain? productGet = db.QueryApp<tblProductMain>(string.Format("SELECT * FROM tblProductMain WHERE ProductID = {0}", prd)).FirstOrDefault();
                                            if (productGet != null)
                                            {
                                                Task<ResultModel?> result = Task.FromResult(db.QueryApp<ResultModel>(string.Format("UPDATE tblProductMain SET SliderID = {0} WHERE ProductID = {1}", sliderID, productGet.ProductID)).FirstOrDefault());
                                            }
                                        }
                                    }
                                }
                            }
                            return await Task.FromResult(new ResultModel() { resultType = 1, resultCaption = "Başarılı", resultMessage = "Kategori başarıyla oluşturuldu." });
                        }
                        else return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Hata", resultMessage = "Kategori içeriği boş olamaz." });
                    }
                    else return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Hata", resultMessage = "Kategori oluşturulurken bir hata oluştu." });
                }
                else if (sliderID > 0)
                {                    
                    tblSliderMain? sliderGet = db.QueryApp<tblSliderMain>(string.Format("SELECT * FROM tblSliderMain WHERE SliderID = {0}", sliderID)).FirstOrDefault();
                    if (sliderGet != null)
                    {
                        Task<ResultModel?> mainTable = Task.FromResult(db.QueryApp<ResultModel>(string.Format("UPDATE tblSliderMain SET SliderName='{0}' WHERE SliderID={1}", FormRowGet(model, "sliderName"), sliderID)).FirstOrDefault());
                        if (mainTable.IsCompletedSuccessfully)
                        {
                            if (list != null)
                            {
                                foreach (var item in list)
                                {
                                    string? caption = !string.IsNullOrEmpty(item.caption) ? item.caption.Replace("'", "''") : item.caption;
                                    string? content = !string.IsNullOrEmpty(item.content) ? item.content.Replace("'", "''") : item.content;

                                    tblSliderContent? cnt = db.QueryApp<tblSliderContent>(string.Format("SELECT * FROM tblSliderContent WHERE SliderID={0} AND LangID={1}", sliderGet.SliderID, item.langID)).FirstOrDefault();

                                    if (cnt != null)
                                    {
                                        string groupID = "Slider_" + sliderID;
                                        string url = AppFunc.TextLinkReturning(item.url);
                                        LinkApp(2, "", item.langID, "Page", "Campaign", groupID, FormRowGet(model, "sliderName"));

                                        if (!string.IsNullOrEmpty(item.caption))
                                        {

                                            Task<ResultModel?> contentTable = Task.FromResult(db.QueryApp<ResultModel>(string.Format("UPDATE tblSliderContent SET SlideTitle='{0}', SlideText='{1}', ButtonText='{2}', URL='{3}' WHERE SliderID={4} AND LangID={5}", caption, item.title, item.description, item.url, sliderID, item.langID)).FirstOrDefault());
                                            LinkApp(1, url, item.langID, "Page", "Campaign", groupID, FormRowGet(model, "sliderName"));
                                        }
                                        else
                                        {
                                            Task<ResultModel?> deleteTable = Task.FromResult(db.QueryApp<ResultModel>(string.Format("DELETE FROM tblSliderContent WHERE SliderID={0} AND LangID={1}", sliderID, item.langID)).FirstOrDefault());
                                        }
                                    }
                                    else
                                    {
                                        if (!string.IsNullOrEmpty(item.caption))
                                        {
                                            string groupID = "Sliders_" + sliderID;
                                            string url = AppFunc.TextLinkReturning(item.url);
                                            Task<ResultModel?> contentTable = Task.FromResult(db.QueryApp<ResultModel>(string.Format("INSERT INTO tblSliderContent (SliderID, LangID, SlideTitle, SlideText, ButtonText, URL) VALUES ({0}, {1}, '{2}', '{3}', '{4}', '{5}')", sliderID, item.langID, caption, item.title, item.description, item.url)).FirstOrDefault());
                                            LinkApp(1, url, item.langID, "Page", "Campaign", groupID, FormRowGet(model, "sliderName"));
                                        }

                                    }
                                    var products = model.ToList();
                                    foreach (var prd1 in products)
                                    {
                                        if (prd1.Key == "_selectedValues[]")
                                        {
                                            var prds = prd1.Value.ToList();
                                            if (prds != null)
                                            {
                                                foreach (var prd in prds)
                                                {
                                                    tblProductMain? productGet = db.QueryApp<tblProductMain>(string.Format("SELECT * FROM tblProductMain WHERE ProductID = {0}", prd)).FirstOrDefault();
                                                    if (productGet != null)
                                                    {
                                                        Task<ResultModel?> result = Task.FromResult(db.QueryApp<ResultModel>(string.Format("UPDATE tblProductMain SET SliderID = {0} WHERE ProductID = {1}", sliderID, productGet.ProductID)).FirstOrDefault());
                                                    }
                                                }
                                            }
                                        }
                                    }

                                }
                                return await Task.FromResult(new ResultModel() { resultType = 1, resultCaption = "Başarılı", resultMessage = "Kategori başarıyla güncellendi." });
                            }
                            else return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Uyarı", resultMessage = "Kayıt Bulunamadı." });
                        }
                        else return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Hata", resultMessage = "Kategori güncellenirken bir hata oluştu." });
                    }
                    else return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Hata", resultMessage = "Kategori bulunamadı." });
                }
                else return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Uyarı", resultMessage = "Kayıt Bulunamadı" });
            }
            catch (Exception)
            {
                await Log(false, "SlideApp");
                return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Hata", resultMessage = "Bir hata oluştu lütfen sistem yöneticinize başvurun." });
            }
        }

        #endregion

        #region Durum ÖneÇıkan Güncelle - Sil

        [Route("slide-process")]

        [HttpPost]
        public async Task<ResultModel> SlideProcess(int processType, int sliderID)
        {
            try
            {
                tblSliderMain? sliderGet = db.QueryApp<tblSliderMain>(string.Format("SELECT * FROM tblSliderMain WHERE SliderID={0}", sliderID)).FirstOrDefault();
                if (sliderGet != null)
                {
                    if (processType == 1)
                    {
                        int isActive = sliderGet.IsActive == true ? 0 : 1;

                        Task<ResultModel?> processApp = Task.FromResult(db.QueryApp<ResultModel>(string.Format("UPDATE tblSliderMain SET IsActive= {1} WHERE SliderID={0}", sliderGet.SliderID, isActive)).FirstOrDefault());

                        await Log(processApp.IsCompletedSuccessfully, "SlideProcess");
                        if (processApp.IsCompletedSuccessfully)
                            return await Task.FromResult(new ResultModel() { resultType = 1, resultCaption = "Başarılı", resultMessage = "Başarılı Güncelleme" });
                        else
                            return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Uyarı", resultMessage = "Kayıt Bulunamadı" });
                    }
                    else if (processType == 2)
                    {
                        Task<ResultModel?> processApp = Task.FromResult(db.QueryApp<ResultModel>(string.Format("UPDATE tblSliderMain SET Deleted = 1 WHERE SliderID={0}", sliderGet.SliderID)).FirstOrDefault());
                        
                        await Log(processApp.IsCompletedSuccessfully, "SlideProcess");
                        if (processApp.IsCompletedSuccessfully)
                            return await Task.FromResult(new ResultModel() { resultType = 1, resultCaption = "Başarılı", resultMessage = "Başarılı Güncelleme" });
                        else
                            return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Uyarı", resultMessage = "Kayıt Bulunamadı" });
                    }
                    else if (processType == 3)
                    {
                        tblProductMain? productGet = db.QueryApp<tblProductMain>(string.Format("SELECT * FROM tblProductMain WHERE SliderID={0}", sliderGet.SliderID)).FirstOrDefault();
                        Task<ResultModel?> processApp = Task.FromResult(db.QueryApp<ResultModel>(string.Format("UPDATE tblProductMain SET SliderID = {0} WHERE ProductID = {1}", 0, productGet!.ProductID)).FirstOrDefault());

                        await Log(processApp.IsCompletedSuccessfully, "SlideProcess");
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
                await Log(false, "SlideProcess");
                return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Uyarı", resultMessage = "Kayıt Bulunamadı" });
            }
        }
        #endregion

        #region Sıralama Güncelle
        [Route("slide-arrangement-update")]
        public async Task<ResultModel> SlideArrangementUpdate(string arrangementBody)
        {
            try
            {
                if (!string.IsNullOrEmpty(arrangementBody))
                {
                    foreach (var item in arrangementBody.Split('_'))
                    {
                        if (item != "" && item != "_" && item != null)
                        {
                            int sliderID = Convert.ToInt32(item.Split(',').First());
                            int arrangement = Convert.ToInt32(item.Split(',').Last());

                            tblSliderMain? sliderGet = db.QueryApp<tblSliderMain>(string.Format("SELECT * FROM tblSliderMain WHERE SliderID={0}", sliderID)).FirstOrDefault();
                            if (sliderGet != null)
                            {
                                Task<ResultModel?> result = Task.FromResult(db.QueryApp<ResultModel>(string.Format("UPDATE tblSliderMain SET Arrangement={0} WHERE SliderID={1}", arrangement, sliderID)).FirstOrDefault());
                            }
                            else return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption ="Uyarı", resultMessage = "Kayıt Bulunamadı" });
                        }
                    }
                    await Log(true, "SlideArrangementUpdate");
                    return await Task.FromResult(new ResultModel() { resultType = 1, resultCaption = "Başarılı", resultMessage ="Başarılı Güncelleme" });
                }
                else return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption ="Uyarı", resultMessage = "Kayıt Bulunamadı" });
            }
            catch (Exception)
            {
                await Log(false, "SlideArrangementUpdate");
                return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Hata", resultMessage = "Bir hata oluştu lütfen sistem yöneticinize başvurun." });
            }
        }
        #endregion

        #region Görsel
        [Route("slide-image-app")]
        public async Task<ResultModel> SlideImageApp(int sliderID)
        {
            try
            {
                var files = Request.Form.Files;
                if (files.Count() > 0)
                {
                    string sliderImg = FtpPost(Request.Form.Files[0], "Vossence/Sliders", "slide");
                    if (sliderImg != "")
                    {
                        Task<ResultModel?> result = Task.FromResult(db.QueryApp<ResultModel>(string.Format("UPDATE tblSliderMain SET ImageURL='{0}' WHERE SliderID={1}", sliderImg, sliderID
                        )).FirstOrDefault());
                        await Log(true, "SlideImageApp");
                        if (result.IsCompletedSuccessfully)
                            return await Task.FromResult(new ResultModel() { resultType = 1, resultCaption = "Başarılı", resultMessage = "Güncelleme Başarılı" });
                        else return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Hata", resultMessage = "Bir hata oluştu lütfen sistem yöneticinize başvurun." });
                    }
                }
                await Log(true, "SlideImageApp");
                return await Task.FromResult(new ResultModel() { resultType = 1, resultCaption = "Başarılı", resultMessage = "Başarılı Güncelleme" });
            }
            catch (Exception)
            {
                await Log(false, "SlideImageApp");
                return await Task.FromResult(new ResultModel() { resultType = 0, resultCaption = "Hata", resultMessage = "Bir hata oluştu lütfen sistem yöneticinize başvurun." });
            }
        }
        #endregion

        #endregion

    }
}
