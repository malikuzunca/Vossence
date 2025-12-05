using Vossence.DATA.Identity;
using Vossence.DATA.ORM;
using Vossence.DATA.Table;
using Dapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static Vossence.DATA.Validation.Cls;

namespace Vossence.ADMIN.Controllers
{
    public class LoginController : Controller
    {
        private readonly IDapper db;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly SignInManager<AppUser> _signManager;

        #region Ctor
        public LoginController(IDapper dapper, UserManager<AppUser> userManager, SignInManager<AppUser> signManager, RoleManager<AppRole> roleManager)
        {
            this.db = dapper;
            this._userManager = userManager;
            this._signManager = signManager;
            this._roleManager = roleManager;
        }
        #endregion

        #region Kullanıcı Girişi
        [Route("app-login")]
        public IActionResult AdminLogin()
        {
            tblSystemSetting? systemGet = db.QueryApp<tblSystemSetting>("SELECT * FROM tblSystemSetting").FirstOrDefault();
            bool userCnt = UserControl(User!.Identity!.Name!);
            if (User.Identity.IsAuthenticated)
                if (userCnt) return Redirect("/");
            return View(systemGet);
        }

        [Route("app-login")]
        [HttpPost]
        public async Task<IActionResult> AdminLogin(string? sGroup, string sPassword, string rememberMe)
        {
            UserList? groupCnt = VirtualUserList(1).FirstOrDefault(x => x.email == sGroup);
            if (groupCnt == null) groupCnt = VirtualUserList(1).FirstOrDefault(x => x.userName == sGroup);
            if (groupCnt != null)
            {
                if (groupCnt.email != null)
                {
                    AppUser? userGet = await _userManager.FindByEmailAsync(groupCnt.email);
                    bool rMe = rememberMe == "on" ? true : false;

                    if (userGet != null)
                    {
                        await _signManager.SignOutAsync();
                        Microsoft.AspNetCore.Identity.SignInResult result = await _signManager.PasswordSignInAsync(userGet, sPassword, rMe, false);

                        if (userGet.LockoutEnabled == true)
                        {
                            if (result.Succeeded)
                                return Redirect("/");
                        }
                    }
                }
                return Redirect("/app-login");
            }
            else return Redirect("/app-login");
        }
        #endregion

        #region Lisans
        [Route("license-page")]
        public IActionResult LicensePage(int nrt)
        {
            tblSystemSetting? systemGet = db.QueryApp<tblSystemSetting>(string.Format("SELECT * FROM tblSystemSetting WHERE SystemID=1")).FirstOrDefault();
            if (systemGet != null)
                return View(systemGet);
            return Redirect("/app-login");
        }
        #endregion

        #region Yetkisiz Kullanım Sayfası
        [Route("access-denied")]
        public IActionResult AccessDenied()
        {
            tblSystemSetting? systemGet = db.QueryApp<tblSystemSetting>("SELECT * FROM tblSystemSetting").FirstOrDefault();
            return View(systemGet);
        }
        #endregion

        #region Kullanıcı Tip Kontrol
        public bool UserControl(string userName)
        {
            AspNetUsers? userGet = db.QueryApp<AspNetUsers>(string.Format("SELECT * FROM AspNetUsers WHERE UserName='{0}'", userName)).FirstOrDefault();
            if (userGet != null)
            {
                tblUserLoginType typeGet = db.Get<tblUserLoginType>("SP_UserDetailGet", new DynamicParameters(new Dictionary<string, object> { { "@userID", userGet.Id } }));
                if (typeGet != null)
                {
                    if (typeGet.LoginType == 1)
                    {
                        if (userGet.LockoutEnabled == true)
                            return true;
                    };
                }
            }
            return false;
        }
        #endregion

        #region Kullanıcı Çıkış Yap
        [Route("logout")]
        public ActionResult LogOut()
        {
            _signManager.SignOutAsync();
            return Redirect("/app-login");
        }
        #endregion

        #region Sanal Tablo

        #region Kullanıcı Listesi
        public List<UserList> VirtualUserList(int loginType)
        {
            return db.GetAll<UserList>("SP_VirtualUserList", new DynamicParameters(new Dictionary<string, object> { { "@loginType", loginType } }));
        }
        #endregion

        #endregion


    }
}
