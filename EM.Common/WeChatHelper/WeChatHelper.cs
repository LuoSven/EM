using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;
using WeChat;

namespace EM.Common.WeChatHelper
{
    public class WeChatHelper
    {
        const string appId = "wxf8afee7a0ab49c31";
        const string appSecret = "b8ac522c780e9be131bba6ef0bbba1ec";
        const string token = "zhe1bu";

        public static string AppId
        {
            get { return appId; }
        }

        public static string AppSecret
        {
            get { return appSecret; }
        }

        public static string Token
        {
            get { return token; }
        }
    }
}
