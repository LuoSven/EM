using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EM.Common
{
    public class ArrayConvertor
    {
        public static string[] Convert(object[] list)
        {
            return Array.ConvertAll<object, string>(list, delegate(object o) { return o.ToString(); });
        }

        public static int[] Convert(string[] list)
        {
            return Array.ConvertAll<string, int>(list, delegate(string s) { int resultint = 0; int.TryParse(s, out  resultint); return resultint; });
        }

        public static int[] Convert(string strList)
        {
            if (!string.IsNullOrEmpty(strList))
            {
                return Convert(strList.Split(','));
            }
            return new int[] { };
        }

        public static string[] Convert(string strList, char separator)
        {
            if (!string.IsNullOrEmpty(strList))
            {
                return Convert((object[])strList.Split(separator));
            }
            return new string[] { };
        }
    }
}
