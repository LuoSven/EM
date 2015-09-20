using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topuc22Top.Data.Dapper;
using Topuc22Top.Data.Infrastructure;
using Topuc22Top.Model.Entities;

using Dapper;

namespace Topuc22Top.Data.Repositories
{
    public    class PromotionDailyRepo : RepositoryBase<Promotion_Daily>, IPromotionDailyRepo
    {

        public PromotionDailyRepo(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }

        public void UpdateEtpWithLoginCount(long count, string channel, DateTime date)
        {
            using (var conn = DapperHelper.Get22Connection())
            {
                string sql = string.Format(@"
update Promotion_Daily
set EtpWithLoginCnt = {0}
where PromoterId in (select PromoterId from  Promoter
where Source = '{1}')
and CreateTime = '{2}'", count, channel, date.ToString("yyyy-MM-dd"));
                conn.Execute(sql);
            }
    
        }

        public void UpdateEtpWithPosCreateCount(long count, string channel, DateTime date) 
        {
            using (var conn = DapperHelper.Get22Connection())
            {
                string sql = string.Format(@"
update Promotion_Daily
set EtpWithPosCreateCnt = {0}
where PromoterId in (select PromoterId from  Promoter
where Source = '{1}')
and CreateTime = '{2}'", count, channel, date.ToString("yyyy-MM-dd"));
                conn.Execute(sql);
            }
        }
    }

    public interface IPromotionDailyRepo : IRepository<Promotion_Daily>
    {
        void UpdateEtpWithLoginCount(long count, string channel, DateTime date);
        void UpdateEtpWithPosCreateCount(long count, string channel, DateTime date);
    }
}
