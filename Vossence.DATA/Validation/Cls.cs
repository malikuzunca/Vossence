using Vossence.DATA.Procedure;
using Vossence.DATA.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vossence.DATA.Validation
{
    public class Cls
    {
        #region Sonuç Dönüş Model
        public class ResultModel
        {
            public int resultType { get; set; }
            public string resultCaption { get; set; }
            public string resultMessage { get; set; }
            public string resultClass { get; set; }
        }
        #endregion

        #region Kullanıcı
        public class UserList
        {
            public string? userID { get; set; }
            public int loginType { get; set; }
            public string? userName { get; set; }
            public string? email { get; set; }
            public string? nameSurname { get; set; }
            public string? phone { get; set; }
            public string? company { get; set; }
            public DateTime registerDate { get; set; }
            public bool? lockoutEnabled { get; set; }
            public DateTimeOffset? lockoutEnd { get; set; }
        }
        public class UserGetModel
        {
            public string userID { get; set; }
            public string userName { get; set; }
            public string userEmailAddress { get; set; }
            public string nameSurname { get; set; }
            public string phoneNumber { get; set; }
            public string? company { get; set; }
        }

        public class AccountUpdateModel
        {
            public string nameSurname { get; set; }
            public string emailAddress { get; set; }
            public string phoneNumber { get; set; }
            public string company { get; set; }
        }

        public class PasswordUpdateModel
        {
            public string emailAddress { get; set; }
            public string newPassword { get; set; }
            public string confirmPassword { get; set; }
        }
        #endregion

        #region Mail
        public class MailTaskModel
        {
            public string? toMails { get; set; }
            public string? mailCaption { get; set; }
            public string? mailContent { get; set; }
        }

        public class MailSmtpSettingModel
        {
            public int mailSettingID { get; set; }
            public string? mailAddress { get; set; }
            public string? password { get; set; }
            public string? host { get; set; }
            public int? port { get; set; }
            public int? ssl { get; set; }
            public int? use { get; set; }
        }
        #endregion

        #region Dil Tab Model
        public class LangItem
        {
            public int langID { get; set; }
            public string? caption { get; set; }
            public string? shortText { get; set; }
            public string? content { get; set; }
            public string? title { get; set; }
            public string? description { get; set; }
            public string? prdCode { get; set; }
            public string? url { get; set; }
        }
        #endregion

        #region Layout Model
        public class LayoutData
        {
            public tblSystemSetting? systemSetting { get; set; }
            public string? langCode { get; set; }
            public string? langName { get; set; }
            public string? linkSet { get; set; }
            public List<SP_WebCmsItems> headerItems { get; set; }
            public List<SP_WebCmsItems> corporateItems { get; set; }
            public List<SP_WebCmsItems> supportItems { get; set; }
            public List<tblSocialMedia> socialMedias { get; set; }
        }
        #endregion

        #region Web Index Model
        public class WebIndexModel
        {
            public tblLanguageContent? languageContent { get; set; }
            public List<SP_WebCmsItems>? serviceMain { get; set; }
            public List<SP_WebServices>? services { get; set; }
            public List<SP_WebProducts>? hardwares { get; set; }
            public List<SP_WebCategories>? categories { get; set; }
            public List<SP_WebCategories>? categoriesHomePage { get; set; }
            public List<SP_WebSliders>? sliders { get; set; }

        }
        #endregion

        #region IndexModel

        public class IndexModel
        {
            public int countUser { get; set; }
            public int countService { get; set; }
            public int countProduct { get; set; }
            public int countCategory { get; set; }
            public int countSlider { get; set; }
            public int countBlogCategory { get; set; }
            public int countBlogPost { get; set; }
            public int countSSSCategory { get; set; }
            public int countSSSPost { get; set; }
            public int countCms { get; set; }
        }
        #endregion
    }
}