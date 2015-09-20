using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topuc22Top.Data.Infrastructure;
using Topuc22Top.Model.Entities;

namespace Topuc22Top.Data.Repositories
{
    public class FileToDeleteRepo : RepositoryBase<File_To_Delete>, IFileToDeleteRepo
    {
        public FileToDeleteRepo(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }
    }
    public interface IFileToDeleteRepo : IRepository<File_To_Delete>
    {
    }
}
