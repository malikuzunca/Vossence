using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static Vossence.DATA.Validation.Cls;
using Vossence.DATA.Table;

namespace Vossence.DATA.Helper
{
    public static class Mail
    {
        public static bool MailApp(List<MailTaskModel> model, tblMailSetting mailSetting)
        {
            bool rModel = false;
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;

            foreach (var task in model)
            {
                MailMessage mail = new MailMessage();
                SmtpClient sc = new SmtpClient();

                if (task.toMails != null)
                {
                    foreach (var item in task.toMails.Split(','))
                    {
                        if (item != null && item != "" && item != ",")
                            mail.To.Add(item);
                    }
                }

                if (mailSetting != null)
                {
                    if (mailSetting.Email != null && mailSetting.Host != null)
                    {
                        mail.From = new MailAddress(mailSetting.Email);
                        mail.Subject = task.mailCaption;
                        mail.IsBodyHtml = true;
                        mail.Body = task.mailContent;

                        sc.Port = Convert.ToInt32(mailSetting.Port);
                        sc.EnableSsl = Convert.ToBoolean(mailSetting.EnableSsl);
                        sc.Host = mailSetting.Host;
                        sc.UseDefaultCredentials = Convert.ToBoolean(mailSetting.UseDefaultCredentials);
                        sc.Credentials = new NetworkCredential(mailSetting.Email, mailSetting.Password);
                        sc.DeliveryMethod = SmtpDeliveryMethod.Network;
                    }
                }

                try
                {
                    if (mailSetting != null)
                    {
                        sc.Send(mail);
                        rModel = true;
                    }
                    else rModel = false;
                }
                catch (SmtpException ex)
                {
                    rModel = false;
                }
            }
            return rModel;
        }
    }
}
