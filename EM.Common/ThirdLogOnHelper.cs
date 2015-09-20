using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using NetDimension.Weibo;
using QConnectSDK.Context;
using RennDotSDK;

namespace EMTop.Common
{
    public class ThirdLogOnHelper
    {

        public static OAuth GetSinaOAuth()
        {
            string AppKey = ConfigurationManager.AppSettings["AppKey"];
            string AppSecrect = ConfigurationManager.AppSettings["AppSecrect"];
            string CallbackUrl = ConfigurationManager.AppSettings["CallbackUrl"];

            //CallbackUrl = "http://www.zheyibu.com/ztc/gallery";
            OAuth oauth = new OAuth(AppKey, AppSecrect, CallbackUrl);
            return oauth;
        }

        public static string GetSinaAuthorizeURL()
        {

            var oauth = ThirdLogOnHelper.GetSinaOAuth();
            return oauth.GetAuthorizeURL();
        }

        public static string GetTencentAuthorizeURL()
        {
            var context = new QzoneContext();
            string guidstate = Guid.NewGuid().ToString().Replace("-", "");

            string scope = "get_user_info,add_share,list_album,upload_pic,check_page_fans,add_t,add_pic_t,del_t,get_repost_list,get_info,get_other_info,get_fanslist,get_idolist,add_idol,del_idol,add_one_blog,add_topic,get_tenpay_addr";
            var authenticationUrl = context.GetAuthorizationUrl(guidstate, scope);
            return authenticationUrl;
        }

        public static string GetRenRenAuthorizationURL()
        {
            RennClient rr = new RennClient();
            return rr.GetAuthorizationURL();


        }

        /// <summary>
        /// 利用新浪微博的短链接API
        /// </summary>
        /// <param name="longUrl">正常非encode网址，包含http://</param>
        /// <returns></returns>
        public static string GetShortUrlBySinaApi(string longUrl)
        {
            NetDimension.Weibo.OAuth oauth = ThirdLogOnHelper.GetSinaOAuth();
            NetDimension.Weibo.Client Sina = new NetDimension.Weibo.Client(oauth);
            var shorturls = Sina.API.ShortUrl.Shorten(longUrl);
            var shortUrl = shorturls.FirstOrDefault();
            return shortUrl.ShortUrl;
        }

    }
}
