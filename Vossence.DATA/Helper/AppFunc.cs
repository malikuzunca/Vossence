using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Vossence.DATA.Table;

namespace Vossence.DATA.Helper
{
    public class AppFunc
    {
        #region Türkçe Karakter Değiştirme
        public static string TextLinkReturning(string? text, int langID = 0)
        {
            string returnText = "";
            if (!string.IsNullOrEmpty(text))
            {
                returnText = text.ToLower().Trim();
                returnText = returnText.Replace(" ", "-");
                returnText = returnText.Replace("+ ", "-");
                returnText = returnText.Replace(" +", "-");
                returnText = returnText.Replace("+", "-");
                returnText = returnText.Replace("ç", "c");
                returnText = returnText.Replace("ğ", "g");
                returnText = returnText.Replace("ı", "i");
                returnText = returnText.Replace("ö", "o");
                returnText = returnText.Replace("ä", "a");
                returnText = returnText.Replace("ş", "s");
                returnText = returnText.Replace("ü", "u");
                returnText = returnText.Replace("$", "");
                returnText = returnText.Replace(":", "-");
                returnText = returnText.Replace("=", "-");
                returnText = returnText.Replace(";", "-");
                returnText = returnText.Replace("%", "");
                returnText = returnText.Replace("&", "-");
                returnText = returnText.Replace(".", "-");
                returnText = returnText.Replace("?", "");
                returnText = returnText.Replace("'", "");
                returnText = returnText.Replace(",", "-");
                returnText = returnText.Replace("'", "");
                returnText = returnText.Replace("^", "");
                returnText = returnText.Replace("--", "-");
                returnText = returnText.Replace("*", "");
                returnText = returnText.Replace("<", "");
                returnText = returnText.Replace(">", "");
                returnText = returnText.Replace("İ", "i");
                returnText = returnText.Replace("#", "");
                //returnText = returnText.Replace("/", "-");
                returnText = returnText.Replace(@"\", "-");
                returnText = returnText.Replace("ä", "a");
                returnText = returnText.Replace("é", "e");
                returnText = returnText.Replace("--", "-");
                returnText = returnText.Replace("\"", "");
                returnText = returnText.Replace(" ", "-");
                if (langID != 0)
                    returnText = LetterConvert(returnText, langID);
                returnText = returnText.Replace("'", "");

            }
            return returnText;
        }
        #endregion

        #region Harf Convert
        public static string LetterConvert(string text, int langID)
        {
            if (langID == 3) // Rusça
            {
                string[] lat_up = { "A", "B", "V", "G", "D", "E", "Yo", "Zh", "Z", "I", "Y", "K", "L", "M", "N", "O", "P", "R", "S", "T", "U", "F", "Kh", "Ts", "Ch", "Sh", "Shch", "\"", "Y", "'", "E", "Yu", "Ya" };
                string[] lat_low = { "a", "b", "v", "g", "d", "e", "yo", "zh", "z", "i", "y", "k", "l", "m", "n", "o", "p", "r", "s", "t", "u", "f", "kh", "ts", "ch", "sh", "shch", "\"", "y", "'", "e", "yu", "ya" };
                string[] rus_up = { "А", "Б", "В", "Г", "Д", "Е", "Ё", "Ж", "З", "И", "Й", "К", "Л", "М", "Н", "О", "П", "Р", "С", "Т", "У", "Ф", "Х", "Ц", "Ч", "Ш", "Щ", "Ъ", "Ы", "Ь", "Э", "Ю", "Я" };
                string[] rus_low = { "а", "б", "в", "г", "д", "е", "ё", "ж", "з", "и", "й", "к", "л", "м", "н", "о", "п", "р", "с", "т", "у", "ф", "х", "ц", "ч", "ш", "щ", "ъ", "ы", "ь", "э", "ю", "я" };
                for (int i = 0; i <= 32; i++)
                {
                    text = text.Replace(rus_up[i], lat_up[i]);
                    text = text.Replace(rus_low[i], lat_low[i]);
                }
            }
            else if (langID == 6) // Arapça
            {
                Dictionary<char, string> arabicToLatinMap = new Dictionary<char, string>
                {
                    {'ا', "a"}, {'ب', "b"}, {'ت', "t"}, {'ث', "th"}, {'ج', "j"},
                    {'ح', "h"}, {'خ', "kh"}, {'د', "d"}, {'ذ', "dh"}, {'ر', "r"},
                    {'ز', "z"}, {'س', "s"}, {'ش', "sh"}, {'ص', "s"}, {'ض', "d"},
                    {'ط', "t"}, {'ظ', "z"}, {'ع', "ʿ"}, {'غ', "gh"}, {'ف', "f"},
                    {'ق', "q"}, {'ك', "k"}, {'ل', "l"}, {'م', "m"}, {'ن', "n"},
                    {'ه', "h"}, {'و', "w"}, {'ي', "y"}, {'ء', "'"}, {'أ', "a"},
                    {'إ', "i"}, {'ؤ', "w"}, {'ئ', "y"}, {'ى', "a"}, {'ة', "h"}
                };

                var result = new System.Text.StringBuilder();
                foreach (var c in text)
                {
                    result.Append(arabicToLatinMap.TryGetValue(c, out var latinChar) ? latinChar : c.ToString());
                }
                text = result.ToString();
            }

            text = text.Replace("û", "u");
            text = text.Replace("à", "a");
            text = text.Replace("é", "e");
            text = text.Replace("À", "a");

            text = text.Replace(" ", "-");
            text = text.Replace("+ ", "-");
            text = text.Replace(" +", "-");
            text = text.Replace("+", "-");
            text = text.Replace("ç", "c");
            text = text.Replace("ğ", "g");
            text = text.Replace("ı", "i");
            text = text.Replace("ö", "o");
            text = text.Replace("ä", "a");
            text = text.Replace("ş", "s");
            text = text.Replace("ü", "u");
            text = text.Replace("$", "");
            text = text.Replace(":", "-");
            text = text.Replace("=", "-");
            text = text.Replace(";", "-");
            text = text.Replace("%", "");
            text = text.Replace("&", "-");
            text = text.Replace(".", "-");
            text = text.Replace("?", "");
            text = text.Replace("'", "");
            text = text.Replace(",", "-");
            text = text.Replace("'", "");
            text = text.Replace("^", "");
            text = text.Replace("--", "-");
            text = text.Replace("*", "");
            text = text.Replace("<", "");
            text = text.Replace(">", "");
            text = text.Replace("İ", "i");
            text = text.Replace("#", "");
            text = text.Replace(@"\", "-");
            text = text.Replace("ä", "a");
            text = text.Replace("é", "e");
            text = text.Replace("--", "-");
            text = text.Replace("\"", "");
            text = text.Replace(" ", "-");
            text = text.Replace("'", "");
            text = text.Replace("ʿ", "");

            return text;
        }
        #endregion

        #region Kullanıcı Adı Oluştur
        public static string UsernameApp(string? email)
        {
            string userName = "";
            if (!string.IsNullOrEmpty(email))
            {
                Random rnd = new Random();
                userName = email.Split('@').First() + rnd.Next(10000, 99999);
            }
            return userName;
        }
        #endregion

        #region Tarih Çevir Tireli
        public static DateTime? ExtFormatDateTire(string? date)
        {
            DateTime dateApp = Convert.ToDateTime(date);

            string year = dateApp.Year.ToString();
            string month = dateApp.Month.ToString();
            string day = dateApp.Day.ToString();

            DateTime dateContact = Convert.ToDateTime(year + "-" + month + "-" + day);
            return dateContact;
        }
        #endregion

        #region SHA256Hash
        public static string SHA256Hash(object elementID)
        {
            using (SHA256 sha1Hash = SHA256.Create())
            {
                byte[] sourceBytes = Encoding.UTF8.GetBytes(elementID.ToString()!);
                byte[] hashBytes = sha1Hash.ComputeHash(sourceBytes);
                string hash = BitConverter.ToString(hashBytes).Replace("-", String.Empty);
                return hash;
            }
        }
        #endregion

        #region Permalink App
        public static string PermalinkApp(string permalink)
        {
            if (!string.IsNullOrEmpty(permalink))
                if (permalink.Length > 3)
                    if (permalink.Substring(0, 3).Contains("/"))
                        permalink = permalink.Remove(0, 3);
            return permalink != "" ? permalink : "";
        }
        #endregion

        #region Ürün Kodu Oluşturma
        public static string ProductCodeApp(string prdStart)
        {
            string productCode = "";
            if (!string.IsNullOrEmpty(prdStart))
            {
                Random rnd = new Random();
                productCode = prdStart + rnd.Next(10000, 99999);
            }
            return productCode;
        }
        #endregion
    }
}
