using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Security;
using System.Text.RegularExpressions;
using System.Collections;

namespace EM.Utils
{
    /// <summary>
    /// 字符串实用类
    /// </summary>
    public sealed class StringUtil
    {
        private StringUtil()
        {
        }

        public static string md5(string str)
        {
            return System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(string.Format("4a6fc8d871f64dba{0}93d16e91013cc35d", str), "MD5").ToLower();
        }

        public static string GetTags(string tags)
        {
            if (string.IsNullOrWhiteSpace(tags))
                return tags;

            string[] arrTag = tags.Split(",， ".ToCharArray());
            StringBuilder sb = new StringBuilder(arrTag[0]);
            Hashtable hash = new Hashtable();
            hash.Add(arrTag[0], string.Empty);
            for (int i = 1; i < arrTag.Length; i++)
            {
                if (arrTag[i] != null && arrTag[i] != string.Empty && !hash.Contains(arrTag[i].ToLower()))
                {
                    sb.Append(string.Format(",{0}", arrTag[i]));
                    hash.Add(arrTag[i].ToLower(), string.Empty);
                }
            }
            return sb.ToString();
        }


        /// <summary>
        /// 返回字符串真实长度, 1个汉字长度为2
        /// </summary>
        /// <returns></returns>
        public static int GetStringLength(string str)
        {
            return Encoding.Default.GetBytes(str).Length;
        }


        /// <summary>
        /// 字符串转Int型,失败返回默认值
        /// </summary>
        /// <param name="pStr"></param>
        /// <param name="pDefaultValue"></param>
        /// <returns></returns>
        public static int StrToInt(string pStr, int pDefaultValue)
        {
            if (string.IsNullOrEmpty(pStr))
            {
                return pDefaultValue;
            }
            try
            {
                return int.Parse(pStr);
            }
            catch
            {
                return pDefaultValue;
            }
        }


        /// <summary>
        /// 判断指定字符串在指定字符串数组中的位置
        /// </summary>
        /// <param name="strSearch">字符串</param>
        /// <param name="stringArray">字符串数组</param>
        /// <param name="caseInsensetive">是否不区分大小写, true为不区分, false为区分</param>
        /// <returns>字符串在指定字符串数组中的位置, 如不存在则返回-1</returns>
        public static int GetInArrayID(string strSearch, string[] stringArray, bool caseInsensetive)
        {
            for (int i = 0; i < stringArray.Length; i++)
            {
                if (caseInsensetive)
                {
                    if (strSearch.ToLower() == stringArray[i].ToLower())
                    {
                        return i;
                    }
                }
                else
                {
                    if (strSearch == stringArray[i])
                    {
                        return i;
                    }
                }

            }
            return -1;
        }


        /// <summary>
        /// 判断指定字符串在指定字符串数组中的位置
        /// </summary>
        /// <param name="strSearch">字符串</param>
        /// <param name="stringArray">字符串数组</param>
        /// <returns>字符串在指定字符串数组中的位置, 如不存在则返回-1</returns>		
        public static int GetInArrayID(string strSearch, string[] stringArray)
        {
            return GetInArrayID(strSearch, stringArray, true);
        }



        /// <summary>
        /// 删除字符串尾部的回车/换行/空格
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string RTrim(string str)
        {
            for (int i = str.Length; i >= 0; i--)
            {
                if (str[i].Equals(" ") || str[i].Equals("\r") || str[i].Equals("\n"))
                {
                    str.Remove(i, 1);
                }
            }
            return str;
        }



        /// <summary>
        /// 从字符串的指定位置截取指定长度的子字符串
        /// </summary>
        /// <param name="str">原字符串</param>
        /// <param name="startIndex">子字符串的起始位置</param>
        /// <param name="length">子字符串的长度</param>
        /// <returns>子字符串</returns>
        public static string CutString(string str, int startIndex, int length)
        {
            if (string.IsNullOrWhiteSpace(str))
                return str;
            if (startIndex >= 0)
            {
                if (length < 0)
                {
                    length = length * -1;
                    if (startIndex - length < 0)
                    {
                        length = startIndex;
                        startIndex = 0;
                    }
                    else
                        startIndex = startIndex - length;
                }

                if (startIndex > str.Length)
                    return string.Empty;
            }
            else
            {
                if (length < 0)
                    return string.Empty;
                else
                {
                    if (length + startIndex > 0)
                    {
                        length = length + startIndex;
                        startIndex = 0;
                    }
                    else
                        return string.Empty;
                }
            }

            if (str.Length - startIndex < length)
            {
                length = str.Length - startIndex;
            }
            return str.Substring(startIndex, length);
        }

        /// <summary>
        /// 取得固定长度的字符串(按单字节截取)，超过长度的字符串自动截取并以“...”结尾。
        /// </summary>
        /// <param name="Str">源字符串</param>
        /// <param name="Length">截取长度</param>
        /// <returns></returns>
        public static string CutString(string str, int length, string endstr)
        {
            //判断字串是否为空
            if (string.IsNullOrEmpty(str))
            {
                return string.Empty;
            }

            //判断字符串长度是否大于截断长度
            if (Encoding.Default.GetByteCount(str) > length)
            {
                //初始化
                int i = 0, j = 0;

                //为汉字或全脚符号长度加2否则加1
                foreach (char c in str)
                {
                    if ((int)c > 127)
                    {
                        i += 2;
                    }
                    else
                    {
                        i++;
                    }
                    if (i > length)
                    {
                        str = str.Substring(0, j) + endstr;
                        break;
                    }
                    j++;
                }
            }

            return str;
        }

        /// <summary>
        /// 从字符串的指定位置开始截取到字符串结尾的了符串
        /// </summary>
        /// <param name="str">原字符串</param>
        /// <param name="startIndex">子字符串的起始位置</param>
        /// <returns>子字符串</returns>
        public static string CutString(string str, int startIndex)
        {
            return CutString(str, startIndex, str.Length);
        }

        /// <summary>
        /// 字符串如果操过指定长度则将超出的部分用指定字符串代替
        /// </summary>
        /// <param name="p_SrcString">要检查的字符串</param>
        /// <param name="p_Length">指定长度</param>
        /// <param name="p_TailString">用于替换的字符串</param>
        /// <returns>截取后的字符串</returns>
        public static string GetSubString(string p_SrcString, int p_Length, string p_TailString)
        {
            return GetSubString(p_SrcString, 0, p_Length, p_TailString);
        }

        /// <summary>
        /// 取指定长度的字符串
        /// </summary>
        /// <param name="p_SrcString">要检查的字符串</param>
        /// <param name="p_StartIndex">起始位置</param>
        /// <param name="p_Length">指定长度</param>
        /// <param name="p_TailString">用于替换的字符串</param>
        /// <returns>截取后的字符串</returns>
        public static string GetSubString(string p_SrcString, int p_StartIndex, int p_Length, string p_TailString)
        {
            if (string.IsNullOrWhiteSpace(p_SrcString))
                return string.Empty;

            string myResult = p_SrcString;
            if (p_Length >= 0)
            {
                byte[] bsSrcString = Encoding.Default.GetBytes(p_SrcString);

                //当字符串长度大于起始位置
                if (bsSrcString.Length > p_StartIndex)
                {
                    int p_EndIndex = bsSrcString.Length;

                    //当要截取的长度在字符串的有效长度范围内
                    if (bsSrcString.Length > (p_StartIndex + p_Length))
                        p_EndIndex = p_Length + p_StartIndex;
                    else
                    {
                        //当不在有效范围内时,只取到字符串的结尾
                        p_Length = bsSrcString.Length - p_StartIndex;
                        p_TailString = string.Empty;
                    }

                    int nRealLength = p_Length;
                    int[] anResultFlag = new int[p_Length];
                    byte[] bsResult = null;

                    int nFlag = 0;
                    for (int i = p_StartIndex; i < p_EndIndex; i++)
                    {
                        if (bsSrcString[i] > 127)
                        {
                            nFlag++;
                            if (nFlag == 3)
                                nFlag = 1;
                        }
                        else
                        {
                            nFlag = 0;
                        }
                        anResultFlag[i] = nFlag;
                    }
                    if ((bsSrcString[p_EndIndex - 1] > 127) && (anResultFlag[p_Length - 1] == 1))
                        nRealLength = p_Length + 1;
                    bsResult = new byte[nRealLength];

                    Array.Copy(bsSrcString, p_StartIndex, bsResult, 0, nRealLength);
                    myResult = Encoding.Default.GetString(bsResult);
                    myResult = myResult + p_TailString;
                }
            }
            return myResult;
        }

        /// <summary>
        /// 自定义的替换字符串函数
        /// </summary>
        public static string ReplaceString(string SourceString, string SearchString, string ReplaceString, bool IsCaseInsensetive)
        {
            return Regex.Replace(SourceString, Regex.Escape(SearchString), ReplaceString, IsCaseInsensetive ? RegexOptions.IgnoreCase : RegexOptions.None);
        }

        /// <summary>
        /// 生成指定数量的html空格符号
        /// </summary>
        public static string Spaces(int nSpaces)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < nSpaces; i++)
            {
                sb.Append(" &nbsp;&nbsp;");
            }
            return sb.ToString();
        }

        /// <summary>
        /// 清理字符串
        /// </summary>
        public static string CleanInput(string strIn)
        {
            return Regex.Replace(strIn.Trim(), @"[^\w\.@-]", "");
        }

        /// <summary>
        /// 分割字符串
        /// </summary>
        public static string[] SplitString(string strContent, string strSplit)
        {
            if (strContent.IndexOf(strSplit) < 0)
            {
                string[] tmp = { strContent };
                return tmp;
            }
            return Regex.Split(strContent, Regex.Escape(strSplit), RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// 分割字符串
        /// </summary>
        public static string[] SplitStringByRegex(string strContent, string strSplit)
        {
            if (!Regex.IsMatch(strContent, strSplit))
            {
                string[] tmp = { strContent };
                return tmp;
            }
            return Regex.Split(strContent, strSplit, RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// 分割字符串
        /// </summary>
        /// <returns></returns>
        public static string[] SplitString(string strContent, string strSplit, int p_3)
        {
            string[] result = new string[p_3];

            string[] splited = SplitString(strContent, strSplit);

            for (int i = 0; i < p_3; i++)
            {
                if (i < splited.Length)
                    result[i] = splited[i];
                else
                    result[i] = string.Empty;
            }

            return result;
        }

        /// <summary>
        /// 返回 HTML 字符串的编码结果
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>编码结果</returns>
        public static string HtmlEncode(string str)
        {
            if (str == null)
                return null;
            return str.Replace(" ", "&nbsp;").Replace("\r\n", "<br />");
        }

        public static string LineEncode(string str)
        {
            if (str == null)
                return null;
            return str.Replace("\r\n", "<br />");
        } 

        /// <summary>
        /// 返回 HTML 字符串的解码结果
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>解码结果</returns>
        public static string HtmlDecode(string str)
        {
            if (str == null)
                return null;
            return str.Replace("&nbsp;", " ").Replace("<br />", "").Replace("<p>", "").Replace("</p>", "").Replace("<br>", "").Replace("<p align=\"left\">", "")
                .Replace("<div>","").Replace("</div>","");
        }

        /// <summary>
        /// 返回 URL 字符串的编码结果
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>编码结果</returns>
        public static string UrlEncode(string str)
        {
            return HttpUtility.UrlEncode(str, Encoding.UTF8);
        }

        /// <summary>
        /// 返回 URL 字符串的编码结果
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>解码结果</returns>
        public static string UrlDecode(string str)
        {
            return HttpUtility.UrlDecode(str, Encoding.UTF8);
        }

        /// <summary>
        /// 删除最后一个字符
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ClearLastChar(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return str;
            else
                return str.Substring(0, str.Length - 1);
        }

        /// <summary>
        /// 将全角数字转换为数字
        /// </summary>
        /// <param name="SBCCase"></param>
        /// <returns></returns>
        public static string SBCCaseToNumberic(string SBCCase)
        {
            char[] c = SBCCase.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                byte[] b = System.Text.Encoding.Unicode.GetBytes(c, i, 1);
                if (b.Length == 2)
                {
                    if (b[1] == 255)
                    {
                        b[0] = (byte)(b[0] + 32);
                        b[1] = 0;
                        c[i] = System.Text.Encoding.Unicode.GetChars(b)[0];
                    }
                }
            }
            return new string(c);
        }


        static public string GetChineseSpell(string strText)
        {
            int len = strText.Length;
            string myStr = string.Empty;
            for (int i = 0; i < len; i++)
            {
                myStr += GetSpell(strText.Substring(i, 1));
            }
            return myStr;
        }

        static public string GetSpell(string cnChar)
        {
            byte[] arrCN = Encoding.Default.GetBytes(cnChar);
            if (arrCN.Length > 1)
            {
                int area = (short)arrCN[0];
                int pos = (short)arrCN[1];
                int code = (area << 8) + pos;
                int[] areacode = { 45217, 45253, 45761, 46318, 46826, 47010, 47297, 47614, 48119, 48119, 49062, 49324, 49896, 50371, 50614, 50622, 50906, 51387, 51446, 52218, 52698, 52698, 52698, 52980, 53689, 54481 };
                for (int i = 0; i < 26; i++)
                {
                    int max = 55290;
                    if (i != 25) max = areacode[i + 1];
                    if (areacode[i] <= code && code < max)
                    {
                        return Encoding.Default.GetString(new byte[] { (byte)(65 + i) });
                    }
                }
                return "*";
            }
            else return cnChar;
        }

        /// <summary>
        /// 从字符串中的尾部删除指定的字符串
        /// </summary>
        /// <param name="sourceString"></param>
        /// <param name="removedString"></param>
        /// <returns></returns>
        public static string Remove(string sourceString, string removedString)
        {
            try
            {
                if (sourceString.IndexOf(removedString) < 0)
                    throw new Exception("原字符串中不包含移除字符串！");
                string result = sourceString;
                int lengthOfSourceString = sourceString.Length;
                int lengthOfRemovedString = removedString.Length;
                int startIndex = lengthOfSourceString - lengthOfRemovedString;
                string tempSubString = sourceString.Substring(startIndex);
                if (tempSubString.ToUpper() == removedString.ToUpper())
                {
                    result = sourceString.Remove(startIndex, lengthOfRemovedString);
                }
                return result;
            }
            catch
            {
                return sourceString;
            }
        }

        /// <summary>
        /// 获取拆分符右边的字符串
        /// </summary>
        /// <param name="sourceString"></param>
        /// <param name="splitChar"></param>
        /// <returns></returns>
        public static string RightSplit(string sourceString, char splitChar)
        {
            string result = null;
            string[] tempString = sourceString.Split(splitChar);
            if (tempString.Length > 0)
            {
                result = tempString[tempString.Length - 1].ToString();
            }
            return result;
        }

        /// <summary>
        /// 获取拆分符左边的字符串
        /// </summary>
        /// <param name="sourceString"></param>
        /// <param name="splitChar"></param>
        /// <returns></returns>
        public static string LeftSplit(string sourceString, char splitChar)
        {
            string result = null;
            string[] tempString = sourceString.Split(splitChar);
            if (tempString.Length > 0)
            {
                result = tempString[0].ToString();
            }
            return result;
        }

        /// <summary>
        /// 去掉最后一个逗号
        /// </summary>
        /// <param name="origin"></param>
        /// <returns></returns>
        public static string DelLastComma(string origin)
        {
            if (origin.IndexOf(",") == -1)
            {
                return origin;
            }
            return origin.Substring(0, origin.LastIndexOf(","));
        }

        /// <summary>
        /// 删除不可见字符
        /// </summary>
        /// <param name="sourceString"></param>
        /// <returns></returns>
        public static string DeleteUnVisibleChar(string sourceString)
        {
            System.Text.StringBuilder sBuilder = new System.Text.StringBuilder(131);
            for (int i = 0; i < sourceString.Length; i++)
            {
                int Unicode = sourceString[i];
                if (Unicode >= 16)
                {
                    sBuilder.Append(sourceString[i].ToString());
                }
            }
            return sBuilder.ToString();
        }

        /// <summary>
        /// 获取数组元素的合并字符串
        /// </summary>
        /// <param name="stringArray"></param>
        /// <returns></returns>
        public static string GetArrayString(string[] stringArray)
        {
            string totalString = null;
            for (int i = 0; i < stringArray.Length; i++)
            {
                totalString = totalString + stringArray[i];
            }
            return totalString;
        }

        /// <summary>
        ///		获取某一字符串在字符串数组中出现的次数
        /// </summary>
        /// <param name="stringArray" type="string[]">
        ///     <para>
        ///         
        ///     </para>
        /// </param>
        /// <param name="findString" type="string">
        ///     <para>
        ///         
        ///     </para>
        /// </param>
        /// <returns>
        ///     A int value...
        /// </returns>
        public static int GetStringCount(string[] stringArray, string findString)
        {
            int count = -1;
            string totalString = GetArrayString(stringArray);
            string subString = totalString;

            while (subString.IndexOf(findString) >= 0)
            {
                subString = totalString.Substring(subString.IndexOf(findString));
                count += 1;
            }
            return count;
        }

        /// <summary>
        ///     获取某一字符串在字符串中出现的次数
        /// </summary>
        /// <param name="stringArray" type="string">
        ///     <para>
        ///         原字符串
        ///     </para>
        /// </param>
        /// <param name="findString" type="string">
        ///     <para>
        ///         匹配字符串
        ///     </para>
        /// </param>
        /// <returns>
        ///     匹配字符串数量
        /// </returns>
        public static int GetStringCount(string sourceString, string findString)
        {
            int count = 0;
            int findStringLength = findString.Length;
            string subString = sourceString;

            while (subString.IndexOf(findString) >= 0)
            {
                subString = subString.Substring(subString.IndexOf(findString) + findStringLength);
                count += 1;
            }
            return count;
        }

        /// <summary>
        /// 截取从startString开始到原字符串结尾的所有字符   
        /// </summary>
        /// <param name="sourceString" type="string">
        ///     <para>
        ///         
        ///     </para>
        /// </param>
        /// <param name="startString" type="string">
        ///     <para>
        ///         
        ///     </para>
        /// </param>
        /// <returns>
        ///     A string value...
        /// </returns>
        public static string GetSubString(string sourceString, string startString)
        {
            try
            {
                int index = sourceString.ToUpper().IndexOf(startString);
                if (index > 0)
                {
                    return sourceString.Substring(index);
                }
                return sourceString;
            }
            catch
            {
                return string.Empty;
            }
        }

        public static string GetSubString(string sourceString, string beginRemovedString, string endRemovedString)
        {
            try
            {
                if (sourceString.IndexOf(beginRemovedString) != 0)
                    beginRemovedString = string.Empty;

                if (sourceString.LastIndexOf(endRemovedString, sourceString.Length - endRemovedString.Length) < 0)
                    endRemovedString = string.Empty;

                int startIndex = beginRemovedString.Length;
                int length = sourceString.Length - beginRemovedString.Length - endRemovedString.Length;
                if (length > 0)
                {
                    return sourceString.Substring(startIndex, length);
                }
                return sourceString;
            }
            catch
            {
                return sourceString; ;
            }
        }

        /// <summary>
        /// 按字节数取出字符串的长度
        /// </summary>
        /// <param name="strTmp">要计算的字符串</param>
        /// <returns>字符串的字节数</returns>
        public static int GetByteCount(string strTmp)
        {
            int intCharCount = 0;
            for (int i = 0; i < strTmp.Length; i++)
            {
                if (System.Text.UTF8Encoding.UTF8.GetByteCount(strTmp.Substring(i, 1)) == 3)
                {
                    intCharCount = intCharCount + 2;
                }
                else
                {
                    intCharCount = intCharCount + 1;
                }
            }
            return intCharCount;
        }

        /// <summary>
        /// 按字节数要在字符串的位置
        /// </summary>
        /// <param name="intIns">字符串的位置</param>
        /// <param name="strTmp">要计算的字符串</param>
        /// <returns>字节的位置</returns>
        public static int GetByteIndex(int intIns, string strTmp)
        {
            int intReIns = 0;
            if (!string.IsNullOrWhiteSpace(strTmp))
            {
                return intIns;
            }
            for (int i = 0; i < strTmp.Length; i++)
            {
                if (System.Text.UTF8Encoding.UTF8.GetByteCount(strTmp.Substring(i, 1)) == 3)
                {
                    intReIns = intReIns + 2;
                }
                else
                {
                    intReIns = intReIns + 1;
                }
                if (intReIns >= intIns)
                {
                    intReIns = i + 1;
                    break;
                }
            }
            return intReIns;
        }

        /// <summary>
        /// <函数：Encode>
        /// 作用：将字符串内容转化为16进制数据编码，其逆过程是Decode
        /// 参数说明：
        /// strEncode 需要转化的原始字符串
        /// 转换的过程是直接把字符转换成Unicode字符,比如数字"3"-->0033,汉字"我"-->U+6211
        /// 函数decode的过程是encode的逆过程.
        /// </summary>
        /// <param name="strEncode"></param>
        /// <returns></returns>
        public static string Encode(string strEncode)
        {
            string strReturn = "";//  存储转换后的编码
            foreach (short shortx in strEncode.ToCharArray())
            {
                strReturn += shortx.ToString("X4");
            }
            return strReturn;
        }
        /// <summary>
        /// <函数：Decode>
        ///作用：将16进制数据编码转化为字符串，是Encode的逆过程
        /// </summary>
        /// <param name="strDecode"></param>
        /// <returns></returns>
        public static string Decode(string strDecode)
        {
            string sResult = string.Empty;
            if (strDecode.Length > 0)
            {
                for (int i = 0; i < strDecode.Length / 4; i++)
                {
                    sResult += (char)short.Parse(strDecode.Substring(i * 4, 4), global::System.Globalization.NumberStyles.HexNumber);
                }
            }
            return sResult;
        }
        /// <summary>
        /// 过滤不安全的字符串
        /// </summary>
        /// <param name="Str"></param>
        /// <returns></returns>
        public static string FilteSQLStr(string Str)
        {
            string sResult = string.Empty;
            if (Str != null)
            {
                if (Str.Length > 0)
                {
                    Str = Str.Replace("'", "");
                    Str = Str.Replace("\"", "");
                    Str = Str.Replace("&", "&amp");
                    Str = Str.Replace("<", "&lt");
                    Str = Str.Replace(">", "&gt");

                    Str = Str.Replace("delete", "");
                    Str = Str.Replace("update", "");
                    Str = Str.Replace("insert", "");
                    sResult = Str;
                }
            }
            return sResult;

        }
        /// <summary>
        /// 月份补满
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public static string FillMon(int i)
        {
            string sResult = string.Empty;
            if (i < 10)
            {
                sResult += "0" + i.ToString();
            }
            else
            {
                sResult += i.ToString();
            }
            return sResult;

        }

        /// <summary>
        /// 随机数生成
        /// </summary>
        /// <param name="i">随机数位数</param>
        /// <returns></returns>
        private static char[] constant = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
        public static string GenerateRandomNumber(int Length)
        {
            System.Text.StringBuilder newRandom = new System.Text.StringBuilder(10);
            Random rd = new Random();
            for (int i = 0; i < Length; i++)
            {
                newRandom.Append(constant[rd.Next(10)]);
            }
            return newRandom.ToString();
        }


        /// <summary>
        /// 判断文件名是否合法
        /// </summary>
        /// <param name="fileName">需要判断的文件名</param>
        /// <returns></returns>
        public static bool IsValidFileName(string fileName)
        {
            string pattern = "\"" + @"\/:*?<>|·";
            Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);
            MatchCollection mc = regex.Matches(fileName);
            return mc.Count == 0;
        }

        #region html 规范化处理

        /// <summary>
        /// 移除掉中文括号，及 空白，一般操作于title
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ReplaceBracesAndTrim(string value)
        {
            return value.Replace("（", "(").Replace("）", ")").Replace(" ", "").Trim();
        }

        /// <summary>
        /// 移除Pattern所匹配到的
        /// </summary>
        /// <param name="html"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public static string RemoveMatched(string html, string pattern)
        {
            if (string.IsNullOrEmpty(html)) return "";
            Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);
            MatchCollection col = regex.Matches(html);
            if (col.Count > 0)
            {
                foreach (Match m in col)
                {
                    html = html.Replace(m.Groups[1].Value, "");
                }
            }
            return html;
        }

        public static string RemoveClassAttr(string html)
        {
            string pattern = @"(class\s*=\s*""[^""]*?"")";
            return RemoveMatched(html, pattern);
        }
        public static string RemoveStyleAttr(string html)
        {
            string pattern = @"(style\s*=\s*""[^""]*?"")";
            return RemoveMatched(html, pattern);
        }

        /// <summary>
        /// 移除html标签
        /// </summary>
        /// <param name="html"></param>
        /// <param name="patternList">eg.移除font标签"(</?font[^>]*>)"</param>
        /// <returns></returns>
        public static string RemoveTag(string html, List<string> patternList)
        {
            if (string.IsNullOrEmpty(html)) return "";
            patternList.ForEach(p => html = RemoveMatched(html, p));
            return html.Trim();
        }

        public static string RemoveAllHtmlTag(string html, bool ignoreBR = true, int takeLength = 0)
        {
            if (string.IsNullOrEmpty(html)) return "";
            var pattern = @"(<[^<>]*?>)";
            Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);
            MatchCollection col = regex.Matches(html);
            if (col.Count > 0)
            {
                foreach (Match m in col)
                {
                    var matchedVal = m.Groups[1].Value;
                    if (matchedVal.Length > 0 && (ignoreBR || matchedVal != "<br/>"))
                    {
                        html = html.Replace(matchedVal, "");
                    }
                }
            }
            html = html.Trim();
            if (html.Length > 0)
            {
                html = StringUtil.CutString(html, takeLength, "...");
            }

            if (html.Length > 0)
            {
                var iIndex = html.LastIndexOf("<");
                if (iIndex > 0) 
                {
                    var str = html.Substring(iIndex);
                    if (str.Length < 10)
                    {
                        html = html.Substring(0, iIndex) + "...";
                    }
                }
            }

            return html;
        }
        #endregion

    }
}
