using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topuc22Top.Data.Infrastructure;
using Topuc22Top.Model.Entities;
using System.Data.Entity;
using Topuc.Framework.Cache;
using Topuc.Framework.Logger;
using System.Diagnostics;

namespace Topuc22Top.Data.Repositories
{
    /// <summary>
    /// 热搜职位、功能
    /// </summary>
    public class FunctionHotSearchRepo : RepositoryBase<TB_Function_HotSearch>, IFunctionHotSearchRepository
    {
        private readonly ICache cache;
#if Debug
        private readonly int  cacheMinutes=1;
#else
        private readonly int cacheMinutes = 60;
#endif

        public FunctionHotSearchRepo(IDatabaseFactory databaseFactory, ICache cache)
            : base(databaseFactory)
        {
            this.cache = cache;
        }

        /// <summary>
        /// 根据功能ID数组获取 热搜职位对象
        /// </summary>
        /// <param name="functionID"></param>
        /// <param name="model">model==1位职位详情页的热搜职位</param>
        /// <returns></returns>
        public IList<TB_Function_HotSearch> GetByFunctions(int[] functionIds,int model = 1)
        {
            return cache.Get(string.Format(Settings.ParaKey_HotArticleTagList, string.Join(",",functionIds)), () =>
            {
                var query = from m in this.DataContext.TB_Function_HotSearch
                            where
                            m.Mode == model
                            && functionIds.Contains(m.FunctionId)
                            select m;
                return query.ToList<TB_Function_HotSearch>();
            }, cacheMinutes);
        }

        public async Task<IList<TB_Function_HotSearch>> GetByFunctionsAsync(int[] functionIds, int model = 1)
        {
            using (TopucDB DataContext = new TopucDB())
            {
                return await DataContext.TB_Function_HotSearch.Where(m => m.Mode == model && functionIds.Contains(m.FunctionId)).ToListAsync<TB_Function_HotSearch>();
            }
        }

        public async Task<IList<string>> GetHotSearchPositionByFunctionAsync(string functionIds)
        {

            Stopwatch sw = new Stopwatch();
            sw.Start();
            string[] functionIdsArray = string.IsNullOrWhiteSpace(functionIds) ? new string[] { } : functionIds.Split(',');
            int temp;
            var arr = functionIdsArray.Where(m => int.TryParse(m, out temp)).Select(m => int.Parse(m)).ToArray<int>();
            IList<TB_Function_HotSearch> hotsearchPositionList = await GetByFunctionsAsync(arr);
            List<string> list = new List<string>();
            if (hotsearchPositionList != null)
            {
                foreach (var p in hotsearchPositionList)
                {
                    if (!string.IsNullOrWhiteSpace(p.KeyWords))
                    {
                        list.AddRange(p.KeyWords.Split(new string[] { ",", "，", "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList<string>());
                    }
                }
            }

            AppLogger.Info("查询公司热点职位共花费：" + sw.ElapsedMilliseconds + "ms, functionId:" + functionIds.ToString());
            return list.Distinct<string>().ToList<string>();
        }

        public  IList<string> GetHotSearchPositionByFunction(string functionIds)
        {
            string[] functionIdsArray = string.IsNullOrWhiteSpace(functionIds) ? new string[] { } : functionIds.Split(',');
            int temp;
            var arr = functionIdsArray.Where(m => int.TryParse(m, out temp)).Select(m => int.Parse(m)).ToArray<int>();
            IList<TB_Function_HotSearch> hotsearchPositionList =  GetByFunctions(arr);
            List<string> list = new List<string>();
            if (hotsearchPositionList != null)
            {
                foreach (var p in hotsearchPositionList)
                {
                    if (!string.IsNullOrWhiteSpace(p.KeyWords))
                    {
                        list.AddRange(p.KeyWords.Split(new string[] { ",", "，", "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList<string>());
                    }
                }
            }
            return list.Distinct<string>().ToList<string>();
        }
    }

    public interface IFunctionHotSearchRepository : IRepository<TB_Function_HotSearch>
    {
        IList<TB_Function_HotSearch> GetByFunctions(int[] functionIds,int model = 1);

        Task<IList<TB_Function_HotSearch>> GetByFunctionsAsync(int[] functionIds, int model = 1);

        /// <summary>
        /// 根据职能获取热搜职位
        /// </summary>
        /// <param name="functionIds"></param>
        /// <returns></returns>
        Task<IList<string>> GetHotSearchPositionByFunctionAsync(string functionIds);

        IList<string> GetHotSearchPositionByFunction(string functionIds);
    }
}
