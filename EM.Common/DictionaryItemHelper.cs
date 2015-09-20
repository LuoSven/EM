using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Topuc22Top.Data;

namespace Topuc22Top.Common
{
    public class DictionaryItemHelper
    {
        public static string GetIndustryName(int id)
        {
            if (id == 0) return string.Empty;
            var ctx = ObjectContextHelper.TopUObjectContext;
            var query = from item in (ctx.DictItem.Where(a => a.Type == "Industry").ToList())
                        where item.ItemId == id
                        select item;
            if (query.Any()) return query.First().ItemName;
            return string.Empty;
        }

        public static string GetCityName(int id)
        {
            if (id == 0) return string.Empty;
            var ctx = ObjectContextHelper.TopUObjectContext;
            var query = from item in (ctx.DictItem.Where(a => a.Type == "City").ToList())
                        where item.ItemId == id
                        select item;
            if (query.Any()) return query.First().ItemName;
            return string.Empty;
        }

        public static string GetFunctionName(int id)
        {
            if (id == 0) return string.Empty;
            var ctx = ObjectContextHelper.TopUObjectContext;
            var query = from item in (ctx.DictItem.Where(a => a.Type == "Function").ToList())
                        where item.ItemId == id
                        select item;
            if (query.Any()) return query.First().ItemName;
            return string.Empty;
        }


        /// <summary>
        /// 扩展功能ID包含自身以及所所有子类ID
        /// </summary>
        /// <param name="id">1001</param>
        /// <returns>1001,1001001,1001002</returns>
        public static string ExtendFunctionIds(string idsStr)
        {
            if (string.IsNullOrEmpty(idsStr)) return string.Empty;
            var ctx = ObjectContextHelper.TopUObjectContext;
            string[] ids = idsStr.Split(',');
            foreach (string idStr in ids)
            {
                int id = Int32.Parse(idStr);
                var query = from item in ctx.DictItem
                            where item.ParentItemId == id && item.Type == "Function"
                            select item.ItemId;
                if (query.Any())//如果找到任何子元素
                {
                    idsStr += "," + string.Join(",", query.ToList());
                }
            }
            return idsStr;
        }

        /// <summary>
        /// 扩展功能ID包含自身以及所所有子类ID,用空格连接
        /// </summary>
        /// <param name="id">1001</param>
        /// <returns>1001 1001001 1001002</returns>
        public static string ExtendFunctionIdsWithBlank(string idsStr)
        {
            if (string.IsNullOrEmpty(idsStr)) return string.Empty;
            var ctx = ObjectContextHelper.TopUObjectContext;
            string[] ids = idsStr.Split(',');
            idsStr = idsStr.Replace(",", " ");
            foreach (string idStr in ids)
            {
                int id = Int32.Parse(idStr);
                var query = from item in ctx.DictItem
                            where item.ParentItemId == id && item.Type == "Function"
                            select item.ItemId;
                if (query.Any())//如果找到任何子元素
                {
                    idsStr += " " + string.Join(" ", query.ToList());
                }
            }
            return idsStr;
        }

        /// <summary>
        /// 扩展城市ID包含自身以及所所有子类ID
        /// </summary>
        /// <param name="id">1001</param>
        /// <returns>1001,1001001,1001002</returns>
        public static string ExtendCityIds(string idsStr)
        {
            if (string.IsNullOrEmpty(idsStr)) return string.Empty;
            var ctx = ObjectContextHelper.TopUObjectContext;
           
            string[] ids = idsStr.Split(',');
            foreach (string idStr in ids)
            {
                int id = Int32.Parse(idStr);
                var query = from item in  (ctx.DictItem.Where(a => a.Type == "City").ToList())
                            where item.ParentItemId == id
                            select item.ItemId;
                if (query.Any())//如果找到任何子元素
                {
                    idsStr += "," + string.Join(",", query.ToList());
                }
            }
            return idsStr;
        }

        /// <summary>
        /// 扩展城市ID包含自身以及所所有子类ID
        /// </summary>
        /// <param name="id">1001</param>
        /// <returns>1001,1001001,1001002</returns>
        public static string ExtendCityIdsWithBlank(string idsStr)
        {
            if (string.IsNullOrEmpty(idsStr)) return string.Empty;
            var ctx = ObjectContextHelper.TopUObjectContext;
            string[] ids = idsStr.Split(',');
            idsStr = idsStr.Replace(",", " ");
            foreach (string idStr in ids)
            {
                int id = Int32.Parse(idStr);
                var query = from item in (ctx.DictItem.Where(a => a.Type == "City").ToList())
                            where item.ParentItemId == id
                            select item.ItemId;
                if (query.Any())//如果找到任何子元素
                {
                    idsStr += " " + string.Join(" ", query.ToList());
                }
            }
            return idsStr;
        }


        /// <summary>
        /// 扩展专业ID包含自身以及所所有子类ID,
        /// </summary>
        /// <param name="id">1001</param>
        /// <returns>1001,1001001,1001002</returns>
        public static string ExtendFormalMajorIds(string idsStr)
        {
            if (string.IsNullOrEmpty(idsStr)) return string.Empty;
            var ctx = ObjectContextHelper.TopUObjectContext;
            string[] ids = idsStr.Split(',');
            foreach (string idStr in ids)
            {
                int id = Int32.Parse(idStr);
                var query = from item in ctx.FormalMajor
                            where item.PId == id
                            select item.Id;
                //if (query.Any())//如果找到任何子元素
                //{
                //    //专业有三层父子关系,多跑一次循环
                //    foreach (var subid in query.ToList())
                //    {
                //        idsStr += "," + subid;
                //        var query2 = from item in ctx.FormalMajor
                //                     where item.PId   == subid
                //                     select item.Id;
                //        if (query2.Any())
                //        {
                //            idsStr += "," + string.Join(",", query2.ToList());
                //        }
                //    }
                //}

                idsStr += "," + string.Join(",", query.ToList());
            }
            return idsStr;
        }



        /// <summary>
        /// 扩展专业ID包含自身以及所二级子类ID,
        /// </summary>
        /// <param name="id">1001</param>
        /// <returns>1001,1001001,1001002</returns>
        public static string ExtendSecondFormalMajorIds(string idsStr)
        {
            if (string.IsNullOrEmpty(idsStr)) return string.Empty;
            var ctx = ObjectContextHelper.TopUObjectContext;
            string[] ids = idsStr.Split(',');
            foreach (string idStr in ids)
            {
                int id = Int32.Parse(idStr);
                var query = from item in ctx.FormalMajor
                            where item.PId == id && item.PId < 1000
                            select item.Id;
                //if (query.Any())//如果找到任何子元素
                //{
                //    //专业有三层父子关系,多跑一次循环
                //    foreach (var subid in query.ToList())
                //    {
                //        idsStr += "," + subid;
                //        var query2 = from item in ctx.FormalMajor
                //                     where item.PId   == subid
                //                     select item.Id;
                //        if (query2.Any())
                //        {
                //            idsStr += "," + string.Join(",", query2.ToList());
                //        }
                //    }
                //}

                idsStr += "," + string.Join(",", query.ToList());
            }
            return idsStr;
        }



        public static string ExtendOldInduIds(string idsStr)
        {
            if (string.IsNullOrEmpty(idsStr)) return string.Empty;
            var ctx = ObjectContextHelper.TopUObjectContext;
            string[] ids = idsStr.Split(',');
            List<int> intids = new List<int>();
            string OldFuncStr = "";

            //idsStr = idsStr.Replace(",", " ");
            foreach (string idStr in ids)
            {
                int id = Int32.Parse(idStr);
                intids.Add(id);

            }
            var query = from item in (ctx.DictItem.Where(a => a.Type == "SearchIndu").ToList())
                        where intids.Contains(item.ItemId)
                        select item.ItemValue;
            if (query.Any())//如果找到任何子元素
            {
                OldFuncStr += " " + string.Join(" ", query.ToList());
            }
            OldFuncStr = OldFuncStr.Replace(",", " ");
            OldFuncStr = OldFuncStr.Trim();
            return OldFuncStr;
        }
    }
}
