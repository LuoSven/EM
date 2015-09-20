using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topuc22Top.Common;
using Topuc22Top.Data.Infrastructure;
using Topuc22Top.Data.ResultModel;
using Topuc22Top.Model.Entities;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using Topuc.Framework.Cache;
using Topuc22Top.Model.DTOs;
using Topuc22Top.Data.Dapper;
using Dapper;

namespace Topuc22Top.Data.Repositories
{
    public class ArticleRepo : RepositoryBase<TB_Article>, IArticleRepo
    {
        private readonly ICache cache;
#if Debug
        private readonly int  cacheMinutes=1;
#else
        private readonly int cacheMinutes = 60;
#endif

        public ArticleRepo(IDatabaseFactory databaseFactory, ICache cache)
            : base(databaseFactory)
        {
            this.cache = cache;
        }

        public IList<ArticleDetailResult> GetHotArticleList(IList<int> articleTypes, int count)
        {
            var query = from ta in DataContext.TB_Article
                        where ta.IsActive && articleTypes.Contains(ta.ArticleType)
                        select new ArticleDetailResult
                        {
                            ArticleId = ta.ArticleId,
                            ArticleTitle = ta.ArticleTitle,
                            ArticleAuthor = ta.ArticleAuthor,
                            ArticleType = ta.ArticleType,
                            Favor = ta.Favor.HasValue ? ta.Favor.Value : 0,
                            IsActive = ta.IsActive,
                            Times = 0,
                            CreateDate = ta.CreateDate,
                            ModifyDate = ta.ModifyDate,
                            UrlHead = ta.UrlHead
                        };
            return query.OrderBy(a => Guid.NewGuid()).Take(count).ToList();
        }
        public ArticleDetailResult GetArticleDetailByArticleId(int articleId)
        {
            var query = from ta in DataContext.TB_Article
                        where ta.ArticleId == articleId
                        select new ArticleDetailResult
                        {
                            ArticleId = ta.ArticleId,
                            ArticleTitle = ta.ArticleTitle,
                            ArticleAuthor = ta.ArticleAuthor,
                            ArticleType = ta.ArticleType,
                            ArticleContent = ta.ArticleContent,
                            Favor = ta.Favor.HasValue ? ta.Favor.Value : 0,
                            IsActive = ta.IsActive,
                            Times = 0,
                            CreateDate = ta.CreateDate,
                            ModifyDate = ta.ModifyDate,
                            UrlHead = ta.UrlHead
                        };
            return query.FirstOrDefault();
        }

        #region 新求职攻略使用

        public int GetTagCount(string tag, ArticleType type = ArticleType.JobGuidance)
        {
            var count = (from a in DataContext.TB_Article
                         where ("," + a.ArticleTags + ",").Contains("," + tag + ",")
                         && a.ArticleType == (int)type
                         select a).Count();

            return count;
        }

        public IList<Article_Tag> GetHotTags(ArticleType type = ArticleType.JobGuidance)
        {
            return cache.Get(string.Format(Settings.ParaKey_HotArticleTagList, type), () =>
            {
                IList<Article_Tag> list = new List<Article_Tag>();
                string tags = string.Empty;
                if (type == ArticleType.JobGuidance)
                {
                    tags = DataContext.TB_Config.Where(x => x.Type == (int)ConfigType.HotTag).Select(x => x.Keys).FirstOrDefault();
                }
                else
                {
                    tags = DataContext.TB_Config.Where(x => x.Type == (int)ConfigType.HotHrTag).Select(x => x.Keys).FirstOrDefault();
                }
                if (tags != null)
                {
                    var array = tags.Replace("\r\n", " ").Split(' ');
                    list = DataContext.Article_Tag.Where(t => array.Contains(t.Name)).ToList();
                }
                return list;
            }, 3600);
        }

        public Dictionary<int, string> GetHotArticles(ArticleType type = ArticleType.JobGuidance)
        {
            return cache.Get(string.Format(Settings.ParaKey_HotArticleList, type), () =>
           {
               Dictionary<int, string> list = new Dictionary<int, string>();

               var articleIds = string.Empty;
               if (type == ArticleType.JobGuidance)
               {
                   articleIds = DataContext.TB_Config.Where(x => x.Type == (int)ConfigType.HotArticle).Select(x => x.Keys).FirstOrDefault();
               }
               else
               {
                   articleIds = DataContext.TB_Config.Where(x => x.Type == (int)ConfigType.HotHrArticle).Select(x => x.Keys).FirstOrDefault();
               }

               if (articleIds != null)
               {
                   var ids = GetIntList(articleIds);
                   foreach (var m in ids)
                   {
                       var title = (from a in DataContext.TB_Article
                                    where a.ArticleId == m
                                    select a.ArticleTitle).FirstOrDefault();
                       list.Add(m, title);
                   }
               }

               return list;
           }, 3600);
        }

        public IList<RecommendedArticleResult> GetRecommendedArticles(ArticleType type = ArticleType.JobGuidance)
        {
            IList<RecommendedArticleResult> list = new List<RecommendedArticleResult>();

            string articleIds = string.Empty;
            if (type == ArticleType.JobGuidance)
            {
                articleIds = DataContext.TB_Config.Where(x => x.Type == (int)ConfigType.RecommendedArticle).Select(x => x.Keys).FirstOrDefault();
            }
            else
            {
                articleIds = DataContext.TB_Config.Where(x => x.Type == (int)ConfigType.RecommendedHrArticle).Select(x => x.Keys).FirstOrDefault();
            }
            if (articleIds != null)
            {
                var ids = GetIntList(articleIds);
                foreach (var m in ids)
                {
                    RecommendedArticleResult result = new RecommendedArticleResult() { ArticleId = m };
                    var article = (from a in DataContext.TB_Article
                                   where a.ArticleId == m
                                   select a).FirstOrDefault();
                    if (article != null)
                    {
                        result.ArticleTitle = article.ArticleTitle;
                        result.Thumb = article.Thumb;
                    }
                    list.Add(result);
                }
            }

            return list;
        }

        public IList<TB_Article> GetArticleList(string tag, int page, int pageSize, out int count, ArticleType type = ArticleType.JobGuidance)
        {
            var query = (from m in DataContext.TB_Article
                         select m).Where(m => m.ArticleType == (int)type);
            if (!string.IsNullOrEmpty(tag))
            {
                query = query.Where(x => ("," + x.ArticleTags + ",").Contains("," + tag + ","));
            }

            count = query.Count();
            return query.OrderByDescending(x => x.CreateDate).ThenByDescending(x => x.ArticleId).Skip(pageSize * (page - 1)).Take(pageSize).ToList();
        }

        public PagedResult<TB_Article> GetArticleList(string tag, int page, int pageSize, ArticleType type = ArticleType.JobGuidance)
        {
            var query = (from m in DataContext.TB_Article
                         select m).Where(m => m.ArticleType == (int)type);
            if (!string.IsNullOrEmpty(tag))
            {
                query = query.Where(x => ("," + x.ArticleTags + ",").Contains("," + tag + ","));
            }
            int count = query.Count();
            return new PagedResult<TB_Article>()
            {
                Results = query.OrderByDescending(x => x.CreateDate).ThenByDescending(x => x.ArticleId).Skip(pageSize * (page - 1)).Take(pageSize).ToList(),
                PageSize = pageSize,
                CurrentPage = page,
                RowCount = count
            };
        }



        public IList<TB_Article> GetExperienceArticleList(string exceptIdsStr, int count, out int totalcount)
        {
            var query = (from m in DataContext.TB_Article
                         select m).Where(m => m.ArticleType == (int)ArticleType.Expirence);
            if (!string.IsNullOrEmpty(exceptIdsStr))
            {
                string[] exceptIdsStrArray = exceptIdsStr.Split(',');
                int[] exceptIdsIntArray = Array.ConvertAll<string, int>(exceptIdsStrArray, delegate(string s) { int returnnum = 0; int.TryParse(s, out returnnum); return returnnum; });

                query = query.Where(a => !exceptIdsIntArray.Contains(a.ArticleId));
            }

            totalcount = query.Count();
            return query.OrderByDescending(x => x.CreateDate).ThenByDescending(x => x.ArticleId).Take(count).ToList();
        }

        public Tuple<int, int> GetArticlePreviousNextId(int articleId, ArticleType type)
        {
            var query = (from m in DataContext.TB_Article
                         select m).Where(m => m.ArticleType == (int)type);
            int previousId = query.Where(a => a.ArticleId > articleId).OrderBy(a => a.CreateDate).Select(a => a.ArticleId).FirstOrDefault();
            int nextId = query.Where(a => a.ArticleId < articleId).OrderByDescending(a => a.CreateDate).ThenByDescending(x => x.ArticleId).Select(a => a.ArticleId).FirstOrDefault();

            return Tuple.Create(previousId, nextId);

        }


        public Dictionary<int, string> GetRelatedArticles(int articleId, ArticleType type = ArticleType.JobGuidance)
        {
            Dictionary<int, string> list = new Dictionary<int, string>();

            ObjectParameter par = new ObjectParameter("Ids", string.Empty);
            //这一步10+ms
            IList<string> result = DataContext.usp_GetRelatedArticleIds(articleId, par).ToList();

            string articleIds = result[0];
            //var articleIds = DataContext.fun_getRelatedArticleIds(articleId);

            if (!string.IsNullOrEmpty(articleIds))
            {
                var ids = GetIntList(articleIds);
                (from a in DataContext.TB_Article
                 where ids.Contains(a.ArticleId) && a.ArticleTitle != null && a.ArticleTitle != ""
                 select new
                 {
                     Id = a.ArticleId,
                     Title = a.ArticleTitle
                 }).ToList().ForEach(p => list.Add(p.Id, p.Title));
            }

            return list;

        }

        public Dictionary<int, string> GetArticleIdAndTitleList(ConfigType type)
        {
            Dictionary<int, string> list = new Dictionary<int, string>();
            var articleIds = DataContext.TB_Config.Where(x => x.Type == (int)type).Select(x => x.Keys).FirstOrDefault();

            if (articleIds != null)
            {
                var ids = GetIntList(articleIds);
                foreach (var m in ids)
                {
                    var title = (from a in DataContext.TB_Article
                                 where a.ArticleId == m
                                 select a.ArticleTitle).FirstOrDefault();
                    if (!string.IsNullOrEmpty(title))
                    {
                        list.Add(m, title);
                    }
                }
            }

            return list;
        }

        public Dictionary<int, string> GetArticleIdAndThumbList(ConfigType type)
        {
            Dictionary<int, string> list = new Dictionary<int, string>();
            var articleIds = DataContext.TB_Config.Where(x => x.Type == (int)type).Select(x => x.Keys).FirstOrDefault();

            if (articleIds != null)
            {
                var ids = GetIntList(articleIds);
                foreach (var m in ids)
                {
                    var thumb = (from a in DataContext.TB_Article
                                 where a.ArticleId == m
                                 select a.Thumb).FirstOrDefault();
                    if (!string.IsNullOrEmpty(thumb))
                    {
                        list.Add(m, thumb);
                    }
                }
            }

            return list;
        }

        public string GetArticleTitle(int articleId)
        {
            if (articleId == 0) return string.Empty;

            var query = from m in DataContext.TB_Article
                        where m.ArticleId == articleId
                        select m.ArticleTitle;
            return query.FirstOrDefault();
        }

        #endregion

        private int[] GetIntList(string idStr)
        {
            if (!string.IsNullOrEmpty(idStr))
            {
                var array = idStr.Trim().Replace("\r\n", ",").Replace("，", ",");
                return ArrayConvertor.Convert(array);
                //int len = array.Length;
                //int[] idlist = new int[len];
                //for (int i = 0; i < len; i++)
                //{
                //    idlist[i] = Int32.Parse(array[i]);
                //}
                //return idlist;
            }
            return new int[] { };
        }


        public Dictionary<int,string> GetTagItems(string tagNames)
        {
            Dictionary<int, string> items = new Dictionary<int, string>();
            if (!string.IsNullOrEmpty(tagNames))
            {
                var array = tagNames.Split(',');
                foreach(var name in array)
                {
                    var key = GetTagIdByName(name);
                    if (!items.ContainsKey(key)) 
                    {
                        items.Add(key, name);
                    }
                }
            }
            return items;
        }


        public string GetTagName(int id)
        {
            return cache.Get(string.Format(Settings.ParaKey_ArticleTag, id), () =>
            {
                var query = DataContext.Article_Tag.Where(t => t.TagId == id).Select(t => t.Name);
                if (query.Any())
                {
                    return query.FirstOrDefault();
                }
                return string.Empty;
            }, 3600);
        }

        private int GetTagIdByName(string name)
        {
            return cache.Get(string.Format(Settings.ParaKey_TagName, name), () =>
            {
                var query = DataContext.Article_Tag.Where(t => t.Name == name).Select(t => t.TagId);
                if (query.Any())
                {
                    return query.FirstOrDefault();
                }
                return 0;
            }, 3600);
        }


        public ArticleDTO GetDetails(int id)
        {
            using(var conn = DapperHelper.Get22Connection())
            {
                string sql = "select * from TB_Article where ArticleId=@id";//select*
                var model = conn.Query<ArticleDTO>(sql, new { id = id }).FirstOrDefault();

                sql = "select ArticleId [Key],ArticleTitle [Value] from TB_Article where ArticleId<@id and ArticleType=@type order by CreateDate desc,ArticleId desc";
                model.Previous = conn.Query<KeyValuePair<int, string>>(sql, new { id = id, type = model.ArticleType }).FirstOrDefault();

                sql = "select ArticleId [Key],ArticleTitle [Value] from TB_Article where ArticleId>@id and ArticleType=@type order by CreateDate,ArticleId";
                model.Next = conn.Query<KeyValuePair<int, string>>(sql, new { id = id, type = model.ArticleType }).FirstOrDefault();

                model.TagList = new List<KeyValuePair<int, string>>();
                if(!string.IsNullOrEmpty(model.ArticleTags))
                {
                    string[] arr = model.ArticleTags.Split(',');
                    sql = "select TagId [Key],Name [Value] from Article_Tag where Name in (select Item from dbo.SplitToStrArray(@tags,','))";
                    model.TagList = conn.Query<KeyValuePair<int, string>>(sql, new { tags = model.ArticleTags }).ToList();
                }

                return model;

            }
        }

    }
    public interface IArticleRepo : IRepository<TB_Article>
    {
        ArticleDTO GetDetails(int id);
        Dictionary<int, string> GetArticleIdAndTitleList(ConfigType type);

        Dictionary<int, string> GetArticleIdAndThumbList(ConfigType type);
        /// <summary>
        /// 获取热门文章
        /// </summary>
        /// <param name="articleTypes">文章类型</param>
        IList<ArticleDetailResult> GetHotArticleList(IList<int> articleTypes, int count);
        /// <summary>
        /// 获取求职指导文章详细内容
        /// </summary>
        ArticleDetailResult GetArticleDetailByArticleId(int articleId);


        #region 新求职攻略使用

        /// <summary>
        /// 获取热门标签集合
        /// </summary>
        /// <returns></returns>
        IList<Article_Tag> GetHotTags(ArticleType type = ArticleType.JobGuidance);
        /// <summary>
        /// 获取热门文章集合
        /// </summary>
        /// <returns></returns>
        Dictionary<int, string> GetHotArticles(ArticleType type = ArticleType.JobGuidance);
        /// <summary>
        /// 获取重点推荐文章集合
        /// </summary>
        /// <returns></returns>
        IList<RecommendedArticleResult> GetRecommendedArticles(ArticleType type = ArticleType.JobGuidance);
        /// <summary>
        /// 获取文章列表
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        IList<TB_Article> GetArticleList(string tag, int page, int pageSize, out int count, ArticleType type = ArticleType.JobGuidance);


        /// <summary>
        /// 获取文章列表
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        PagedResult<TB_Article> GetArticleList(string tag, int page, int pageSize, ArticleType type = ArticleType.JobGuidance);
        /// <summary>
        /// 统计包含该标签的文章数
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        int GetTagCount(string tag, ArticleType type = ArticleType.JobGuidance);
        /// <summary>
        /// 获取相关文章
        /// </summary>
        /// <returns></returns>
        Dictionary<int, string> GetRelatedArticles(int articleId, ArticleType type = ArticleType.JobGuidance);

        /// <summary>
        /// 获取猎头说的文章
        /// </summary>
        /// <param name="exceptIdsStr">排除的id</param>
        /// <param name="count"></param>
        /// <param name="totalcount"></param>
        /// <returns></returns>
        IList<TB_Article> GetExperienceArticleList(string exceptIdsStr, int count, out int totalcount);

        /// <summary>
        /// 获得上一篇和下一篇的文章id
        /// 2014-8-7日Green改写添加type参数
        /// </summary>
        /// <param name="articleId"></param>
        /// <returns></returns>
        Tuple<int, int> GetArticlePreviousNextId(int articleId, ArticleType type);

        string GetArticleTitle(int articleId);

        #endregion

        Dictionary<int, string> GetTagItems(string tagNames);


        /// <summary>
        /// 根据ID获取Tag名称
        /// </summary>
        string GetTagName(int id);
    }
}
