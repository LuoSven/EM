using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topuc22Top.Common;
using Topuc22Top.Data.Infrastructure;
using Topuc22Top.Data.ResultModel;
using Topuc22Top.Model.Entities;

namespace Topuc22Top.Data.Repositories
{
    public class JobPushRepo : RepositoryBase<Job_Push>, IJobPushRepo
    {
        public JobPushRepo(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }

        public PagedResult<StuPushResult> GetStuPushList(int enterpriseId, string posId, int order, int page, int pageSize, int? stuPushStatus = null)
        {
            var pushList = from p in DataContext.Job_Push
                          where p.EnterpriseId == enterpriseId
                          select p;
            if (stuPushStatus.HasValue)
            {
                if (stuPushStatus == (int)PushEtpMark.Invited)
                    pushList = pushList.Where(p => p.EtpMark == (int)PushEtpMark.Invited 
                        || p.EtpMark == (int)PushEtpMark.InvitedAnother);
                else
                    pushList = pushList.Where(p => p.EtpMark == stuPushStatus);
            }
            if (!string.IsNullOrEmpty(posId))
            {
                int pid = 0;
                Int32.TryParse(posId, out pid);
                if(pid!=0)
                {
                    pushList = pushList.Where(p => p.JobId == pid);
                }
               
            }
            var maxPushIds = (from p in pushList
                              group p by p.UserId into gp
                              select gp.Max(p => p.PushId));
            var query = from p in DataContext.Job_Push
                        join u in DataContext.TB_S_Account
                        on p.UserId equals u.UserId into ju
                        from su in ju.DefaultIfEmpty()
                        join j in DataContext.TB_Position_Element
                        on p.JobId equals j.PositionId into jj
                        from sj in jj.DefaultIfEmpty()
                        where p.EnterpriseId == enterpriseId
                        join b in DataContext.TB_S_Basic
                        on p.UserId equals b.UserId into jb
                        from sb in jb.DefaultIfEmpty()
                        join o in DataContext.TB_Stu_Objective
                        on p.UserId equals o.UserId into jo
                        from so in jo.DefaultIfEmpty()

                        join v in DataContext.WeChatUpload
                        on p.UserId equals v.UserId into gv
                        from vv in gv.DefaultIfEmpty()

                        where maxPushIds.Contains(p.PushId)
                        && sj.PositionStatus != (int)PositionStatus.Deleted
                        select new StuPushResult()
                        {
                            PushId = p.PushId,
                            PositionId = p.JobId,
                            EnterpriseId = p.EnterpriseId,
                            UserId = p.UserId,
                            MatchDegree = p.MatchDegree,
                            EtpMark = p.EtpMark,

                            UserName = su.UserName,
                            Email = su.RegisterEmail,
                            Gender = sb.Gender,
                            Mobile = sb.Mobile,
                            City = sb.CurrentLocation,

                            Postion = sj.Position,
                            JobCityId = sj.CityId,
                            
                            ObjectiveCity = so.ObjectLocation,
                            ObjectiveIndustry = so.ObjectIndustry,
                            ObjectivePosType = so.ObjectiveType,
                            ObjectiveSalary = so.ObjectiveSalary,
                            CreateDate = p.CreateDate,
                            VideoUrl= vv.Link
                        };
            
            int count = query.Count();
            var list = new List<StuPushResult>();
            if (order == 1)
            {
                list = query.OrderByDescending(p => p.MatchDegree).ThenByDescending(p => p.CreateDate).Skip((page - 1) * pageSize).Take(pageSize).ToList();
            }
            else if (order == 2)
            {
                list = query.OrderBy(p => p.MatchDegree).ThenByDescending(p => p.CreateDate).Skip((page - 1) * pageSize).Take(pageSize).ToList();
            }
            else
            {
                list = query.OrderByDescending(p => p.CreateDate).Skip((page - 1) * pageSize).Take(pageSize).ToList();
            }

            foreach (var it in list)
            {
                var lastEdu = DataContext.TB_Stu_Education.Where(a => a.UserId == it.UserId).OrderByDescending(a => a.StartDate).FirstOrDefault();
                if (lastEdu != null)
                {
                    it.Degree = lastEdu.Degree;
                    it.School = lastEdu.SchoolName;
                    it.Major = lastEdu.Major;
                }
            }
            return new PagedResult<StuPushResult>()
            {
                Results = list,
                PageSize = pageSize,
                CurrentPage = page,
                RowCount = count
            };
        }

        public Dictionary<int, string> GetPushJobs(int enterpriseId)
        {
            var pushList = from p in DataContext.Job_Push
                          where p.EnterpriseId == enterpriseId
                          select p;
            var query = from p in pushList
                        join j in DataContext.TB_Position_Element on p.JobId equals j.PositionId
                        join d in DataContext.DictItem.Where(d=>d.Type=="city") on j.CityId equals d.ItemId
                        select new
                        {
                            Key = j.PositionId,
                            Value = j.Position+","+d.ItemName
                        };
            return query.Distinct().ToDictionary(j => j.Key, j => j.Value);
                        
        }


        public int GetStuPushCnt(int enterpriseId,string posId)
        {
            var query = from p in DataContext.Job_Push
                        join j in DataContext.TB_Position_Element
                        on p.JobId equals j.PositionId into jj
                        from sj in jj.DefaultIfEmpty()
                        where p.EnterpriseId == enterpriseId
                        && sj.PositionStatus != 99
                        select new StuPushResult()
                        {
                            PushId = p.PushId,
                            PositionId = p.JobId
                        };
            if (!string.IsNullOrEmpty(posId))
            {
                int id = 0;
                Int32.TryParse(posId, out id);
                if (id != 0)
                {
                    query = query.Where(x => x.PositionId == id);
                }
            }
            return query.Count();

        }

        public PagedResult<JobSimpleResult> GetJobPushList(int userId, int page, int pageSize)
        {
            var query = from p in DataContext.Job_Push
                        join j in DataContext.TB_Position_Element on p.JobId equals j.PositionId into jj
                        from pp in jj.DefaultIfEmpty()
                        join e in DataContext.TB_Enterprise on p.EnterpriseId equals e.EnterpriseId into je
                        from ee in je.DefaultIfEmpty()
                        where p.UserId == userId 
                        select new JobSimpleResult()
                        {
                            PositionId = pp.PositionId,
                            Position = pp.Position,
                            CityId = pp.CityId,
                            SalaryMin = pp.SalaryMin,
                            SalaryMax = pp.SalaryMax,
                            EtpId = ee.EnterpriseId,
                            EtpName = ee.Name,
                            CreateDate = p.EtpMarkDate.Value,
                            InternSalaryType = pp.InternSalaryType,
                            PositionType = pp.PositionType
                        };
            int cnt = query.Count();
            return new PagedResult<JobSimpleResult>()
            {
                RowCount = cnt,
                Results = query.OrderByDescending(j => j.CreateDate).Skip((page-1)*pageSize).Take(pageSize).ToList(),
                CurrentPage = page,
                PageSize = pageSize
            };
        }


        public void PushJob(int jobId)
        {
            //DataContext.usp_PushJob(jobId);
            DataContext.usp_PushJobToSchool(jobId);
        }

        public void GetStuPushCount(int enterpriseId, string posId, out int allCount, out int invitedCount, out int ignoreCount) 
        {
            var pushList = from p in DataContext.Job_Push
                           where p.EnterpriseId == enterpriseId
                           select p;
            if (!string.IsNullOrEmpty(posId))
            {
                int pid = 0;
                Int32.TryParse(posId, out pid);
                if (pid != 0)
                {
                    pushList = pushList.Where(p => p.JobId == pid);
                }

            }
            var maxPushIds = (from p in pushList
                              group p by p.UserId into gp
                              select gp.Max(p => p.PushId));
            var query = from p in DataContext.Job_Push
                        join j in DataContext.TB_Position_Element
                        on p.JobId equals j.PositionId into jj
                        from sj in jj.DefaultIfEmpty()
                        where maxPushIds.Contains(p.PushId)
                        && sj.PositionStatus != 99
                        select new StuPushResult()
                        {
                            EtpMark = p.EtpMark
                        };

            allCount = query.Count();
            invitedCount = query.Where(p => p.EtpMark == (int)PushEtpMark.Invited
                || p.EtpMark == (int)PushEtpMark.InvitedAnother).Count();
            ignoreCount = query.Where(p => p.EtpMark == (int)PushEtpMark.Ignore).Count();
        }

    }
    public interface IJobPushRepo : IRepository<Job_Push>
    {
        int GetStuPushCnt(int enterpriseId,string posId);
        PagedResult<StuPushResult> GetStuPushList(int enterpriseId,string posId, int order, int page, int pageSize, int? stuPushStatus = null);

        /// <summary>
        /// 获得企业所有获得推荐的职位，返回职位ID和名称
        /// </summary>
        Dictionary<int,string> GetPushJobs(int enterpriseId);

        PagedResult<JobSimpleResult> GetJobPushList(int userId,  int page, int pageSize);
        void PushJob(int jobId);

        void GetStuPushCount(int enterpriseId, string posId, out int allCount, out int invitedCount, out int ignoreCount);
    }
}
