using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topuc22Top.Data.Infrastructure;
using Topuc22Top.Model.Entities;

namespace Topuc22Top.Data.Repositories
{
    public class JobTemplateRepo : RepositoryBase<TB_Position_Template>, IJobTemplateRepo
    {
        public JobTemplateRepo(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }
        public Dictionary<int, string> GetList(int enterpriseId)
        {
            var query = from m in DataContext.TB_Position_Template
                        where m.CorporationId == enterpriseId
                        select new
                        {
                            TemplateId = m.TemplateId,
                            TemplateName = m.TemplateName
                        };
            return query.ToDictionary(x => x.TemplateId, y => y.TemplateName);
        }

        public PagedResult<TB_Position_Template> GetList(int enterpriseId, int page, int pageSize)
        {
            var query = from m in DataContext.TB_Position_Template
                        where m.CorporationId == enterpriseId
                        select m;
            return new PagedResult<TB_Position_Template>()
            {
                CurrentPage = page,
                PageSize = pageSize,
                RowCount = query.Count(),
                Results = query.OrderByDescending(f => f.TemplateId).Skip(pageSize * (page - 1)).Take(pageSize).ToList()
            };
        }
        public int GetTemplateNameCount(int enterpriseId, string templateName)
        {
            var query = from m in DataContext.TB_Position_Template
                        where m.CorporationId == enterpriseId && m.TemplateName.Contains(templateName)
                        select m;
            return query.Count();
        }
    }
    public interface IJobTemplateRepo : IRepository<TB_Position_Template>
    {
        Dictionary<int, string> GetList(int enterpriseId);
        PagedResult<TB_Position_Template> GetList(int enterpriseId, int page, int pageSize);
        int GetTemplateNameCount(int enterpriseId, string templateName);
    }
}
