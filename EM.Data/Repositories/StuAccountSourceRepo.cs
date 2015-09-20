using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Topuc.Framework.Logger;
using Topuc22Top.Common;
using Topuc22Top.Data.Infrastructure;
using Topuc22Top.Data.ResultModel;
using Topuc22Top.Model.Entities;
using Topuc22Top.Model.ExtendEntities;


namespace Topuc22Top.Data.Repositories
{

    public class StuAccountSourceRepo : RepositoryBase<Stu_Account_Source>, IStuAccountSourceRepo
    {
        public StuAccountSourceRepo(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }

        public async Task CreateAsync(int userId, string promotionVal)
        {
            var source = new Stu_Account_Source() { };
            source.CreateTime = DateTime.Now;
            source.UserId = userId;
            source.Source = promotionVal;

            DataContext.Stu_Account_Source.Add(source);
            DataContext.SaveChanges();
        }

    }


    public interface IStuAccountSourceRepo : IRepository<Stu_Account_Source>
    {
        Task CreateAsync(int userId, string promotionVal);
    }

}
