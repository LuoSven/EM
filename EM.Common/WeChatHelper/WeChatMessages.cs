using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EM.Common.WeChatHelper
{
    public abstract class WeChatContentBase
    {
        public abstract string GetContentStr();
    }


    public class TextContent : WeChatContentBase
    {
        public string content { get; set; }

        public override string GetContentStr()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            sb.Append("\"touser\":\"#openid#\",");
            sb.Append("\"msgtype\":\"text\",");
            sb.Append("\"text\":{");
            sb.AppendFormat("\"content\": \"{0}\"", content);
            sb.Append("}");
            sb.Append("}");
            return sb.ToString();
        }
    }

    public class PicContent : WeChatContentBase
    {
        public string media_id { get; set; }

        public override string GetContentStr()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            sb.Append("\"touser\":\"#openid#\",");
            sb.Append("\"msgtype\":\"text\",");
            sb.Append("\"text\":{");
            sb.AppendFormat("\"media_id\": \"{0}\"", media_id);
            sb.Append("}");
            sb.Append("}");
            return sb.ToString();
        }
    }

    public class PicTextContent : WeChatContentBase
    {
        public string title { get; set; }
        public string description { get; set; }
        public string url { get; set; }
        public string picurl { get; set; }

        public PicTextContent(string title, string description, string url, string picurl)
        {
            this.title = title;
            this.description= description;
            this.url = url;
            this.picurl = picurl;
        }

        public override string GetContentStr()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            sb.Append("\"touser\":\"#openid#\",");
            sb.Append("\"msgtype\":\"news\",");
            sb.Append("\"news\":{");
            sb.Append("\"articles\": [");

            sb.Append("{");
            sb.AppendFormat("\"title\":\"{0}\",", title);
            sb.AppendFormat("\"description\":\"{0}\",", description);
            sb.AppendFormat("\"url\":\"{0}\",", url);
            sb.AppendFormat("\"picurl\":\"{0}\"", picurl);
            sb.Append("}");

            sb.Append("]");
            sb.Append("}");
            sb.Append("}");
            return sb.ToString();
        }
    }

    public class VideoContent : WeChatContentBase
    {
        public string media_id { get; set; }
        public string title { get; set; }
        public string description { get; set; }

        public override string GetContentStr()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            sb.Append("\"touser\":\"#openid#\",");
            sb.Append("\"msgtype\":\"video\",");

            sb.Append("\"video\":{");
            sb.AppendFormat("\"media_id\":\"{0}\",", media_id);
            sb.AppendFormat("\"title\":\"{0}\",", title);
            sb.AppendFormat("\"description\":\"{0}\",", description);
            sb.Append("}");

            sb.Append("}");
            return sb.ToString();
        }
    }
}
