using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace EMTop.Common.WeChatHelper
{
   
     
    public class ReturnCode
    {
        public int errcode { get; set; }
        public string errmsg { get; set; }
        public override string ToString()
        {
            return "{ \"errcode\":" + errcode + ",\"errmsg\":\"" + errmsg + "\"}";
        }
    }


    public class WeChatPushHelper
    {
        private static string Token;
        private static DateTime TokenTime;
        private string toUser;

        public WeChatPushHelper(string toUser)
        {
            this.toUser = toUser;
        }

        public void Push(WeChatContentBase content)
        {
            HttpWebRequest myrequest = PrepareRequest();

            string contentStr = content.GetContentStr();
            contentStr = contentStr.Replace("#openid#",toUser);
            byte[] postdata = Encoding.GetEncoding("UTF-8").GetBytes(contentStr);
            myrequest.ContentLength = postdata.Length;

            Stream newStream = myrequest.GetRequestStream();
            newStream.Write(postdata, 0, postdata.Length);
            newStream.Close();
            myrequest.BeginGetResponse(null, null);
        }


        private static HttpWebRequest PrepareRequest()
        {
            if (TokenTime == new DateTime() || TokenTime.AddSeconds(7140) <= DateTime.Now)
            {
                TokenTime = DateTime.Now;
                string accesstokenurl = "https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid=" + WeChatHelper.AppId + "&secret=" + WeChatHelper.AppSecret;
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(accesstokenurl);
                HttpWebResponse responst = (HttpWebResponse)request.GetResponse();
                StreamReader reader = new StreamReader(responst.GetResponseStream());
                string result = reader.ReadToEnd();
                string[] accessresult = result.Replace("\"", "").Split(',');
                string[] accesstoken = accessresult[0].Split(':');
                Token = accesstoken[1];
            }


            string posturl = "https://api.weixin.qq.com/cgi-bin/message/custom/send?access_token=" + Token;

            HttpWebRequest myrequest = (HttpWebRequest)HttpWebRequest.Create(posturl);
            myrequest.ContentType = "application/x-www-form-urlencoded";
            myrequest.Method = "POST";
            return myrequest;
        }
    }


    /// <summary>
    /// 主动推送
    /// </summary>
    public class WeChatNotifyHelper
    {
        WeChatPushNotify weChatPushNotify;

        public WeChatNotifyHelper(WeChatPushType pt)
        {
            switch (pt)
            {
                case WeChatPushType.News:
                    weChatPushNotify = new NewsWeChatPushNotify();
                    break;
                case WeChatPushType.Message:
                    weChatPushNotify = new MessagePushNotify();
                    break;
                default:
                    break;
            }
        }

        public void Push(object o)
        {
            weChatPushNotify.Push(o);
        }

    }

    /// <summary>
    /// 推送信息类型
    /// </summary>
    public enum WeChatPushType
    {
        News,
        Message,
        TemplateMessage
    }

    #region 微信推送方法
    /// <summary>
    /// 微信推送基类
    /// </summary>
    public abstract class WeChatPushNotify
    {
        protected void PushToWechat(string Content)
        {
            string accesstokenurl = "https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid=" + WeChatHelper.AppId + "&secret=" + WeChatHelper.AppSecret;
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(accesstokenurl);
            HttpWebResponse responst = (HttpWebResponse)request.GetResponse();
            StreamReader reader = new StreamReader(responst.GetResponseStream());
            string content = reader.ReadToEnd();
            string[] accessresult = content.Replace("\"", "").Split(',');
            string[] accesstoken = accessresult[0].Split(':');
            string mytoken = accesstoken[1];

            string posturl = "https://api.weixin.qq.com/cgi-bin/message/custom/send?access_token=" + mytoken;

            HttpWebRequest myrequest = (HttpWebRequest)HttpWebRequest.Create(posturl);
            myrequest.ContentType = "application/x-www-form-urlencoded";
            myrequest.Method = "POST";

            byte[] postdata = Encoding.GetEncoding("UTF-8").GetBytes(Content);
            myrequest.ContentLength = postdata.Length;

            Stream newStream = myrequest.GetRequestStream();
            newStream.Write(postdata, 0, postdata.Length);
            newStream.Close();

            myrequest.BeginGetResponse(null, null);
            //HttpWebResponse myResponse = (HttpWebResponse)myrequest.GetResponse();
            //StreamReader myreader = new StreamReader(myResponse.GetResponseStream(), Encoding.UTF8);
            //string result = reader.ReadToEnd();
        }

        public abstract void Push(object o);
    }

    /// <summary>
    /// 推送新闻
    /// </summary>
    public class NewsWeChatPushNotify : WeChatPushNotify
    {
        public override void Push(object o)
        {
            string toUser = o as string;
            string[] articles = new string[2];
            articles[0] = WeChatPushMessage.NewsArticleUnit("这一步校园招聘", "", "http://www.zheyibu.com", "");
            articles[1] = WeChatPushMessage.NewsArticleUnit("这一步", "", "", "");

            base.PushToWechat(WeChatPushMessage.NewsMessage(toUser, articles));
        }
    }


    /// <summary>
    /// 消息推送
    /// </summary>
    public class MessagePushNotify : WeChatPushNotify
    {
        public override void Push(object o)
        {
            Tuple<string, string> t = o as Tuple<string, string>;
            base.PushToWechat(WeChatPushMessage.TextMessage(t.Item1, t.Item2));
        }
    }
    #endregion

    #region 微信推送消息
    public static class WeChatPushMessage
    {
        public static string TextMessage(string toUser, string msg)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            sb.AppendFormat("\"touser\":\"{0}\",", toUser);
            sb.Append("\"msgtype\":\"text\",");
            sb.Append("\"text\":{");
            sb.AppendFormat("\"content\": \"{0}\"", msg);
            sb.Append("}");
            sb.Append("}");
            return sb.ToString();
        }

        public static string NewsMessage(string toUser, string[] articles)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            sb.AppendFormat("\"touser\":\"{0}\",", toUser);
            sb.Append("\"msgtype\":\"news\",");
            sb.Append("\"news\":{");
            sb.Append("\"articles\": [");
            for (int i = 0; i < articles.Count(); i++)
            {
                sb.Append(articles[i]);
                if (i != articles.Count() - 1)
                {
                    sb.Append(',');
                }
            }
            sb.Append("]");
            sb.Append("}");
            sb.Append("}");
            return sb.ToString();
        }

        public static string NewsArticleUnit(string title, string description, string url, string picUrl)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            sb.AppendFormat("\"title\":\"{0}\",", title);
            sb.AppendFormat("\"description\":\"{0}\",", description);
            sb.AppendFormat("\"url\":\"{0}\",", url);
            sb.AppendFormat("\"picurl\":\"{0}\"", picUrl);
            sb.Append("}");
            return sb.ToString();
        }
    }
    #endregion
}
