using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topuc.Framework.Cache;
using Topuc22Top.Data.Infrastructure;
using Topuc22Top.Model.Entities;

namespace Topuc22Top.Data.Repositories
{
    public class RichEditorRepo : RepositoryBase<Editor_RichText>, IRichEditorRepo
    {
        private readonly ICache cache;
        public RichEditorRepo(IDatabaseFactory databaseFactory, ICache cache)
            : base(databaseFactory)
        {
            this.cache = cache;
        }

        public string GetContentStrById(string pageName, string place)
        {
            //return cache.Get(string.Format(Settings.ParaKey_CityAreaInfo, pageName, place), () =>
            //{
            var entity = (from m in DataContext.Editor_RichText
                          where m.PageName == pageName && m.Place == place
                          select m).FirstOrDefault();
            if (entity != null)
            {
                return entity.ContentStr;
            }
            else
            {
                //需创建编辑位
                //var context = ObjectContextHelper.TopUObjectContext;
                var obj = new Editor_RichText()
                {
                    ID = 0,
                    PageName = pageName,
                    Place = place,
                    ContentStr = ""
                };
                DataContext.Editor_RichText.Add(obj);
                DataContext.SaveChanges();
                return "";
            }
            //}, 60 *12);
        }
    }

    public interface IRichEditorRepo : IRepository<Editor_RichText>
    {
        string GetContentStrById(string pageName, string place);
    }
}
