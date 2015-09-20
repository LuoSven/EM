using Topuc.Framework.Cache;
using Topuc22Top.Data.Infrastructure;
using Topuc22Top.Model.Entities;
using System.Linq;

namespace Topuc22Top.Data.Repositories
{
    public class EtpCertificateRepo : RepositoryBase<Etp_Certificate>, IEtpCertificateRepo
    {
        private readonly ICache cache;
        public EtpCertificateRepo(IDatabaseFactory databaseFactory, ICache cache)
            : base(databaseFactory)
        {
            this.cache = cache;
        }

        public Etp_Certificate GetByEnterpriseId(int etpId) 
        {
            return (from entity in DataContext.Etp_Certificate
                    where entity.EnterpriseId == etpId
                    select entity).FirstOrDefault();
        }

        public void ReSet(int etpId)
        {
            DataContext.Database.ExecuteSqlCommand(string.Format(@"
    delete from Etp_Certificate where EnterpriseId = {0}
    update TB_Enterprise set CertificationStatus = null where EnterpriseId = {0}
", etpId));
        }

    }

    /// <summary>
    /// 企业认证 审核
    /// </summary>
    public interface IEtpCertificateRepo : IRepository<Etp_Certificate>
    {
        Etp_Certificate GetByEnterpriseId(int etpId);

        void ReSet(int etpId);
    }

}
