using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;


namespace EM.Utils
{
    public class HtmlUtil
    {
        public static string StrFormat(object obj)
        {
            if (obj == null)
            {
                return string.Empty;
            }
            return StrFormat(obj.ToString());
        }

        /// <summary>
        /// 忽略所有的回车换行符
        /// </summary>
        public static string StrFormat(string str)
        {
            string str2 = string.Empty;

            if (!string.IsNullOrWhiteSpace(str))
            {
                str = str.Replace("\r\n", "");
                str = str.Replace("\n", "");
                str = str.Replace("<p>", string.Empty);
                str = str.Replace("</p>", string.Empty);
                str2 = str;
            }
            return str2;
        }

        /// <summary>
        /// 改正sql语句中的转义字符
        /// </summary>
        public static string mashSQL(string str)
        {
            string str2 = string.Empty;

            if (!string.IsNullOrWhiteSpace(str))
            {
                str = str.Replace("\'", "'");
                str2 = str;
            }
            return str2;
        }

        /// <summary>
        /// 替换sql语句中的有问题符号
        /// </summary>
        public static string ChkSQL(string str)
        {
            string str2 = string.Empty;

            if (!string.IsNullOrWhiteSpace(str))
            {
                str = str.Replace("'", "''");
                str2 = str;
            }
            return str2;
        }


        /// 替换html字符
        /// </summary>
        public static string EncodeHtml(string strHtml)
        {
            if (!string.IsNullOrWhiteSpace(strHtml))
            {
                strHtml = strHtml.Replace(",", "&def;");
                strHtml = strHtml.Replace("'", "&dot;");
                strHtml = strHtml.Replace("\"", "&quot;");
                strHtml = strHtml.Replace(";", "&dec;");
                strHtml = strHtml.Replace("<", "&lt;");
                strHtml = strHtml.Replace(">", "&gt;");
                strHtml = strHtml.Replace(" ", "&nbsp;");
                return strHtml;
            }
            return string.Empty; ;
        }

        /// <summary>
        /// 为脚本替换特殊字符串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ReplaceStrToScript(string str)
        {
            if (str == string.Empty) return string.Empty;
            str = str.Replace("\\", "\\\\");
            str = str.Replace("'", "\\'");
            str = str.Replace("\"", "\\\"");
            return str;
        }

        /// <summary>
        /// 移除Html标记
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string RemoveHtml(string content)
        {
            string regexstr = @"<[^>]*>";
            return EncodeHtml(Regex.Replace(content, regexstr, string.Empty, RegexOptions.IgnoreCase));
        }


        public static string RemoveBBSHtml(string content)
        {
            string result = "";
            string regexstr = @"<[^>]*>";
            result = Regex.Replace(content, regexstr, string.Empty, RegexOptions.IgnoreCase);
            regexstr =@"\[[^\]]*\]";
            result = Regex.Replace(result, regexstr, string.Empty, RegexOptions.IgnoreCase);

            return result;
        }

        // <summary>将HTML内容转换成纯文本形式，即去除HTML格式
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string ConvertToText(string source)
        {
            try
            {
                string result;            //remove line breaks,tabs
                result = source.Replace("\r", " ");
                result = result.Replace("\n", " ");
                result = result.Replace("\t", " ");
                //remove the header
                result = Regex.Replace(result, "(<head>).*(</head>)", string.Empty, RegexOptions.IgnoreCase);
                result = Regex.Replace(result, @"<( )*script([^>])*>", "<script>", RegexOptions.IgnoreCase);
                result = Regex.Replace(result, @"(<script>).*(</script>)", string.Empty, RegexOptions.IgnoreCase);
                //remove all styles
                result = Regex.Replace(result, @"<( )*style([^>])*>", "<style>", RegexOptions.IgnoreCase); //clearing attributes
                result = Regex.Replace(result, "(<style>).*(</style>)", string.Empty, RegexOptions.IgnoreCase);

                //insert tabs in spaces of <td> tags
                result = Regex.Replace(result, @"<( )*td([^>])*>", " ", RegexOptions.IgnoreCase);

                //insert line breaks in places of <br> and <li> tags
                result = Regex.Replace(result, @"<( )*br( )*>", "\r", RegexOptions.IgnoreCase);
                result = Regex.Replace(result, @"<( )*li( )*>", "\r", RegexOptions.IgnoreCase);

                //insert line paragraphs in places of <tr> and <p> tags
                result = Regex.Replace(result, @"<( )*tr([^>])*>", "\r\r", RegexOptions.IgnoreCase);
                result = Regex.Replace(result, @"<( )*p([^>])*>", "\r\r", RegexOptions.IgnoreCase);

                //remove anything thats enclosed inside < >
                result = Regex.Replace(result, @"<[^>]*>", string.Empty, RegexOptions.IgnoreCase);

                //replace special characters:
                result = Regex.Replace(result, @"&", "&", RegexOptions.IgnoreCase);
                result = Regex.Replace(result, @" ", " ", RegexOptions.IgnoreCase);
                result = Regex.Replace(result, @"<", "<", RegexOptions.IgnoreCase);
                result = Regex.Replace(result, @">", ">", RegexOptions.IgnoreCase);
                result = Regex.Replace(result, @"&(.{2,6});", string.Empty, RegexOptions.IgnoreCase);
                //remove extra line breaks and tabs
                result = Regex.Replace(result, @" ( )+", " ");
                result = Regex.Replace(result, "(\r)( )+(\r)", "\r\r");
                result = Regex.Replace(result, @"(\r\r)+", "\r\n");

                //remove blank
                result = Regex.Replace(result, @"\s", "");

                return result;
            }
            catch 
            {
                return "";
            }
        }

        /// <summary>
        /// 移除Html标签及&nbsp等标记
        /// </summary>
        public static string Remove(string content)
        {
            if (string.IsNullOrEmpty(content)) return string.Empty;
            string regexstr = @"<[^>]*>";
            string temp = Regex.Replace(content, regexstr, string.Empty, RegexOptions.IgnoreCase);
            if (temp.Contains("<"))  //如果尾部存在没有闭合的标签 eg: <p
                temp = temp.Remove(temp.LastIndexOf("<"));
            temp = temp.Replace("&def;", "").Replace("&dot;", "").Replace("&quot;", "").Replace("&dec;", "").Replace("&lt;", "").Replace("&gt;", "").Replace("&nbsp;", "");
            return Regex.Replace(temp, "\\s{2,}", " ");   //将多个空格替换成一个空格
        }


        /// <summary>
        /// 过滤HTML中的不安全标签
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string RemoveUnsafeHtml(string content)
        {
            content = Regex.Replace(content, @"(\<|\s+)o([a-z]+\s?=)", "$1$2", RegexOptions.IgnoreCase);
            content = Regex.Replace(content, @"(script|frame|form|meta|behavior|style)([\s|:|>])+", "$1.$2", RegexOptions.IgnoreCase);
            return content;
        }


        public static string GetSafeTextFromHtml(string content)
        {
            Regex regex = new Regex("(<[^>]{0,}>)");
            content = regex.Replace(content, new MatchEvaluator(x => { return string.Empty; }));
            return content;
            //// 将输入字符串编码，这样所有的HTML标签都失效了。 
            //StringBuilder sb = new StringBuilder(
            //                        HttpUtility.HtmlEncode(content));
            //// 然后我们选择性的允许<b> 和 <i> 
            //sb.Replace(" ", "");
            //sb.Replace("&lt;b&gt;", "<b>");
            //sb.Replace("&lt;/b&gt;", "");
            //sb.Replace("&lt;i&gt;", "<i>");
            //sb.Replace("&lt;/i&gt;", "");
            //sb.Replace("<br>", "");
            //sb.Replace("</br>", "");
            //sb.Replace("<br />", "");
            //sb.Replace("<br/>", "");
            //return sb.ToString(); 
        }

        /// <summary>
        /// 从HTML中获取文本,保留br,p,img
        /// </summary>
        /// <param name="HTML"></param>
        /// <returns></returns>
        public static string GetTextFromHTML(string html)
        {
            System.Text.RegularExpressions.Regex regEx = new System.Text.RegularExpressions.Regex(@"</?(?!br|/?p|img)[^>]*>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            return regEx.Replace(html, "");
        }

        /// <summary>
        /// 在文本为空时返回默认字符串，比如：“无”
        /// </summary>
        /// <param name="str">需要处理的字符串</param>
        /// <param name="defaultStr">默认字符串</param>
        /// <returns>返回</returns>
        public static string GetDefaultTextIfEmpty(string str, string defaultStr)
        {
            str = StrFormat(str);
            if (string.IsNullOrEmpty(str))
            {
                return defaultStr;
            }
            return str;
        }
    }
}
