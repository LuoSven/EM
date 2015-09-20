using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace EMTop.Common
{
    public class SmsHelper
    {
        public static string SendSMS(string mobile, string smsText)
        {
            string alertMsg = string.Empty;
            if (string.IsNullOrEmpty(mobile) || string.IsNullOrEmpty(smsText))
            {
                alertMsg = "短信发送失败，手机号码或短信内容不能为空。";
                return alertMsg;
            }

            using (SmsServiceReference.SmsServicePortTypeClient smsClient = new SmsServiceReference.SmsServicePortTypeClient())
            {

                string userName = "topuc";
                string password = "yxsl84h";
                string suffix = "这一步";

                string userName64 = Convert.ToBase64String(System.Text.ASCIIEncoding.Default.GetBytes(userName));
                string password64 = Convert.ToBase64String(System.Text.ASCIIEncoding.Default.GetBytes(password));
                string content64 = Convert.ToBase64String(System.Text.ASCIIEncoding.Default.GetBytes(smsText + "【" + suffix + "】"));
                //MobileSmsServiceReference.StatusDO sdo = smsClient.getStatus(userName64, password64);

                alertMsg = "";
                string ret = smsClient.send(userName64, password64, mobile, content64, "", "");
                switch (ret)
                {
                    case "100":
                        alertMsg = "发送成功";
                        break;
                    case "102":
                        alertMsg = "发送失败，返回代码：102, 密码错误！";
                        break;
                    case "103":
                        alertMsg = "发送失败，返回代码：103，欠费！";
                        break;
                    case "104":
                        alertMsg = "发送失败，返回代码：104";
                        break;
                }
                return ret;
            }

        }


        public static string SendSMSContainsSuffix(string mobile, string smsText)
        {
            string alertMsg = string.Empty;
            if (string.IsNullOrEmpty(mobile) || string.IsNullOrEmpty(smsText))
            {
                alertMsg = "短信发送失败，手机号码或短信内容不能为空。";
                return alertMsg;
            }

            using (SmsServiceReference.SmsServicePortTypeClient smsClient = new SmsServiceReference.SmsServicePortTypeClient())
            {

                string userName = "topuc";
                string password = "yxsl84h";

                string userName64 = Convert.ToBase64String(System.Text.ASCIIEncoding.Default.GetBytes(userName));
                string password64 = Convert.ToBase64String(System.Text.ASCIIEncoding.Default.GetBytes(password));
                string content64 = Convert.ToBase64String(System.Text.ASCIIEncoding.Default.GetBytes(smsText));
                //MobileSmsServiceReference.StatusDO sdo = smsClient.getStatus(userName64, password64);

                alertMsg = "";
                string ret = smsClient.send(userName64, password64, mobile, content64, "", "");
                switch (ret)
                {
                    case "100":
                        alertMsg = "发送成功";
                        break;
                    case "102":
                        alertMsg = "发送失败，返回代码：102, 密码错误！";
                        break;
                    case "103":
                        alertMsg = "发送失败，返回代码：103，欠费！";
                        break;
                    case "104":
                        alertMsg = "发送失败，返回代码：104";
                        break;
                }
                return ret;
            }

        }

        public static Task<string> SendSMSASync(string mobile, string smsText)
        {
            return Task.Run<string>(() =>
            {
                return SendSMS(mobile, smsText);
            });
        }

        public static Task<string> SendSMSASyncContainsSuffix(string mobile, string smsText)
        {
            return Task.Run<string>(() =>
            {
                return SendSMSContainsSuffix(mobile, smsText);
            });
        }

        public static void SendSmsBy29API(string smsHostUrl, string mobile, string smsText)
        {
            //smsHostUrl = "http://api.topuc.com/sms/";
            //mobile = "15855979065";
#if DEBUG
            smsHostUrl = "http://api.topuc.com/sms/";
            //mobile = "15855979065";
#endif
            if (string.IsNullOrEmpty(smsHostUrl) || string.IsNullOrEmpty(smsText))
                return;
            var uri = new Uri(new Uri(smsHostUrl), "?text=" + smsText + "&mobile=" + mobile);
            var httpClient = new HttpClient();
            httpClient.GetAsync(uri);
            //var responseJson = httpClient.GetAsync(uri).Result.Content.ReadAsStringAsync().Result;
            return;
        }

        public static string SendYPSms(string mobile, string text)
        {
            //地址
            string BASE_URI = "http://yunpian.com";
            //服务版本号
            string VERSION = "v1";
            //通用接口发短信的http地址
            string URI_SEND_SMS = BASE_URI + "/" + VERSION + "/sms/send.json";
            var ApiKey = "df1d8e5907e24edac658ecc233223540";

            if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(mobile))
            {
                //注意：参数必须进行Uri.EscapeDataString编码。以免&#%=等特殊符号无法正常提交
                string parameter = "apikey=" + ApiKey + "&text=" + Uri.EscapeDataString(text) + "&mobile=" + mobile;
                var extend = Guid.NewGuid().ToString();
                //if (!string.IsNullOrEmpty(extend) && Regex.IsMatch(extend, @"^[a-zA-Z0-9]*?$"))
                parameter += "&extend" + extend;
                System.Net.WebRequest req = System.Net.WebRequest.Create(URI_SEND_SMS);
                req.ContentType = "application/x-www-form-urlencoded";
                req.Method = "POST";
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(parameter);//这里编码设置为utf8
                req.ContentLength = bytes.Length;
                System.IO.Stream os = req.GetRequestStream();
                os.Write(bytes, 0, bytes.Length);
                os.Close();
                System.Net.WebResponse resp = req.GetResponse();
                if (resp != null)
                {
                    System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream());
                    var responseJson = sr.ReadToEnd();
                }
                return "";
            }
            return "短信发送失败，手机号码或短信内容不能为空。";
        }
    }
}
