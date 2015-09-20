using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topuc22Top.Data.Infrastructure;
using Topuc22Top.Data.ResultModel;
using Topuc22Top.Model.DTOs;
using Topuc22Top.Model.Entities;

namespace Topuc22Top.Data.Repositories
{
    public class StuEducationRepo : RepositoryBase<TB_Stu_Education>, IStuEducationRepo
    {
        public StuEducationRepo(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }

        public TB_Stu_Education GetLastEdu(int userId)
        {
            var query = from e in DataContext.TB_Stu_Education
                        where e.UserId == userId
                        select e;
            return query.OrderByDescending(e => e.StartDate).FirstOrDefault();

        }


        public async Task<EducationDTO> GetByIdAsync(int id)
        {
            string sql = string.Format("select * from TB_Stu_Education  where Id= {0}", id);//select*
            return await DataContext.Database.SqlQuery<EducationDTO>(sql).FirstOrDefaultAsync();
        }


        public async Task<List<EducationDTO>> FindListByUserAsync(int userId)
        {
            string sql = string.Format("select *  from TB_Stu_Education  where UserId= {0} order by StartDate desc", userId);//select*
            return await DataContext.Database.SqlQuery<EducationDTO>(sql).ToListAsync();
        }

        public async Task<EducationDTO> FindLatestEduByUserAsync(int userId)
        {
            string sql = string.Format("select * from TB_Stu_Education  where UserId= {0} order by StartDate desc", userId);//select*
            return await DataContext.Database.SqlQuery<EducationDTO>(sql).FirstOrDefaultAsync();
        }

        public IList<string> FindEduSchoolNamesByUser(int userId)
        {
            string sql = string.Format("select distinct SchoolName from TB_Stu_Education  where UserId= {0} order by SchoolName", userId);
            return DataContext.Database.SqlQuery<string>(sql).ToList();
        }


        public async Task CreateAsync(EducationDTO edu)
        {
            TB_Stu_Education model = new TB_Stu_Education()
            {
                StartDate = edu.StartDate,
                UserId = edu.UserId,
                EndDate = edu.EndDate,
                SchoolName = edu.SchoolName,
                Major = edu.Major,
                Degree = edu.Degree,
                IsEntranceExamination = edu.IsEntranceExamination,
                CreateDate = DateTime.Now,
                ModifyDate = DateTime.Now
            };
            #region 根据学校名称获得学校ID，根据专业名称获得专业Code
            await UpdateMajorCode(edu, model);
            await UpdateSchoolId(edu, model);
            #endregion

            //DataContext.TB_Stu_Education.Add(model);
            //DataContext.SaveChanges();

            var result = await Dapper.DapperHelper.InsertAsync(model);
        }

        public async Task UpdateAsync(EducationDTO edu)
        {
            var model = await DataContext.TB_Stu_Education.FindAsync(edu.Id);
            model.StartDate = edu.StartDate;
            model.EndDate = edu.EndDate;
            model.Degree = edu.Degree;
            model.IsEntranceExamination = edu.IsEntranceExamination;
            model.ModifyDate = DateTime.Now;

            #region 根据学校名称获得学校ID，根据专业名称获得专业Code
            if (model.Major != edu.Major)
            {
                model.Major = edu.Major;
                await UpdateMajorCode(edu, model);
            }

            if (model.SchoolName != edu.SchoolName)
            {
                model.SchoolName = edu.SchoolName;
                await UpdateSchoolId(edu, model);
            }
            #endregion

            //DataContext.TB_Stu_Education.Attach(model);
            //DataContext.Entry(model).State = EntityState.Modified;
            //DataContext.SaveChanges();
            var result=await   Dapper.DapperHelper.UpdateAsync(model);

        }

        private async Task UpdateSchoolId(EducationDTO edu, TB_Stu_Education model)
        {
            string schoolSql = string.Format("select ID from School where Name='{0}'", edu.SchoolName);
            int schoolId = await DataContext.Database.SqlQuery<int>(schoolSql).FirstOrDefaultAsync();
            if (schoolId != 0)
            {
                model.SchoolId = schoolId;
            }
        }

        private async Task UpdateMajorCode(EducationDTO edu, TB_Stu_Education model)
        {
            string majorSql = string.Format("select Code from MajorStandard where Major='{0}'", edu.Major);
            string majorCode = await DataContext.Database.SqlQuery<string>(majorSql).FirstOrDefaultAsync();
            if (string.IsNullOrEmpty(majorCode))
            {
                model.MajorCode = majorCode;
            }
        }

        public async Task DeleteAsync(int id)
        {
            string sql = string.Format("delete from TB_Stu_Education where Id='{0}'", id);
            await DataContext.Database.ExecuteSqlCommandAsync(sql);
        }

        public string GetStuMajorCodes(int userId) 
        {
            string sql = string.Format("select MajorCode from tb_STU_Education where userId = {0} and isnull(majorcode,'') <> '' and isnull(majorcode,'') <> '0' and majorcode in (select code from MajorStandard) order by  1 desc", userId);
            var majorCodes = DataContext.Database.SqlQuery<string>(sql).ToList();
            if (majorCodes.Count > 0) 
            {
                return string.Join(",", majorCodes);
            }
            return string.Empty;
        }
      
    }
    public interface IStuEducationRepo : IRepository<TB_Stu_Education>
    {

        TB_Stu_Education GetLastEdu(int userId);

        Task<EducationDTO> GetByIdAsync(int id);

        Task<List<EducationDTO>> FindListByUserAsync(int userId);

        Task CreateAsync(EducationDTO edu);

        Task UpdateAsync(EducationDTO edu);

        Task DeleteAsync(int id);

        Task<EducationDTO> FindLatestEduByUserAsync(int userId);
        /// <summary>
        /// 获取教育经历中所有学校
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        IList<string> FindEduSchoolNamesByUser(int userId);

        string GetStuMajorCodes(int userId);

    }
}
