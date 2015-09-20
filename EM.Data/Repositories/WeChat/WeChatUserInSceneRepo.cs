using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topuc22Top.Common;
using Topuc22Top.Data.Infrastructure;
using Topuc22Top.Model.Entities;

namespace Topuc22Top.Data.Repositories
{
    public class WeChatUserInSceneRepo : RepositoryBase<WeChatUsersInScenes>, IWeChatUserInSceneRepo
    {
        public WeChatUserInSceneRepo(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }

        public int GetSceneIdBy(string openId, string AppId)
        {
            int result = 0;
            if (this.DataContext.WeChatUsersInScenes.Any(v => v.OpenId == openId && v.AppId == AppId))
            {
                result = this.GetMany(v => v.OpenId == openId && v.AppId == AppId).SingleOrDefault().SceneId;
            }
            return result;
        }

        public int GetTotalOpenIds(string content,DateTime time)
        {
            var queryFrom = (from s in this.DataContext.WeChatScene
                             join us in this.DataContext.WeChatUsersInScenes on s.SceneId equals us.SceneId
                             where s.SceneContent == content && us.CreateDate <= time
                             select new
                             {
                                 us.OpenId
                             }).Distinct();
            return queryFrom.Count();
        }

        public WeChatUsersInScenes GetUserLastEvent(string openId) 
        {
            return (from entity in DataContext.WeChatUsersInScenes
                    where entity.OpenId == openId
                    select entity).OrderByDescending(p => p.CreateDate).FirstOrDefault();
        }

        public void GetUserSubCountByPeriod(string cityName, DateTime startDate, DateTime endDate, out int subCount, out int unSubCount) 
        {
            subCount = 0; unSubCount = 0;
            var model = (from entity in DataContext.WeChatUsersInScenes
                        where entity.CreateDate >= startDate && entity.CreateDate < endDate
                        && (from scene in DataContext.WeChatScene where scene.SceneContent == cityName select scene.SceneId).Contains(entity.SceneId)
                        group entity by entity.SceneId into g
                        select new
                        {
                            SubCount = g.Where(p => p.EventType == (int)WeChatSceneEventType.Subscribe).Count(),
                            UnSubCount = g.Where(p => p.EventType == (int)WeChatSceneEventType.UnSubscribe).Count(),
                        }).FirstOrDefault();
            if (model != null)
            {
                subCount = model.SubCount;
                unSubCount = model.UnSubCount;
            }
        }

        public IList<Tuple<string, int, int>> GetUserSubCountByPeriod(DateTime startDate, DateTime endDate) 
        {
            var list = new List<Tuple<string, int, int>>() { };

            var models = (from entity in DataContext.WeChatScene
                          join uscene in DataContext.WeChatUsersInScenes.Where(p => p.CreateDate >= startDate && p.CreateDate < endDate)
                          on entity.SceneId equals uscene.SceneId into g
                          select new
                          {
                              SceneContent = entity.SceneContent,
                              SubCount = g.Where(p => p.EventType == (int)WeChatSceneEventType.Subscribe).Count(),
                              UnSubCount = g.Where(p => p.EventType == (int)WeChatSceneEventType.UnSubscribe).Count(),
                          }
                              ).ToList();
            foreach (var i in models)
            {
                list.Add(new Tuple<string, int, int>(i.SceneContent, i.SubCount, i.UnSubCount));
            }
            return list;
        }

    }

    public interface IWeChatUserInSceneRepo : IRepository<WeChatUsersInScenes>
    {
        int GetSceneIdBy(string openId, string AppId);
        int GetTotalOpenIds(string content, DateTime time);
        /// <summary>
        /// 获取用户（微信）最后一次event（也可能上次操作不在记录范围之内）（有场景扫码关注 或 取消关注）
        /// </summary>
        /// <param name="openId"></param>
        /// <returns></returns>
        
        WeChatUsersInScenes GetUserLastEvent(string openId);
        /// <summary>
        /// 获取一段时间内单个场景的关注和取消关注数
        /// </summary>
        /// <param name="subCount"></param>
        /// <param name="unSubCount"></param>
        void GetUserSubCountByPeriod(string cityName, DateTime startDate, DateTime endDate, out int subCount, out int unSubCount);

        /// <summary>
        /// 获取 一段时间之内 各场景 的 扫描 情况
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        IList<Tuple<string,int,int>> GetUserSubCountByPeriod(DateTime startDate, DateTime endDate);

    }
}
