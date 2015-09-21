using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;

namespace EM.Common
{
    /// <summary>
    /// Topuc日期相关方法
    /// </summary>
    public static class TopucDate
    {
        /// <summary>
        /// 22top默认最大日期，若为该时期，则替换为现在
        /// </summary>
        public static DateTime dtMax = new DateTime(9999, 12, 31);

        /// <summary>
        /// 获得年份
        /// </summary>
        /// <param name="p_Date"></param>
        /// <returns></returns>
        public static int GetYear(DateTime p_Date)
        {
            if (p_Date == dtMax)
                return 0;//如果是默认最大日期，则返回0，表示不选中
            else
                return p_Date.Year;
        }
        /// <summary>
        /// 获得月份
        /// </summary>
        /// <param name="p_Date"></param>
        /// <returns></returns>
        public static int GetMonth(DateTime p_Date)
        {
            if (p_Date == dtMax)
                return 0;//如果是默认最大日期，则返回0，表示不选中
            else
                return p_Date.Month;
        }

        public static int GetDay(DateTime p_Date)
        {
            if (p_Date == dtMax)
                return 0;
            else
                return p_Date.Day;
        }

        /// <summary>
        /// 初始化日期
        /// </summary>
        /// <param name="ddlYear"></param>
        /// <param name="ddlMonth"></param>
        /// <param name="p_Date"></param>
        public static void InitDate(DropDownList ddlYear, DropDownList ddlMonth, DateTime p_Date)
        {
            ddlYear.SelectedValue = GetYear(p_Date).ToString();
            ddlMonth.SelectedValue = GetMonth(p_Date).ToString();
        }

        public static void InitDate(DropDownList ddlYear, DropDownList ddlMonth, DropDownList ddlDay, DateTime p_Date)
        {
            ddlYear.SelectedValue = GetYear(p_Date).ToString();
            ddlMonth.SelectedValue = GetMonth(p_Date).ToString();
            ddlDay.SelectedValue = GetDay(p_Date).ToString();
        }

        /// <summary>
        /// 获得年份
        /// </summary>
        /// <param name="p_YearMonth"></param>
        /// <returns></returns>
        public static string GetYearMonth(object p_dt)
        {
            if (p_dt != null)
            {
                try
                {
                    DateTime dt = (DateTime)p_dt;
                    if (dt == dtMax)
                        return "今";
                    else
                        return dt.ToString("yyyy-MM");
                }
                catch
                {
                    return string.Empty;
                }

            }
            return string.Empty;
        }

        public static string GetYearMonth(object p_StartDt, object p_EndDt)
        {
            if (p_StartDt == null || p_EndDt == null)
                return string.Empty;
            if (p_StartDt.Equals(dtMax) && p_EndDt.Equals(dtMax))
                return string.Empty;
            return string.Format("{0} 至 {1}", GetYearMonth(p_StartDt), GetYearMonth(p_EndDt));
        }

        public static void BindDate(DropDownList ddlStartYear, DropDownList ddlStartMonth, DropDownList ddlEndYear, DropDownList ddlEndMonth)
        {
            ddlStartYear.BindYear();
            ddlStartMonth.BindMonth();
            ddlEndYear.BindYear();
            ddlEndMonth.BindMonth();
        }

        public static DateTime BuildDate(DropDownList ddlYear, DropDownList ddlMonth)
        {
            //不判断选中值为null，绑定方法已经选中某行
            return BuildDate(ddlYear.SelectedValue, ddlMonth.SelectedValue);
        }

        /// <summary>
        /// 获得日期
        /// </summary>
        /// <param name="p_Year"></param>
        /// <param name="p_Month"></param>
        /// <returns></returns>
        public static DateTime BuildDate(string p_Year, string p_Month)
        {
            //年月不选，默认至今（22Top默认最大日期）
            //年不选，默认至今
            //月不选，默认 某年1月
            if (p_Year == "0" || p_Month == "0")
            {
                if (p_Year == "0")
                    return dtMax;
                else
                    return new DateTime(int.Parse(p_Year), 1, 1);
            }
            else
                return new DateTime(int.Parse(p_Year), int.Parse(p_Month), 1);
        }

        public static DateTime BuildDate(string year, string month, string day)
        {
            if (year == "0") return dtMax;
            else
            {
                if (month == "0")
                {
                    if (day == "0")
                        return new DateTime(int.Parse(year), 1, 1);
                    else
                        return new DateTime(int.Parse(year), 1, int.Parse(day));
                }
                else
                {
                    if (day == "0")
                        return new DateTime(int.Parse(year), int.Parse(month), 1);
                    else
                        return new DateTime(int.Parse(year), int.Parse(month), int.Parse(day));
                }
            }
        }

        /// <summary>
        /// 初始化年份选择列表
        /// </summary>
        /// <param name="ddl">选择列表</param>
        /// <param name="count">倒数年份数</param>
        /// <param name="num">前推年份数</param>
        /// <param name="item">默认初始文本</param>
        /// <param name="value">默认初始值</param>
        /// <param name="selectValue">选中值</param>
        public static void BindYear(DropDownList ddl, int count, int num, string value, string defaultValue)
        {

            ListItem li0 = new ListItem(defaultValue, value);
            ddl.Items.Add(li0);

            int iAge = DateTime.Now.Year + num;
            int iYear = 0;
            for (int i = 0; i < count; i++)
            {
                iYear = iAge - i;
                ListItem li = new ListItem(iYear.ToString(), iYear.ToString());
                ddl.Items.Add(li);
                if (iYear.ToString() == defaultValue)
                {
                    ddl.SelectedIndex = i;
                }
            }
        }


        public static string BuildDateDiff(DateTime startDate, DateTime endDate)
        {
            TimeSpan ts = endDate.Subtract(startDate);
            double days = ts.Days;
            if (days == 0)
            {
                double hours = ts.Hours;
                if (hours == 0)
                {
                    double mins = ts.Minutes;
                    if (mins == 0)
                    {
                        double seconds = ts.Seconds;
                        return seconds + "秒前";
                    }
                    else
                        return mins + "分钟前";
                }
                else
                    return hours + "小时前";
            }
            else 
            {
                if(days > 0)
                    return days + "天前";
            }
            return string.Empty;
        }
    }
}
