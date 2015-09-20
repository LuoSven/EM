using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topuc22Top.Data.Infrastructure;
using Topuc22Top.Model.Entities;
using System.Data.Entity;

namespace Topuc22Top.Data.Repositories
{
    public class BookRepo : RepositoryBase<TB_Book>, IBookRepo
    {
        public BookRepo(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }

        public IList<TB_Book> GetList(int? count)
        {
            var query = from m in DataContext.TB_Book
                        select m;
            if (count.HasValue)
            {
                return query.OrderByDescending(x => x.ModifyTime).Take(count.Value).ToList();
            }

            return query.OrderByDescending(x => x.ModifyTime).ToList();
        }
    }
    public interface IBookRepo : IRepository<TB_Book>
    {
        /// <summary>
        /// 获取推荐的书籍列表
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        IList<TB_Book> GetList(int? count);
    }
}
