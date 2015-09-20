using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topuc22Top.Data.Dapper;
using Topuc22Top.Data.Infrastructure;
using Topuc22Top.Model.Entities;

using Dapper;
using System.Data;

namespace Topuc22Top.Data.Repositories
{
    public class AssignListRepo : RepositoryBase<AssignList>, IAssignListRepo
    {
        public AssignListRepo(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }

        public int ReAssignEtp(int etpId, string etpName, int assignTo, out string userName)
        {
            using (var conn = DapperHelper.Get22Connection())
            {
                DynamicParameters p = new DynamicParameters();
                p.Add("@etpId", etpId);
                p.Add("@etpName", etpName);
                p.Add("@assignTo", assignTo);
                p.Add("@code", -1, DbType.Int32, ParameterDirection.Output);
                p.Add("@userName", "", DbType.String, ParameterDirection.Output);

                conn.Execute("[dbo].[usp_ReAssignEtp]", p, null, null, CommandType.StoredProcedure);
                int code = p.Get<int>("@code");
                userName = p.Get<string>("@userName");

                return code;
            }
        }

    }

    public interface IAssignListRepo : IRepository<AssignList>
    {
        int ReAssignEtp(int etpId, string etpName, int assignTo, out string userName);
    }

}
