using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topuc22Top.Data.Infrastructure;
using Topuc22Top.Model.Entities;
using Topuc22Top.Model.DTOs;

namespace Topuc22Top.Data.Repositories
{
    public class MajorIntroduceRepo : RepositoryBase<Major_Introduce>, IMajorIntroduceRepo
    {
        public MajorIntroduceRepo(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {

        }

        public Major_Introduce GetByMajorCode(string majorCode) 
        {
            return (from entity in DataContext.Major_Introduce
                    where entity.MajorCode == majorCode
                    select entity).FirstOrDefault();
        }

        public MajorDTO GetDtoByMajorCode(string majorCode)
        {
            StringBuilder sql=new StringBuilder();
            sql.Append(" select a.Major,a.MajorCode,b.ConferredDegrees,b.Curriculums,b.ReapSkills,b.SchoolingYears,b.TrainingGoal,b.TrainingRequirement,d.Major as ParentMajor  from MajorPosition a");
            sql.Append(" left join Major_Introduce b on a.MajorCode=b.MajorCode ");
            sql.Append(" left join  MajorStandard c on a.MajorCode=c.Code ");
            sql.Append(" left join MajorStandard d on c.ParentCode =d.Code");
            sql.AppendFormat("  where a.MajorCode='{0}'",majorCode);
            return DataContext.Database.SqlQuery<MajorDTO>(sql.ToString()).FirstOrDefault();
        }

    }

    public interface IMajorIntroduceRepo : IRepository<Major_Introduce>
    {
        Major_Introduce GetByMajorCode(string majorCode);
        MajorDTO GetDtoByMajorCode(string majorCode);
    }

}
