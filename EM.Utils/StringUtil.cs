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
    /// �ַ���ʵ����
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

            string[] arrTag = tags.Split(",�� ".ToCharArray());
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
        /// �����ַ�����ʵ����, 1�����ֳ���Ϊ2
        /// </summary>
        /// <returns></returns>
        public static int GetStringLength(string str)
        {
            return Encoding.Default.GetBytes(str).Length;
        }


        /// <summary>
        /// �ַ���תInt��,ʧ�ܷ���Ĭ��ֵ
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
        /// �ж�ָ���ַ�����ָ���ַ��������е�λ��
        /// </summary>
        /// <param name="strSearch">�ַ���</param>
        /// <param name="stringArray">�ַ�������</param>
        /// <param name="caseInsensetive">�Ƿ����ִ�Сд, trueΪ������, falseΪ����</param>
        /// <returns>�ַ�����ָ���ַ��������е�λ��, �粻�����򷵻�-1</returns>
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
        /// �ж�ָ���ַ�����ָ���ַ��������е�λ��
        /// </summary>
        /// <param name="strSearch">�ַ���</param>
        /// <param name="stringArray">�ַ�������</param>
        /// <returns>�ַ�����ָ���ַ��������е�λ��, �粻�����򷵻�-1</returns>		
        public static int GetInArrayID(string strSearch, string[] stringArray)
        {
            return GetInArrayID(strSearch, stringArray, true);
        }



        /// <summary>
        /// ɾ���ַ���β���Ļس�/����/�ո�
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
        /// ���ַ�����ָ��λ�ý�ȡָ�����ȵ����ַ���
        /// </summary>
        /// <param name="str">ԭ�ַ���</param>
        /// <param name="startIndex">���ַ�������ʼλ��</param>
        /// <param name="length">���ַ����ĳ���</param>
        /// <returns>���ַ���</returns>
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
        /// ȡ�ù̶����ȵ��ַ���(�����ֽڽ�ȡ)���������ȵ��ַ����Զ���ȡ���ԡ�...����β��
        /// </summary>
        /// <param name="Str">Դ�ַ���</param>
        /// <param name="Length">��ȡ����</param>
        /// <returns></returns>
        public static string CutString(string str, int length, string endstr)
        {
            //�ж��ִ��Ƿ�Ϊ��
            if (string.IsNullOrEmpty(str))
            {
                return string.Empty;
            }

            //�ж��ַ��������Ƿ���ڽضϳ���
            if (Encoding.Default.GetByteCount(str) > length)
            {
                //��ʼ��
                int i = 0, j = 0;

                //Ϊ���ֻ�ȫ�ŷ��ų��ȼ�2�����1
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
        /// ���ַ�����ָ��λ�ÿ�ʼ��ȡ���ַ�����β���˷���
        /// </summary>
        /// <param name="str">ԭ�ַ���</param>
        /// <param name="startIndex">���ַ�������ʼλ��</param>
        /// <returns>���ַ���</returns>
        public static string CutString(string str, int startIndex)
        {
            return CutString(str, startIndex, str.Length);
        }

        /// <summary>
        /// �ַ�������ٹ�ָ�������򽫳����Ĳ�����ָ���ַ�������
        /// </summary>
        /// <param name="p_SrcString">Ҫ�����ַ���</param>
        /// <param name="p_Length">ָ������</param>
        /// <param name="p_TailString">�����滻���ַ���</param>
        /// <returns>��ȡ����ַ���</returns>
        public static string GetSubString(string p_SrcString, int p_Length, string p_TailString)
        {
            return GetSubString(p_SrcString, 0, p_Length, p_TailString);
        }

        /// <summary>
        /// ȡָ�����ȵ��ַ���
        /// </summary>
        /// <param name="p_SrcString">Ҫ�����ַ���</param>
        /// <param name="p_StartIndex">��ʼλ��</param>
        /// <param name="p_Length">ָ������</param>
        /// <param name="p_TailString">�����滻���ַ���</param>
        /// <returns>��ȡ����ַ���</returns>
        public static string GetSubString(string p_SrcString, int p_StartIndex, int p_Length, string p_TailString)
        {
            if (string.IsNullOrWhiteSpace(p_SrcString))
                return string.Empty;

            string myResult = p_SrcString;
            if (p_Length >= 0)
            {
                byte[] bsSrcString = Encoding.Default.GetBytes(p_SrcString);

                //���ַ������ȴ�����ʼλ��
                if (bsSrcString.Length > p_StartIndex)
                {
                    int p_EndIndex = bsSrcString.Length;

                    //��Ҫ��ȡ�ĳ������ַ�������Ч���ȷ�Χ��
                    if (bsSrcString.Length > (p_StartIndex + p_Length))
                        p_EndIndex = p_Length + p_StartIndex;
                    else
                    {
                        //��������Ч��Χ��ʱ,ֻȡ���ַ����Ľ�β
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
        /// �Զ�����滻�ַ�������
        /// </summary>
        public static string ReplaceString(string SourceString, string SearchString, string ReplaceString, bool IsCaseInsensetive)
        {
            return Regex.Replace(SourceString, Regex.Escape(SearchString), ReplaceString, IsCaseInsensetive ? RegexOptions.IgnoreCase : RegexOptions.None);
        }

        /// <summary>
        /// ����ָ��������html�ո����
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
        /// �����ַ���
        /// </summary>
        public static string CleanInput(string strIn)
        {
            return Regex.Replace(strIn.Trim(), @"[^\w\.@-]", "");
        }

        /// <summary>
        /// �ָ��ַ���
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
        /// �ָ��ַ���
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
        /// �ָ��ַ���
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
        /// ���� HTML �ַ����ı�����
        /// </summary>
        /// <param name="str">�ַ���</param>
        /// <returns>������</returns>
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
        /// ���� HTML �ַ����Ľ�����
        /// </summary>
        /// <param name="str">�ַ���</param>
        /// <returns>������</returns>
        public static string HtmlDecode(string str)
        {
            if (str == null)
                return null;
            return str.Replace("&nbsp;", " ").Replace("<br />", "").Replace("<p>", "").Replace("</p>", "").Replace("<br>", "").Replace("<p align=\"left\">", "")
                .Replace("<div>","").Replace("</div>","");
        }

        /// <summary>
        /// ���� URL �ַ����ı�����
        /// </summary>
        /// <param name="str">�ַ���</param>
        /// <returns>������</returns>
        public static string UrlEncode(string str)
        {
            return HttpUtility.UrlEncode(str, Encoding.UTF8);
        }

        /// <summary>
        /// ���� URL �ַ����ı�����
        /// </summary>
        /// <param name="str">�ַ���</param>
        /// <returns>������</returns>
        public static string UrlDecode(string str)
        {
            return HttpUtility.UrlDecode(str, Encoding.UTF8);
        }

        /// <summary>
        /// ɾ�����һ���ַ�
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
        /// ��ȫ������ת��Ϊ����
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
        /// ���ַ����е�β��ɾ��ָ�����ַ���
        /// </summary>
        /// <param name="sourceString"></param>
        /// <param name="removedString"></param>
        /// <returns></returns>
        public static string Remove(string sourceString, string removedString)
        {
            try
            {
                if (sourceString.IndexOf(removedString) < 0)
                    throw new Exception("ԭ�ַ����в������Ƴ��ַ�����");
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
        /// ��ȡ��ַ��ұߵ��ַ���
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
        /// ��ȡ��ַ���ߵ��ַ���
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
        /// ȥ�����һ������
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
        /// ɾ�����ɼ��ַ�
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
        /// ��ȡ����Ԫ�صĺϲ��ַ���
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
        ///		��ȡĳһ�ַ������ַ��������г��ֵĴ���
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
        ///     ��ȡĳһ�ַ������ַ����г��ֵĴ���
        /// </summary>
        /// <param name="stringArray" type="string">
        ///     <para>
        ///         ԭ�ַ���
        ///     </para>
        /// </param>
        /// <param name="findString" type="string">
        ///     <para>
        ///         ƥ���ַ���
        ///     </para>
        /// </param>
        /// <returns>
        ///     ƥ���ַ�������
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
        /// ��ȡ��startString��ʼ��ԭ�ַ�����β�������ַ�   
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
        /// ���ֽ���ȡ���ַ����ĳ���
        /// </summary>
        /// <param name="strTmp">Ҫ������ַ���</param>
        /// <returns>�ַ������ֽ���</returns>
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
        /// ���ֽ���Ҫ���ַ�����λ��
        /// </summary>
        /// <param name="intIns">�ַ�����λ��</param>
        /// <param name="strTmp">Ҫ������ַ���</param>
        /// <returns>�ֽڵ�λ��</returns>
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
        /// <������Encode>
        /// ���ã����ַ�������ת��Ϊ16�������ݱ��룬���������Decode
        /// ����˵����
        /// strEncode ��Ҫת����ԭʼ�ַ���
        /// ת���Ĺ�����ֱ�Ӱ��ַ�ת����Unicode�ַ�,��������"3"-->0033,����"��"-->U+6211
        /// ����decode�Ĺ�����encode�������.
        /// </summary>
        /// <param name="strEncode"></param>
        /// <returns></returns>
        public static string Encode(string strEncode)
        {
            string strReturn = "";//  �洢ת����ı���
            foreach (short shortx in strEncode.ToCharArray())
            {
                strReturn += shortx.ToString("X4");
            }
            return strReturn;
        }
        /// <summary>
        /// <������Decode>
        ///���ã���16�������ݱ���ת��Ϊ�ַ�������Encode�������
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
        /// ���˲���ȫ���ַ���
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
        /// �·ݲ���
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
        /// ���������
        /// </summary>
        /// <param name="i">�����λ��</param>
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
        /// �ж��ļ����Ƿ�Ϸ�
        /// </summary>
        /// <param name="fileName">��Ҫ�жϵ��ļ���</param>
        /// <returns></returns>
        public static bool IsValidFileName(string fileName)
        {
            string pattern = "\"" + @"\/:*?<>|��";
            Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);
            MatchCollection mc = regex.Matches(fileName);
            return mc.Count == 0;
        }

        #region html �淶������

        /// <summary>
        /// �Ƴ����������ţ��� �հף�һ�������title
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ReplaceBracesAndTrim(string value)
        {
            return value.Replace("��", "(").Replace("��", ")").Replace(" ", "").Trim();
        }

        /// <summary>
        /// �Ƴ�Pattern��ƥ�䵽��
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
        /// �Ƴ�html��ǩ
        /// </summary>
        /// <param name="html"></param>
        /// <param name="patternList">eg.�Ƴ�font��ǩ"(</?font[^>]*>)"</param>
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
