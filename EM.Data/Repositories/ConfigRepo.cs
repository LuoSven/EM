using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topuc22Top.Data.Infrastructure;
using Topuc22Top.Model.Entities;
using System.Data.Entity;
using Topuc22Top.Common;

namespace Topuc22Top.Data.Repositories
{

    public class ConfigRepo : RepositoryBase<TB_Config>, IConfigRepo
    {
        public ConfigRepo(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }

        public string GetFamousWall()
        {
            return DataContext.TB_Config.Where(x => x.Type == (int)ConfigType.FamousWall).Select(x => x.Keys).FirstOrDefault();
        }

        public string GetMobileHomeJobs()
        {
            return DataContext.TB_Config.Where(x => x.Type == (int)ConfigType.MobileHomeJobs).Select(x => x.Keys).FirstOrDefault();
        }


        public string GetHotEtps()
        {
            var query = DataContext.TB_Config.Where(x => x.Type == (int)ConfigType.HotEtps).Select(x => x.Keys);
            if (query.Any())
            {
                return query.FirstOrDefault().Replace("\r\n",",");
            }
            return string.Empty;
        }

        /// <summary>
        /// 根据类型返回第一条记录
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public TB_Config GetByType(int type)
        {
            var query = from m in this.DataContext.TB_Config
                        where m.Type == type
                        select m;
            return query.FirstOrDefault<TB_Config>();
        }

    }
    public interface IConfigRepo : IRepository<TB_Config>
    {
        /// <summary>
        /// 获取名企墙的企业ID集合
        /// </summary>
        /// <returns></returns>
        string GetFamousWall();

        string GetMobileHomeJobs();

        string GetHotEtps();

        /// <summary>
        /// 根据类型返回第一条记录
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        TB_Config GetByType(int type);
    }
}
