using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topuc22Top.Data.Infrastructure;
using Topuc22Top.Model.Entities;

namespace Topuc22Top.Data.Repositories
{
    public class WeChatUnbindUserInProcessRepo : RepositoryBase<WeChatUnbindUserInProcess>, IWeChatUnbindUserInProcessRepo
    {
        public WeChatUnbindUserInProcessRepo(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }

        public int? GetLastProcess(string openId)
        {
            int? result = null;
            var process = this.DataContext.WeChatUnbindUserInProcess.Where(v => v.OpenId == openId).LastOrDefault();
            if (process != null)
            {
                result = process.Process;
            }
            return result;
        }
    }

    public interface IWeChatUnbindUserInProcessRepo : IRepository<WeChatUnbindUserInProcess>
    {
        int? GetLastProcess(string openId);
    }
}
