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
    public class ApplyEtpNoteRepo : RepositoryBase<Apply_EtpNote>, IApplyEtpNoteRepo
    {
        public ApplyEtpNoteRepo(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }


        public IList<Apply_EtpNote> GetNoteListByApplyId(int applyId) 
        {
            return (from entity in DataContext.Apply_EtpNote
                    where entity.ApplyId == applyId
                    select entity).OrderByDescending(p => p.CreateDate).ToList();
        }

    }


    public interface IApplyEtpNoteRepo : IRepository<Apply_EtpNote>
    {
        IList<Apply_EtpNote> GetNoteListByApplyId(int applyId);
    }

}
