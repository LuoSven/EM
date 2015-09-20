using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topuc22Top.Data.Infrastructure;
using Topuc22Top.Data.ResultModel;
using Topuc22Top.Data.ViewModel;
using Topuc22Top.Model.Entities;

namespace Topuc22Top.Data.Repositories
{
    public class SeoEtpNewsRepo : RepositoryBase<TB_Seo_EtpNews>, ISeoEtpNewsRepo
    {
        public SeoEtpNewsRepo(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }

        public List<SEOCompanyNews> GetSeoCompanyNews(int id, int page, int pagesize, out int totalcount)
        {
            var list = from n in DataContext.TB_Seo_EtpNews
                       where n.EnterpriseId == id
                       select new SEOCompanyNews
                       {
                           Title = n.Title,
                           NewsUrl = n.Link,
                           PublishTime = n.PublishTime,
                           Source = n.Source
                       };

            totalcount = list.Count();

            var pagenewslist = list.OrderByDescending(a => a.PublishTime).Skip((page - 1) * pagesize).Take(pagesize).ToList();

            return pagenewslist;

        }
    }

    public interface ISeoEtpNewsRepo : IRepository<TB_Seo_EtpNews>
    {
        List<SEOCompanyNews> GetSeoCompanyNews(int id, int page, int pagesize, out int totalcount);
    }
}
