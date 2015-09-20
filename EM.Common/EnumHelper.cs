using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Reflection;

namespace Topuc22Top.Common
{
    public static class EnumHelper
    {
        /// <summary>
        /// 获取枚举值的详细文本
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static string GetEnumDescription(this object e)
        {
            object objDesc = DataCache.GetCache(e.ToString());
            if (objDesc != null)
            {
                return objDesc.ToString();//获得缓存
            }
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
                        DataCache.SetCache(e.ToString(), dscript.Description);//设置缓存
                        return dscript.Description;
                    }
                }

            }

            //如果没有检测到合适的注释，则用默认名称
            return e.ToString();
        }
        /// <summary>
        /// 扩展方法:使用枚举填充ListControl的Items,枚举类型是int类型
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="control"></param>
        public static void FillItemsByEnum<TEnum>(this ListControl control) where TEnum : struct
        {
            Type theEnum = typeof(TEnum);
            Array values = Enum.GetValues(theEnum);

            for (int i = 0; i < values.Length; i++)
			{
                object value = values.GetValue(i);
                string Text = ((TEnum)value).GetEnumDescription();
                ListItem item = new ListItem(Text, ((int)value).ToString());
                control.Items.Add(item);
			}
        }
    }
}
