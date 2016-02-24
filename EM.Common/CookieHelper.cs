using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;

namespace EM.Common
{
    public class CookieHelper
    {
        public CookieHelper()
        {
            //
        }


        /// <summary>
        /// 清除指定Cookie的所有子项
        /// </summary>
        /// <param name="p_CookieName">Cookie名字</param>
        public static void ClearCookieItems(string p_CookieName)
        {
            if (HttpContext.Current.Request.Cookies[p_CookieName] != null)
            {
                HttpCookie cookie = HttpContext.Current.Request.Cookies[p_CookieName];
                cookie.Values.Clear();
                HttpContext.Current.Response.Cookies.Set(cookie);
            }
        }

        ///<summary>
        /// Write cookie value
        /// </summary>
        /// <param name="strName">cookie name</param>
        /// <param name="strValue">cookie value</param>
        public static void WriteCookie(string cookieName, string cookieValue, bool rememberMe, int expiresDays=1)
        {
            if (string.IsNullOrEmpty(cookieName)) return;
            //删除旧的同名Cookie
            HttpContext.Current.Response.Cookies.Remove(cookieName);
            var cookie = new HttpCookie(cookieName);
                cookie.Domain = HttpContext.Current.Request.Url.Host;
                cookie.Secure = false;
                cookie.Value = cookieValue;
                cookie.Expires = DateTime.Now.AddDays(expiresDays);
                if (rememberMe)
                {
                    if (expiresDays == 0)
                        expiresDays = 365; //Remember me forever
                    cookie.Expires = DateTime.Now.AddDays(expiresDays);
                }
            HttpContext.Current.Response.AppendCookie(cookie);
        }

        /// <summary>
        /// Read cookie
        /// </summary>
        /// <param name="strName">cookie name</param>
        /// <returns>cookie value</returns>
        public static string GetCookie(string cookieName)
        {
            if (HttpContext.Current.Request.Cookies != null && HttpContext.Current.Request.Cookies[cookieName] != null)
            {
                return HttpContext.Current.Request.Cookies[cookieName].Value.ToString();
            }
            return string.Empty;
        }

        /// <summary>
        /// Cookies
        /// </summary>
        /// <param name="strName">cookie name</param>
        public static void DeleteCookie(string strName)
        {
            if (HttpContext.Current.Request.Cookies[strName] != null)
            {
                HttpCookie cookie = HttpContext.Current.Request.Cookies[strName];
                cookie.Values.Clear();
                cookie.Expires = DateTime.Now.AddDays(-1);
                cookie.Domain = HttpContext.Current.Request.Url.Host;
                cookie.Secure = false;
                HttpContext.Current.Response.Cookies.Set(cookie);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cookieName">名称</param>
        /// <param name="cookieValue">值</param>
        /// <param name="expires">过期时间</param>
        public static void WriteCookie(string cookieName, string cookieValue, DateTime? expires)
        {
            HttpCookie cookie;
            if (HttpContext.Current.Request.Cookies[cookieName] != null)
            {
                cookie = HttpContext.Current.Request.Cookies[cookieName];
            }
            else
            {
                cookie = new HttpCookie(cookieName);
            }

            string ConfigVersion = ConfigurationManager.AppSettings["ConfigVersion"];
            if (ConfigVersion == "Release")
            {
                cookie.Domain = ".zheyibu.com";
                cookie.Secure = false;
            }
            if (ConfigVersion == "Debug")
            {
                cookie.Domain = ".test.com";
                cookie.Secure = false;
            }
            if (ConfigVersion == "Test")
            {
                cookie.Domain = ".beta.com";
                cookie.Secure = false;
            }

            cookie.Value = HttpUtility.UrlEncode(cookieValue, Encoding.GetEncoding("UTF-8"));

            if (expires.HasValue)
            {
                cookie.Expires = expires.Value;
            }
            else
            {
                cookie.Expires = DateTime.MaxValue;
            }
            if (HttpContext.Current.Request.Cookies[cookieName] != null)
            {
                HttpContext.Current.Response.Cookies.Set(cookie);
            }
            else
            {
                HttpContext.Current.Response.Cookies.Add(cookie);
            }
        }
    }
}