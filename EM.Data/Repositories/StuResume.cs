using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topuc22Top.Data.Infrastructure;
using Topuc22Top.Data.ResultModel;
using Topuc22Top.Model.DTOs;
using Topuc22Top.Model.Entities;
using System.Data.Entity;
using  Topuc22Top.Model.VMs;
using Topuc22Top.Model.ExtendEntities;

namespace Topuc22Top.Data.Repositories
{
    public class StuResumeRepo : RepositoryBase<TB_S_Account>, IStuResumeRepo
    {
        public StuResumeRepo(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }



        public StudentProfileResult GetStudentResult(int userId)
        {
            StudentProfileResult StuResult = new StudentProfileResult();

            var context = ObjectContextHelper.TopUObjectContext;

            TB_S_Account acc = context.TB_S_Account.Where(a => a.UserId == userId).FirstOrDefault();
            if (acc != null)
            {
                StuResult.StuId = userId;
                StuResult.UserName = acc.UserName;
                StuResult.RegisterEmail = acc.RegisterEmail;

                StuResult.Account = context.TB_S_Account.Where(s => s.UserId == userId).FirstOrDefault();
                StuResult.BasicInfo = context.TB_S_Basic.Where(s => s.UserId == userId).FirstOrDefault();
                StuResult.Educations = context.TB_Stu_Education.Where(e => e.UserId == userId).OrderByDescending(e => e.StartDate).ToList();
                StuResult.Internships = context.TB_Stu_Internship.Where(s => s.UserId == userId).ToList();

                StuResult.ITSkills = context.TB_Stu_ITSkill.Where(s => s.UserId == userId).ToList();

                StuResult.Languages = context.TB_Stu_Language.Where(s => s.UserId == userId).ToList();
                StuResult.Projects = context.TB_Stu_Project.Where(s => s.UserId == userId).ToList();

                StuResult.Rewards = context.TB_Stu_Reward.Where(s => s.UserId == userId).ToList();

                StuResult.CampusJobs = context.TB_Stu_CampusJob.Where(s => s.UserId == userId).ToList();

                StuResult.Certificates = context.TB_Stu_Certificate.Where(s => s.UserId == userId).ToList();

                StuResult.Objective = context.TB_Stu_Objective.Where(e => e.UserId == userId).FirstOrDefault();

                StuResult.Evalution = context.TB_Stu_Evaluation.Where(e => e.UserId == userId).FirstOrDefault();

                //StuResult.Others = context.TB_Stu_Other.Where(e => e.UserId == userId).ToList();

                StuResult.Attachment = context.TB_Stu_Attachment.Where(e => e.UserId == userId).ToList();

                StuResult.NetworkLink = context.TB_Stu_NetworkLink.Where(v => v.UserId == userId).FirstOrDefault();

                return StuResult;
            }
            else
            {
                return null;
            }

        }

    }
    public interface IStuResumeRepo : IRepository<TB_S_Account>
    {
        /// <summary>
        /// 通过UserID获取简历转发的基本信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        StudentProfileResult GetStudentResult(int userId);


    }
}
