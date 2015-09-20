using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace EMTop.Common
{
    public static class PageHelper
    {
        ///// <summary>
        ///// 返回不为空的字符串
        ///// </summary>
        ///// <param name="str"></param>
        ///// <param name="strExt"></param>
        ///// <returns></returns>
        //public static string ChooseString(object str, object strExt, Components.ComponentType p_CT)
        //{
        //    if (str != null && !string.IsNullOrWhiteSpace(str.ToString()) && str.ToString() != "0")
        //    {
        //        return Components.GetText(p_CT, str.ToString());
        //    }
        //    else if (strExt != null)
        //    {
        //        return strExt.ToString();
        //    }
        //    return string.Empty;
        //}

        /// <summary>
        /// 返回不为空的字符串
        /// </summary>
        /// <param name="str"></param>
        /// <param name="strExt"></param>
        /// <returns></returns>
        public static string ChooseString(object str, object strExt)
        {
            if (str != null && !string.IsNullOrWhiteSpace(str.ToString()))
            {
                return str.ToString();
            }
            else if (strExt != null)
            {
                return strExt.ToString();
            }
            return string.Empty;
        }

        public static string GetTelephone(object strAreaCode, object strTelephone, object strExt)
        {
            string res = string.Format("{0}-{1}-{2}", strAreaCode, strTelephone, strExt);
            return res.Trim('-');
        }

        /// <summary>
        /// 根据生日获得岁数
        /// </summary>
        /// <param name="p_Birthday"></param>
        /// <returns></returns>
        public static string GetAge(object p_Birthday)
        {
            DateTime? dt = p_Birthday as DateTime?;
            if (dt.HasValue)
            {
                return (DateTime.Now.Year - dt.Value.Year).ToString();
            }
            return string.Empty;
        }
    }
}
