using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topuc22Top.Data.Infrastructure;
using Topuc22Top.Model.Entities;

namespace Topuc22Top.Data.Repositories
{
    public class WeChatSceneRepo : RepositoryBase<WeChatScene>, IWeChatSceneRepo
    {
        public WeChatSceneRepo(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }

        public WeChatScene GetSceneBySceneId(int id)
        {
            return this.GetAll().Where(v => v.SceneId == id).SingleOrDefault();
        }
    }

    public interface IWeChatSceneRepo : IRepository<WeChatScene>
    {
        WeChatScene GetSceneBySceneId(int id);
    }
}
