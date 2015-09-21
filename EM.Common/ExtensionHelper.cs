using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Reflection;
using System.Web.UI;
namespace EM.Common
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
    }
}
