using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Topuc.Framework.Cache;
using Topuc22Top.Common;
using Topuc22Top.Data.Infrastructure;
using Topuc22Top.Data.ResultModel;
using Topuc22Top.Model.Entities;
using System.Data.Entity;


namespace Topuc22Top.Data.Repositories
{
    public class EtpActivationRepo : RepositoryBase<Etp_Activation>, IEtpActivationRepo
    {
        private readonly ICache cache;
        public EtpActivationRepo(IDatabaseFactory databaseFactory, ICache cache)
            : base(databaseFactory)
        {
            this.cache = cache;
        }

        public void UpdateStatus(int etpId, EtpActivationStatus status)
        {
            var query = (from a in DataContext.Etp_Activation
                         where a.EtpId == etpId
                         select a).ToList();
            foreach (var item in query)
            {
                item.ActivationStatus = (int)status;
            }
        }

    }

    public interface IEtpActivationRepo : IRepository<Etp_Activation>
    {
        void UpdateStatus(int etpId, EtpActivationStatus status);
    }

}
