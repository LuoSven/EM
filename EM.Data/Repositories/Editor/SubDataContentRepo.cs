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
    class SubDataContentRepo : RepositoryBase<SubData_Content>, ISubDataContentRepository
    {

        private readonly ICache cache;
        public SubDataContentRepo(IDatabaseFactory databaseFactory, ICache cache)
            : base(databaseFactory)
        {
            this.cache = cache;
        }

        public IList<SubData_Content> GetListByCataId(int cataId, int count)
        {
            var list = DataContext.SubData_Content.Where(p => p.CataId == cataId).OrderByDescending(p => p.Sort).Take(count).ToList();
            list.ForEach(p => {
                if (string.IsNullOrEmpty(p.Link) || p.Link == "#") 
                {
                    //p.Link = "javascript:void(0);";
                }
            });
            list = list.ToList();
            return list;
        }

        public SubData_Content GetLinkByCataId(int cataId) 
        {
            return (from entity in DataContext.SubData_Content
                    where entity.CataId == cataId
                    orderby entity.Sort descending
                    select entity).FirstOrDefault();
        }
    }

    public interface ISubDataContentRepository : IRepository<SubData_Content>
    {
        IList<SubData_Content> GetListByCataId(int cataId, int count);
        SubData_Content GetLinkByCataId(int cataId);
    }
}
