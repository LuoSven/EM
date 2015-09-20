using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topuc22Top.Data.Infrastructure;
using Topuc22Top.Model.Entities;
using System.Data.Entity;
using Topuc.Framework.Logger;
using System.Diagnostics;

namespace Topuc22Top.Data.Repositories
{
    public class CompanyActivationRepository : RepositoryBase<Etp_Activation>, ICompanyActivationRepository
    {
        public CompanyActivationRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }

        public IList<Etp_Activation> GetListByEtpId(int? EtpId)
        {
            var query = from a in DataContext.Etp_Activation
                        select a;

            if (EtpId.HasValue)
            {
                query = query.Where(a => a.EtpId == EtpId);
            }
            return query.ToList();
        }


        public async Task<PagedResult<Etp_Activation>> GetListAsync(int page, int pageSize)
        {
            var activedEtpIds = DataContext.TB_Enterprise_Account.Select(e => e.EnterpriseId);
            var query = from a in DataContext.Etp_Activation
                        where !activedEtpIds.Contains(a.EtpId)
                        select a;
            return new PagedResult<Etp_Activation>()
            {
                Results = await query.OrderByDescending(a => a.Id).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(),
                PageSize = pageSize,
                CurrentPage = page,
                RowCount = await query.CountAsync()

            };

        }

    }

    public interface ICompanyActivationRepository : IRepository<Etp_Activation>
    {
        IList<Etp_Activation> GetListByEtpId(int? EtpId);


        Task<PagedResult<Etp_Activation>> GetListAsync(int page, int pageSize);
    }
}
