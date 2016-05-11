using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Reflection;
using System.Web.UI;
namespace EM
{
    public static class ExtensionHelper
    {
        /// <summary>
        /// 获取枚举值的详细文本
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static string GetEnumDescription(this object e)
        {

            //object objDesc = DataCache.GetCache(e.ToString());
            //if (objDesc != null)
            //{
            //    return objDesc.ToString();//获得缓存
            //}
            //获取字段信息
            FieldInfo[] ms = e.GetType().GetFields();

            Type t = e.GetType();
            foreach (FieldInfo f in ms)
            {
                //判断名称是否相等
                if (f.Name != e.ToString()) continue;

                //反射出自定义属性
                foreach (Attribute attr in f.GetCustomAttributes(true))
                {
                    //类型转换找到一个Description，用Description作为成员名称
                    DescriptionAttribute dscript = attr as DescriptionAttribute;
                    if (dscript != null)
                    {
                        //DataCache.SetCache(e.ToString(), dscript.Description);//设置缓存
                        return dscript.Description;
                    }
                }
            }
            //如果没有检测到合适的注释，则用默认名称
            return e.ToString();
        }

        public static List<int> ToInts(this string e,char split=',')
        {
            if (string.IsNullOrEmpty(e))
                return new List<int>();
            var Ids = new List<int>();
            foreach (var id in e.Split(','))
            {
                var tempId = 0;
                int.TryParse(id, out tempId);
                Ids.Add(tempId);
            }
            return Ids;
        }
        public static int ToInt(this string e)
        {
            if (string.IsNullOrEmpty(e))
                return 0;
            var Id = 0;
            int.TryParse(e, out Id);
            return Id;
        }

        public static Decimal ToDecimal(this string e)
        {
            if (string.IsNullOrEmpty(e))
                return 0;
            Decimal Id = 0;
            Decimal.TryParse(e, out Id);
            return Id;
        }
        /// <summary>
        /// 获取月份名称，碰到1月就会返回年，主要是下拉和报表用
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static string GetMonthName(this DateTime e)
        {
            var MonthName="";
            if (e.Month != 1)
            {
                MonthName = e.Month.ToString() + "月";
            }
            else
            {
                MonthName = e.Year + "年" + e.Month.ToString() + "月";
            }
            return MonthName;
        }
        public static List<KeyValuePair<int, string>> GetEnumList(this object em)
        {
            var Type = em.GetType();
            var list = new List<KeyValuePair<int, string>>();
            foreach (int key in Enum.GetValues(Type))
            {
                string strName = Enum.ToObject(Type, key).GetEnumDescription();//获取名称
                list.Add(new KeyValuePair<int,string>(key,strName));//添加到DropDownList控件
            }
            return list;
        }


        public static void BindYear(this DropDownList ddl)
        {
            ddl.Items.Clear();
            int intNowYear = DateTime.Today.Year;
            for (int i = 0; i < 60; i++)
            {
                string tempYear = (intNowYear - i).ToString();
                ddl.Items.Add(new ListItem(tempYear, tempYear));
            }
            ddl.Items.Insert(0, new ListItem("年", "0"));
            ddl.SelectedIndex = 0;
        }

        public static void BindMonth(this DropDownList ddl)
        {
            ddl.Items.Clear();
            for (int i = 0; i < 12; i++)
            {
                string tempMonth = (i + 1).ToString();
                ddl.Items.Add(new ListItem(tempMonth, tempMonth));
            }
            ddl.Items.Insert(0, new ListItem("月", "0"));
            ddl.SelectedIndex = 0;
        }

        public static void BindDay(this DropDownList ddl)
        {
            ddl.Items.Clear();
            for (int i = 0; i < 31; i++)
            {
                string tempDay = (i + 1).ToString();
                ddl.Items.Add(new ListItem(tempDay, tempDay));
            }
            ddl.Items.Insert(0, new ListItem("日", "0"));
            ddl.SelectedIndex = 0;
        }
        /// <summary>
        /// 多于5个字自动省略
        /// </summary>
        /// <param name="e"></param>
        /// <param name="maxLength"></param>
        /// <returns></returns>
        public static string Omit(this string e, int maxLength = 5)
        {
            if (string.IsNullOrWhiteSpace(e))
                return string.Empty;
            var str = e;
            str = str.Length > maxLength ? str.Substring(0, maxLength) + "..." : str;
            return e;
        }

        /// <summary>
        /// 自动格式化
        /// </summary>
        /// <param name="e"></param>
        /// <param name="maxLength"></param>
        /// <returns></returns>
        public static string ToYMdHms(this DateTime? e)
        {
            if (e.HasValue)
            {
                return e.Value.ToString("yyyy/MM/dd HH:mm:ss");
            }
            return string.Empty;
        }

        /// <summary>
        /// 自动格式化
        /// </summary>
        /// <param name="e"></param>
        /// <param name="maxLength"></param>
        /// <returns></returns>
        public static string ToYMdHms(this DateTime e)
        {
          
                return e.ToString("yyyy/MM/dd HH:mm:ss");
        }
    }
}
