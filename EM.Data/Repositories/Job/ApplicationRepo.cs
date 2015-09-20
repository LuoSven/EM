using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Topuc22Top.Common;
using Topuc22Top.Data.Infrastructure;
using Topuc22Top.Data.ResultModel;
using Topuc22Top.Data.SearchModels;
using Topuc22Top.Data.ViewModel;
using Topuc22Top.Model.Entities;
using Topuc22Top.Common;

using Dapper;
using Topuc22Top.Data.Dapper;
using Topuc22Top.Model.DTOs;

namespace Topuc22Top.Data.Repositories
{
    public class ApplicationRepo : RepositoryBase<TB_S_Position>, IApplicationRepo
    {
        public ApplicationRepo(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }


        public IList<JobInApply> GetAppliedJobs(int userId)
        {
            var query = from p in DataContext.TB_S_Position
                        join job in DataContext.TB_Position_Element
                        on p.PositionId equals job.PositionId
                        join etp in DataContext.TB_Enterprise
                        on p.EnterpriseId equals etp.EnterpriseId
                        join c in (DataContext.DictItem.Where(a => a.Type == "City"))
                        on job.CityId equals c.ItemId
                        join i in DataContext.Stu_Interview on p.ApplyId equals i.ApplyId into interviewgroup
                        from ig in interviewgroup.DefaultIfEmpty()
                        where p.UserId == userId
                        orderby p.ApplyId descending
                        select new JobInApply()
                        {
                            ApplyID = p.ApplyId,
                            JobID = p.PositionId,
                            EtpID = p.EnterpriseId,
                            JobName = job.Position,
                            EtpName = etp.Name,
                            ApplyStatus = p.ApplyStatus,
                            ApplyTime = p.ApplyTime,
                            ModifyDate = p.ModifyDate,
                            City = c.ItemName,
                            InterView = ig

                        };

            return query.OrderByDescending(x => x.ApplyTime).ToList();
        }

        public IList<JobInApply> GetAppliedJobs(int userId, int pageNo, int pageSize, out int recCount)
        {
            var query = from p in DataContext.TB_S_Position
                        join job in DataContext.TB_Position_Element
                        on p.PositionId equals job.PositionId
                        join etp in DataContext.TB_Enterprise
                        on p.EnterpriseId equals etp.EnterpriseId
                        join c in (DataContext.DictItem.Where(a => a.Type == "City"))
                        on job.CityId equals c.ItemId
                        where p.UserId == userId
                        orderby p.ApplyId descending
                        select new JobInApply()
                        {
                            ApplyID = p.ApplyId,
                            JobID = p.PositionId,
                            EtpID = p.EnterpriseId,
                            JobName = job.Position,
                            EtpName = etp.Name,
                            ApplyStatus = p.ApplyStatus,
                            ApplyTime = p.ApplyTime,
                            ModifyDate = p.ModifyDate,
                            City = c.ItemName
                        };
            recCount = query.Count();
            return query.OrderByDescending(x => x.ApplyTime).Skip(pageSize * (pageNo - 1)).Take(pageSize).ToList();
        }

        public PagedResult<ApplyItem> GetApplicationList(int enterpriseId, ApplicationSearchModel sm, int page, int pageSize, out int filterCnt)
        {
            if (!string.IsNullOrEmpty(sm.IsPush))
                throw new Exception(); //异常，推荐简历 不该调用此方法

            var query = from m in DataContext.TB_S_Position
                        join basic in DataContext.TB_S_Basic
                        on m.UserId equals basic.UserId into gbasic
                        join account in DataContext.TB_S_Account
                        on m.UserId equals account.UserId into gaccount
                        join position in DataContext.TB_Position_Element
                        on m.PositionId equals position.PositionId into gposition
                        join education in DataContext.TB_Stu_Education
                        on m.UserId equals education.UserId into geducation
                        
                        join v in DataContext.TB_Stu_CloudVideo
                        on m.UserId equals v.UserId into gv

                        join i in DataContext.Job_Push.Where(p => p.EnterpriseId == enterpriseId && p.EtpMark == (int)PushEtpMark.Invited)
                        on new { uid = m.UserId, pid = m.PositionId } equals new { uid = i.UserId, pid = i.JobId } into ji
                        from ii in ji.DefaultIfEmpty()

                        join stuIv in DataContext.Stu_Interview
                        on m.ApplyId equals stuIv.ApplyId into gStuIv

                        join note in DataContext.Apply_EtpNote
                        on m.ApplyId equals note.ApplyId into gNote

                        where m.EnterpriseId == enterpriseId
                        && m.ApplyStatus != (int)ApplyStatus.Deleted //20150615新机制：DeleteDate表示进回收箱的时间，ApplyStatus.Deleted表示彻底删除
                        && m.UserId > 0
                        && geducation.OrderByDescending(x => x.StartDate).Select(x => x.Degree).FirstOrDefault()!=null 
                        select new ApplyItem()
                        {
                            UserId = m.UserId,
                            ApplyTime = m.ApplyTime,
                            ApplyStatus = m.ApplyStatus,
                            PositionId = m.PositionId,
                            FavoriteTime = m.FavoriteTime, //收藏的日期
                            DeleteDate = m.DeleteDate, //进回收箱日期

                            ApplyId = m.ApplyId,
                            MatchDegree = m.MatchDegree,

                            Gender = gbasic.Select(x => x.Gender == null ? (bool?)null : (bool?)x.Gender).FirstOrDefault(),
                            CurrentLocation = gbasic.Select(x => x.CurrentLocation).FirstOrDefault(),
                            Mobile = gbasic.Select(x => x.Mobile).FirstOrDefault(),
                            stuCity = gbasic.Select(x => x.CurrentLocation).FirstOrDefault() ?? 0,
                            Email = gaccount.Select(x => x.RegisterEmail).FirstOrDefault(),
                            UserName = gaccount.Select(x => x.UserName).FirstOrDefault(),

                            Position = gposition.Select(x => x.Position).FirstOrDefault(),
                            jobCity = gposition.Select(x => x.CityId).FirstOrDefault(),
                            DegreeId = geducation.OrderByDescending(x => x.StartDate).Select(x => x.Degree).FirstOrDefault(),
                            Majors = geducation.OrderByDescending(x => x.StartDate).Select(x => x.Major).ToList(),
                            SchoolNames = geducation.OrderByDescending(x => x.StartDate).Select(x => x.SchoolName).ToList(),
                            IsFilterOut = m.IsFilterOut,
                            FilterOutResons = m.FilterOutReasons,
                            VideoUrl = gv.Where(o=>o.Status ==(int)VideoUploadStatus.CanPlay).OrderByDescending(p => p.CreateDate).Select(p => p.CloudFileId).FirstOrDefault(),
                            OrderStatus = m.ApplyStatus <= (int)ApplyStatus.Read ? 0 : 1,
                            IsInvited = (ii.PushId != null),
                            ModifyDate = m.ModifyDate,
                            EtpNoteCount = m.EtpNoteCount ?? 0,
                            NoteList = gNote.OrderByDescending(p => p.CreateDate).Take(3).ToList()
                        };

            //根据职位筛选
            if (!string.IsNullOrEmpty(sm.posId))
            {
                int posid = int.Parse(sm.posId);
                query = query.Where(p => p.PositionId == posid);
            }

            //简历状态筛选
            if (sm.IsRecycleBin == "1")
            {
                query = query.Where(p => p.ApplyStatus == (int)ApplyStatus.Refused || p.ApplyStatus == (int)ApplyStatus.BeenInterested || p.DeleteDate.HasValue);//进回收箱了
            }
            else
            {
                query = query.Where(p => p.ApplyStatus != (int)ApplyStatus.Refused && p.ApplyStatus != (int)ApplyStatus.BeenInterested && !p.DeleteDate.HasValue); //未进回收箱
                if (sm.currentstatus.HasValue)
                {
                    if (sm.currentstatus.Value == (int)ApplyStatus.Apply)
                    {
                        //未处理
                        query = query.Where(p => p.ApplyStatus == (int)ApplyStatus.Apply || p.ApplyStatus == (int)ApplyStatus.Read);
                    }
                    else if (sm.currentstatus.Value == (int)ApplyStatus.Interested)
                    {
                        //感兴趣
                        query = query.Where(p => p.ApplyStatus == sm.currentstatus.Value);
                    }
                    else if (sm.currentstatus.Value == (int)ApplyStatus.Approved)
                    {
                        //通知面试
                        query = query.Where(p => p.ApplyStatus == sm.currentstatus.Value);
                    }
                    else if (sm.currentstatus.Value != 0) 
                    {
                        //throw new Exception("参数有问题 currentstatus 非预期"); //参数有问题
                    }
                }
                else
                {
                    //全部简历
                }
            }

            if (sm.IsRecycleBin == "1" ||
        (sm.currentstatus.HasValue && (sm.currentstatus.Value == (int)ApplyStatus.Interested || sm.currentstatus.Value == (int)ApplyStatus.Approved)) //感兴趣、通知面试
        )
            {
                //不用考虑过滤情况
                filterCnt = 0;
            }
            else
            {
                filterCnt = query.Where(p => p.IsFilterOut).Count();
                if (sm.IsFilterOut == "1")
                {
                    query = query.Where(p => p.IsFilterOut);
                }
                else
                {
                    query = query.Where(p => !p.IsFilterOut);
                }
            }

            #region
            if (!string.IsNullOrEmpty(sm.degree))
            {
                int degreeId = Int32.Parse(sm.degree);
                query = query.Where(p => p.DegreeId.HasValue && p.DegreeId.Value == degreeId);
            }
            if (!string.IsNullOrEmpty(sm.city))
            {
                int cityId = int.Parse(sm.city);
                query = query.Where(p => p.CurrentLocation == cityId);
            }
            if (!string.IsNullOrEmpty(sm.school))
            {
                query = query.Where(p => p.SchoolNames.Contains(sm.school));
            }
            if (!string.IsNullOrEmpty(sm.major))
            {
                query = query.Where(p => p.Majors.Contains(sm.major));
            }
            if (!string.IsNullOrEmpty(sm.dtime))
            {
                int datespan = GetDateSpan(sm.dtime);
                DateTime dt = DateTime.Now.AddDays(-1 * datespan);
                query = query.Where(f => f.ApplyTime > dt);
            }
            if (!string.IsNullOrEmpty(sm.keyword))
            {
                query = query.Where(p => p.UserName.Contains(sm.keyword) || p.Position.Contains(sm.keyword) || p.SchoolNames.Contains(sm.keyword));
            }
            if (sm.IsFavorites == "1")
            {
                //星标简历
                query = query.Where(p => p.FavoriteTime.HasValue);
            }
            #endregion

            PagedResult<ApplyItem> result = new PagedResult<ApplyItem>()
            {
                CurrentPage = page,
                PageSize = pageSize,
                RowCount = query.Count(),
                Results =
                (sm.IsRecycleBin == "1" || (sm.currentstatus.HasValue && sm.currentstatus >= (int)ApplyStatus.Interested))
                ? query.OrderByDescending(p => p.ModifyDate).Skip(pageSize * (page - 1)).Take(pageSize).ToList()    //? 按修改时间排序
                : (
                    (sm.currentstatus.HasValue && sm.currentstatus == (int)ApplyStatus.Apply)
                    ?
                    query.OrderByDescending(p => p.ApplyTime).Skip(pageSize * (page - 1)).Take(pageSize).ToList() //处理的简历，默认按投递时间排序（新投递的在前
                    :
                    query.OrderBy(x => 
                    ((x.ApplyStatus == (int)ApplyStatus.Refused || x.ApplyStatus == (int)ApplyStatus.BeenInterested || x.DeleteDate.HasValue) ? 100 : x.OrderStatus)
                    ).ThenByDescending(x => x.ApplyTime).Skip(pageSize * (page - 1)).Take(pageSize).ToList()  //状态+时间
                 )
            };
            return result;
        }

        public ApplyItem GetApplicationById(int ApplyId, int enterpriseId)
        {
                 var query = from m in DataContext.TB_S_Position
                        join basic in DataContext.TB_S_Basic
                        on m.UserId equals basic.UserId into gbasic
                        join account in DataContext.TB_S_Account
                        on m.UserId equals account.UserId into gaccount
                        join position in DataContext.TB_Position_Element
                        on m.PositionId equals position.PositionId into gposition
                        join education in DataContext.TB_Stu_Education
                        on m.UserId equals education.UserId into geducation
                        join v in DataContext.WeChatUpload
                        on m.UserId equals v.UserId into gv
                        from vv in gv.DefaultIfEmpty()
                        join i in DataContext.Job_Push.Where(p => p.EnterpriseId == enterpriseId && p.EtpMark == (int)PushEtpMark.Invited)
                        on new { uid = m.UserId, pid = m.PositionId } equals new { uid = i.UserId, pid = i.JobId } into ji
                        from ii in ji.DefaultIfEmpty()
                        where m.EnterpriseId == enterpriseId
                        && m.ApplyStatus != (int)ApplyStatus.Deleted //20150615新机制：DeleteDate表示进回收箱的时间，ApplyStatus.Deleted表示彻底删除
                        && m.UserId > 0 && m.ApplyId == ApplyId
                             select new ApplyItem()
                        {
                            UserId = m.UserId,
                            ApplyTime = m.ApplyTime,
                            ApplyStatus = m.ApplyStatus,
                            PositionId = m.PositionId,

                            ApplyId = m.ApplyId,
                            MatchDegree = m.MatchDegree,

                            Gender = gbasic.Select(x => x.Gender == null ? (bool?)null : (bool?)x.Gender).FirstOrDefault(),
                            CurrentLocation = gbasic.Select(x => x.CurrentLocation).FirstOrDefault(),
                            Mobile = gbasic.Select(x => x.Mobile).FirstOrDefault(),

                            Email = gaccount.Select(x => x.RegisterEmail).FirstOrDefault(),
                            UserName = gaccount.Select(x => x.UserName).FirstOrDefault(),

                            Position = gposition.Select(x => x.Position).FirstOrDefault(),
                            jobCity = gposition.Select(x => x.CityId).FirstOrDefault(),

                            DegreeId = geducation.OrderByDescending(x => x.StartDate).Select(x => x.Degree).FirstOrDefault(),
                            Majors = geducation.OrderByDescending(x => x.StartDate).Select(x => x.Major).ToList(),
                            SchoolNames = geducation.OrderByDescending(x => x.StartDate).Select(x => x.SchoolName).ToList(),
                            IsFilterOut = m.IsFilterOut,
                            FilterOutResons = m.FilterOutReasons,
                            VideoUrl = vv.Link,
                            OrderStatus = m.ApplyStatus <= (int)ApplyStatus.Read ? 0 : 1,
                            IsInvited = (ii.PushId != null),
                            FavoriteTime = m.FavoriteTime
                        };

                 return query.FirstOrDefault();
        }

        /// <summary>
        /// 被过滤的应聘简历列表（全部删除 时 调用）
        /// </summary>
        /// <param name="enterpriseId"></param>
        /// <param name="sm"></param>
        /// <returns></returns>
        public IList<TB_S_Position> GetFilterOutList(int enterpriseId, ApplicationSearchModel sm)
        {
            var query = from m in DataContext.TB_S_Position
                        join basic in DataContext.TB_S_Basic
                        on m.UserId equals basic.UserId into gbasic
                        join account in DataContext.TB_S_Account
                        on m.UserId equals account.UserId into gaccount
                        join position in DataContext.TB_Position_Element
                        on m.PositionId equals position.PositionId into gposition
                        join education in DataContext.TB_Stu_Education
                        on m.UserId equals education.UserId into geducation

                        where m.EnterpriseId == enterpriseId
                        && m.ApplyStatus != (int)ApplyStatus.Deleted
                        && m.UserId > 0
                        && geducation.OrderByDescending(x => x.StartDate).Select(x => x.Degree).FirstOrDefault() != null
                        select new ApplyItem()
                        {
                            ApplyStatus = m.ApplyStatus,
                            FavoriteTime = m.FavoriteTime,
                            DeleteDate = m.DeleteDate, //进回收箱日期
                            ApplyId = m.ApplyId,
                            CurrentLocation = gbasic.Select(x => x.CurrentLocation).FirstOrDefault(),

                            UserName = gaccount.Select(x => x.UserName).FirstOrDefault(),

                            Position = gposition.Select(x => x.Position).FirstOrDefault(),
                            DegreeId = geducation.OrderByDescending(x => x.StartDate).Select(x => x.Degree).FirstOrDefault(),
                            Majors = geducation.OrderByDescending(x => x.StartDate).Select(x => x.Major).ToList(),
                            SchoolNames = geducation.OrderByDescending(x => x.StartDate).Select(x => x.SchoolName).ToList(),
                            IsFilterOut = m.IsFilterOut,
                        };

            //根据职位筛选
            if (!string.IsNullOrEmpty(sm.posId))
            {
                int posid = int.Parse(sm.posId);
                query = query.Where(p => p.PositionId == posid);
            }

            query = query.Where(p => p.ApplyStatus != (int)ApplyStatus.Refused && p.ApplyStatus != (int)ApplyStatus.BeenInterested && !p.DeleteDate.HasValue); //未进回收箱

            query = query.Where(p => p.IsFilterOut);

            #region
            if (!string.IsNullOrEmpty(sm.degree))
            {
                int degreeId = Int32.Parse(sm.degree);
                query = query.Where(p => p.DegreeId.HasValue && p.DegreeId.Value == degreeId);
            }
            if (!string.IsNullOrEmpty(sm.city))
            {
                int cityId = int.Parse(sm.city);
                query = query.Where(p => p.CurrentLocation == cityId);
            }
            if (!string.IsNullOrEmpty(sm.school))
            {
                query = query.Where(p => p.SchoolNames.Contains(sm.school));
            }
            if (!string.IsNullOrEmpty(sm.major))
            {
                query = query.Where(p => p.Majors.Contains(sm.major));
            }
            if (!string.IsNullOrEmpty(sm.dtime))
            {
                int datespan = GetDateSpan(sm.dtime);
                DateTime dt = DateTime.Now.AddDays(-1 * datespan);
                query = query.Where(f => f.ApplyTime > dt);
            }
            if (!string.IsNullOrEmpty(sm.keyword))
            {
                query = query.Where(p => p.UserName.Contains(sm.keyword) || p.Position.Contains(sm.keyword) || p.SchoolNames.Contains(sm.keyword));
            }
            if (sm.IsFavorites == "1")
            {
                //星标简历
                query = query.Where(p => p.FavoriteTime.HasValue);
            }
            #endregion

            var idList = query.Select(x => x.ApplyId);
            
            return DataContext.TB_S_Position.Where(x => idList.Contains(x.ApplyId)).ToList();
        }

        public PagedResult<ApplyItem> GetApplyList(int userId, int? applyStatus, int page, int pageSize)
        {
            var query = from a in DataContext.TB_S_Position
                        join p in DataContext.TB_Position_Element on a.PositionId equals p.PositionId
                        join e in DataContext.TB_Enterprise on a.EnterpriseId equals e.EnterpriseId
                        where a.UserId == userId
                        select new ApplyItem()
                        {
                            UserId = a.UserId,
                            ApplyTime = a.ApplyTime,
                            ApplyStatus = a.ApplyStatus,
                            PositionId = a.PositionId,
                            ApplyId = a.ApplyId,
                            MatchDegree = a.MatchDegree,
                            Position = p.Position,
                            ModifyDate = a.ModifyDate,
                            EtpId = e.EnterpriseId,
                            EtpName = e.Name,
                            Function = p.FunctionIds,
                            jobCity = p.CityId,
                            IsFilterOut = a.IsFilterOut
                        };
            if (applyStatus.HasValue)
            {
                query = query.Where(a => a.ApplyStatus == applyStatus.Value);
            }
            int count = query.Count();

            return new PagedResult<ApplyItem>()
            {
                CurrentPage = page,
                PageSize = pageSize,
                RowCount = count,
                Results = query.OrderByDescending(a => a.ApplyTime).Skip(pageSize * (page - 1)).Take(pageSize).ToList()
            };

        }
        public void GetApplicationCount(int enterpriseId, string posId, out int allCount)
        {
            var query = from m in DataContext.TB_S_Position

                        join education in DataContext.TB_Stu_Education
                        on m.UserId equals education.UserId into geducation

                        where m.EnterpriseId == enterpriseId
                        && m.ApplyStatus != (int)ApplyStatus.Deleted && !m.DeleteDate.HasValue
                        && m.UserId > 0
                        && geducation.OrderByDescending(x => x.StartDate).Select(x => x.Degree).FirstOrDefault() != null
                        select m;
            if (!string.IsNullOrEmpty(posId))
            {
                int id = 0;
                Int32.TryParse(posId, out id);
                if (id != 0)
                {
                    query = query.Where(x => x.PositionId == id);
                }
            }
            //全部简历的数量
            allCount = query.Count();
        }

        public void GetApplicationCount(int enterpriseId, out int interestedCount, out int approvedCount, out int needHandleCnt)
        {
            var query = from m in DataContext.TB_S_Position

                        join education in DataContext.TB_Stu_Education
                        on m.UserId equals education.UserId into geducation

                        where m.EnterpriseId == enterpriseId
                        && m.ApplyStatus != (int)ApplyStatus.Deleted && !m.DeleteDate.HasValue
                        && m.UserId > 0
                        && geducation.OrderByDescending(x => x.StartDate).Select(x => x.Degree).FirstOrDefault() != null
                        select m;
            //未处理
            needHandleCnt = query.Where(p => p.ApplyStatus == (int)ApplyStatus.Apply || p.ApplyStatus == (int)ApplyStatus.Read).Count();
            //感兴趣
            interestedCount = query.Where(p => p.ApplyStatus == (int)ApplyStatus.Interested).Count();
            //面试
            approvedCount = query.Where(p => p.ApplyStatus == (int)ApplyStatus.Approved).Count();
        }

        #region 应聘简历 高级筛选 所在城市、毕业院校、专业类别

        public Dictionary<int, string> GetApplyCurrentLocations(int enterpriseId, ApplicationSearchModel sm)
        {
            var dict = new Dictionary<int, string>();
            var query = from m in DataContext.TB_S_Position
                        join n in DataContext.TB_S_Basic
                        on m.UserId equals n.UserId
                        where m.EnterpriseId == enterpriseId
                        select new
                        {
                            CurrentLocation = n.CurrentLocation ?? 0,
                            PosId = m.PositionId,
                            ApplyStatus = m.ApplyStatus,
                            IsFilterOut = m.IsFilterOut,
                            DeleteDate = m.DeleteDate
                        };

            if (sm.IsRecycleBin == "1")
            {
                query = query.Where(p => p.ApplyStatus == (int)ApplyStatus.Refused || p.ApplyStatus == (int)ApplyStatus.BeenInterested || p.DeleteDate.HasValue);//进回收箱了
            }
            else
            {
                query = query.Where(p => p.ApplyStatus != (int)ApplyStatus.Refused && p.ApplyStatus != (int)ApplyStatus.BeenInterested && !p.DeleteDate.HasValue); //未进回收箱
                if (sm.currentstatus.HasValue)
                {
                    if (sm.currentstatus.Value == (int)ApplyStatus.Apply)
                    {
                        //未处理
                        query = query.Where(p => p.ApplyStatus == (int)ApplyStatus.Apply || p.ApplyStatus == (int)ApplyStatus.Read);
                    }
                    else if (sm.currentstatus.Value == (int)ApplyStatus.Interested)
                    {
                        //感兴趣
                        query = query.Where(p => p.ApplyStatus == sm.currentstatus.Value);
                    }
                    else if (sm.currentstatus.Value == (int)ApplyStatus.Approved)
                    {
                        //通知面试
                        query = query.Where(p => p.ApplyStatus == sm.currentstatus.Value);
                    }
                    else if (sm.currentstatus.Value != 0)
                    {
                        //throw new Exception("参数有问题 currentstatus 非预期"); //参数有问题
                    }
                }
                else
                {
                    //全部简历
                }
            }


            if (sm.IsRecycleBin == "1" ||
                    (sm.currentstatus.HasValue && (sm.currentstatus.Value == (int)ApplyStatus.Interested || sm.currentstatus.Value == (int)ApplyStatus.Approved)) //感兴趣、通知面试
                    )
            {
                //不用考虑过滤情况
            }
            else
            {
                if (sm.IsFilterOut == "1")
                {
                    query = query.Where(p => p.IsFilterOut);
                }
                else
                {
                    query = query.Where(p => !p.IsFilterOut);
                }
            }

            if (!string.IsNullOrEmpty(sm.posId))
            {
                int pid = 0;
                Int32.TryParse(sm.posId, out pid);
                if (pid != 0)
                {
                    query = query.Where(a => a.PosId == pid);
                }
            }

            var list = (from c in query
                        join d in DataContext.DictItem
                        on c.CurrentLocation equals d.ItemId
                        where d.Type == "city"
                        select new
                        {
                            CityId = c.CurrentLocation,
                            CityName = d.ItemName,
                        }).ToList();
            foreach (var m in list)
            {
                if (m.CityId != 0 && !dict.Keys.Contains(m.CityId))
                {
                    dict.Add(m.CityId, m.CityName);
                }
            }
            return dict;
        }

        public Dictionary<string, string> GetApplySchools(int enterpriseId, ApplicationSearchModel sm)
        {
            IList<string> schools = new List<string>();
            var query = from m in DataContext.TB_S_Position
                        join n in DataContext.TB_Stu_Education
                        on m.UserId equals n.UserId into g
                        where m.EnterpriseId == enterpriseId
                        && m.DeleteDate == null & !m.DeleteDate.HasValue
                        select new
                        {
                            SchoolNames = g.OrderByDescending(x => x.StartDate).Select(x => x.SchoolName).ToList(),
                            UserId = m.UserId,
                            PosId = m.PositionId,
                            ApplyStatus = m.ApplyStatus,
                            IsFilterOut = m.IsFilterOut,
                            DeleteDate = m.DeleteDate
                        };

            if (sm.IsRecycleBin == "1")
            {
                query = query.Where(p => p.ApplyStatus == (int)ApplyStatus.Refused || p.ApplyStatus == (int)ApplyStatus.BeenInterested || p.DeleteDate.HasValue);//进回收箱了
            }
            else
            {
                query = query.Where(p => p.ApplyStatus != (int)ApplyStatus.Refused && p.ApplyStatus != (int)ApplyStatus.BeenInterested && !p.DeleteDate.HasValue); //未进回收箱
                if (sm.currentstatus.HasValue)
                {
                    if (sm.currentstatus.Value == (int)ApplyStatus.Apply)
                    {
                        //未处理
                        query = query.Where(p => p.ApplyStatus == (int)ApplyStatus.Apply || p.ApplyStatus == (int)ApplyStatus.Read);
                    }
                    else if (sm.currentstatus.Value == (int)ApplyStatus.Interested)
                    {
                        //感兴趣
                        query = query.Where(p => p.ApplyStatus == sm.currentstatus.Value);
                    }
                    else if (sm.currentstatus.Value == (int)ApplyStatus.Approved)
                    {
                        //通知面试
                        query = query.Where(p => p.ApplyStatus == sm.currentstatus.Value);
                    }
                    else if (sm.currentstatus.Value != 0)
                    {
                        //throw new Exception("参数有问题 currentstatus 非预期"); //参数有问题
                    }
                }
                else
                {
                    //全部简历
                }
            }

            if (sm.IsRecycleBin == "1" ||
        (sm.currentstatus.HasValue && (sm.currentstatus.Value == (int)ApplyStatus.Interested || sm.currentstatus.Value == (int)ApplyStatus.Approved)) //感兴趣、通知面试
        )
            {
                //不用考虑过滤情况
            }
            else
            {
                if (sm.IsFilterOut == "1")
                {
                    query = query.Where(p => p.IsFilterOut);
                }
                else
                {
                    query = query.Where(p => !p.IsFilterOut);
                }
            }

            if (!string.IsNullOrEmpty(sm.posId))
            {
                int id = 0;
                Int32.TryParse(sm.posId, out id);
                if (id != 0)
                {
                    query = query.Where(a => a.PosId == id);
                }
            }

            var list = query.ToList();
            foreach (var m in list)
            {
                foreach (var name in m.SchoolNames)
                {
                    if (!string.IsNullOrEmpty(name) && !schools.Contains(name))
                    {
                        schools.Add(name);
                    }
                    break;
                }
            }
            return schools.ToDictionary(s => s, s => s);
        }

        public Dictionary<string, string> GetApplyMajors(int enterpriseId, ApplicationSearchModel sm)
        {
            IList<string> majors = new List<string>();

            var query = from m in DataContext.TB_S_Position
                        join n in DataContext.TB_Stu_Education
                        on m.UserId equals n.UserId into g
                        where m.EnterpriseId == enterpriseId
                        && m.DeleteDate == null & !m.DeleteDate.HasValue
                        select new
                        {
                            MajorNames = g.OrderByDescending(x => x.StartDate).Select(x => x.Major).ToList(),
                            UserId = m.UserId,
                            PosId = m.PositionId,
                            ApplyStatus = m.ApplyStatus,
                            IsFilterOut = m.IsFilterOut,
                            DeleteDate = m.DeleteDate
                        };

            if (sm.IsRecycleBin == "1")
            {
                query = query.Where(p => p.ApplyStatus == (int)ApplyStatus.Refused || p.ApplyStatus == (int)ApplyStatus.BeenInterested || p.DeleteDate.HasValue);//进回收箱了
            }
            else
            {
                query = query.Where(p => p.ApplyStatus != (int)ApplyStatus.Refused && p.ApplyStatus != (int)ApplyStatus.BeenInterested && !p.DeleteDate.HasValue); //未进回收箱
                if (sm.currentstatus.HasValue)
                {
                    if (sm.currentstatus.Value == (int)ApplyStatus.Apply)
                    {
                        //未处理
                        query = query.Where(p => p.ApplyStatus == (int)ApplyStatus.Apply || p.ApplyStatus == (int)ApplyStatus.Read);
                    }
                    else if (sm.currentstatus.Value == (int)ApplyStatus.Interested)
                    {
                        //感兴趣
                        query = query.Where(p => p.ApplyStatus == sm.currentstatus.Value);
                    }
                    else if (sm.currentstatus.Value == (int)ApplyStatus.Approved)
                    {
                        //通知面试
                        query = query.Where(p => p.ApplyStatus == sm.currentstatus.Value);
                    }
                    else if (sm.currentstatus.Value != 0)
                    {
                        //throw new Exception("参数有问题 currentstatus 非预期"); //参数有问题
                    }
                }
                else
                {
                    //全部简历
                }
            }

            if (sm.IsRecycleBin == "1" ||
        (sm.currentstatus.HasValue && (sm.currentstatus.Value == (int)ApplyStatus.Interested || sm.currentstatus.Value == (int)ApplyStatus.Approved)) //感兴趣、通知面试
        )
            {
                //不用考虑过滤情况
            }
            else
            {
                if (sm.IsFilterOut == "1")
                {
                    query = query.Where(p => p.IsFilterOut);
                }
                else
                {
                    query = query.Where(p => !p.IsFilterOut);
                }
            }

            if (!string.IsNullOrEmpty(sm.posId))
            {
                int pid = 0;
                Int32.TryParse(sm.posId, out pid);
                if (pid != 0)
                {
                    query = query.Where(a => a.PosId == pid);
                }
            }

            var list = query.ToList();
            foreach (var m in list)
            {
                foreach (var name in m.MajorNames)
                {
                    if (!string.IsNullOrEmpty(name) && !majors.Contains(name))
                    {
                        majors.Add(name);
                    }
                    break;
                }
            }

            return majors.ToDictionary(m => m, m => m);
        }
        
        #endregion

        public void CalcMatchDegree(int positionId)
        {
            var applyIds = GetMany(a => a.PositionId == positionId).Select(a => a.ApplyId).ToList();
            foreach (int applyId in applyIds)
            {
                DataContext.usp_CalcApplyMatchDegree(applyId);
            }
        }


        private int GetDateSpan(string deploytime)
        {
            switch (deploytime)
            {
                case "1": return 1;
                case "2": return 2;
                case "3": return 3;
                case "4": return 7;
                case "5": return 14;
                case "6": return 42;
                case "7": return 30;
                case "8": return 60;
            }
            return 1;
        }

        public bool IsStuApplied(int stuId)
        {
            return (from s in DataContext.TB_S_Position
                    where s.UserId == stuId
                    select 1).Any();
        }

        public bool IsStuAppliedWithFeedBack(int stuId)
        {
            return (from s in DataContext.TB_S_Position
                    where s.UserId == stuId
                    && s.ApplyStatus == (int)ApplyStatus.Approved
                    select 1).Any();
        }

        public InterviewViewInfoModel GetInterviewViewInfoModel(int applyId, int enterpriseId)
        {
            var query = from apply in DataContext.TB_S_Position

                        join pos in DataContext.TB_Position_Element
                        on apply.PositionId equals pos.PositionId into gPos
                        from iPos in gPos.DefaultIfEmpty()

                        join etp in DataContext.TB_Enterprise
                        on apply.EnterpriseId equals etp.EnterpriseId into gEtp
                        from iEtp in gEtp.DefaultIfEmpty()

                        join user in DataContext.TB_S_Account
                        on apply.UserId equals user.UserId into gUser
                        from iUser in gUser.DefaultIfEmpty()

                        join contact in DataContext.TB_Enterprise_Contact
                        on apply.EnterpriseId equals contact.EnterpriseId into gContact

                        where apply.ApplyId == applyId && apply.EnterpriseId == enterpriseId
                        select new InterviewViewInfoModel
                        {
                            Id = 0,
                            EnterpriseId = apply.EnterpriseId,
                            EnterpriseName = iEtp.Name,
                            ApplyId = apply.ApplyId,
                            UserId = apply.UserId,
                            UserName = iUser.UserName,
                            UserEmail = iUser.RegisterEmail,
                            InterviewInvitation = iPos.Position + "面试通知" + "-" + iEtp.Name,
                            InterviewTime = DateTime.MinValue, //在此逻辑外赋值
                            InterviewHour = 10,
                            InterivewMinute = 30,
                            InterviewPlace = gContact.Select(p => p.Address).FirstOrDefault(),
                            ContactMan = gContact.Select(p => p.ContactMan).FirstOrDefault(),
                            ContactTelephone = gContact.Select(p => ((string.IsNullOrEmpty(p.ContactAreaCode) ? "" : p.ContactAreaCode + "-") +
                                                    p.ContactTelephone + (string.IsNullOrEmpty(p.ContactExt) ? "" : "-" + p.ContactExt))
                                                ).FirstOrDefault(),
                            PositionName = iPos.Position,
                            PositionID = apply.PositionId,
                            ApplyStatus = apply.ApplyStatus

                        };
            return query.FirstOrDefault();
        }



        public async Task CreateAsync(int jobId, int userId)
        {
            int etpId = DataContext.TB_Position_Element.Where(p => p.PositionId == jobId).Select(p => p.EnterpriseId).FirstOrDefault();
            TB_S_Position apply = new TB_S_Position()
            {
                ApplyStatus = (int)ApplyStatus.Apply,
                PositionId = jobId,
                EnterpriseId = etpId,
                UserId = userId,
                LetterId = 0,
                IsWithVideoResume = false,
                ApplyTime = DateTime.Now,
                CreateDate = DateTime.Now,
                ModifyDate = DateTime.Now
            };
            DataContext.TB_S_Position.Add(apply);
            await DataContext.SaveChangesAsync();
        }

        #region dapper

        public async Task<IList<AppliedJobDTO>> GetAppliedJobsAsync(int userId, int status)
        {
            using (var conn = DapperHelper.Get22Connection())
            {
                string sql = string.Format(@"
select a.ApplyId, a.PositionId PositionId, b.Position PositionName, a.EnterpriseId EtpId, c.Name EtpName
, d.ItemName CityName
,a.ApplyStatus ApplyStatus
, case a.ApplyTime when null then '' else CONVERT(varchar(100),a.ApplyTime,23) end ApplyTime
, case a.ModifyDate when null then '' else CONVERT(varchar(100),a.ModifyDate,23) end ModifyDate
,e.InterviewPlace
, case e.InterviewTime when null then '' else CONVERT(varchar(100),e.InterviewTime,20) end InterviewTime
,e.ContactMan,e.ContactTelephone
,f.LogoPic
,a.DeleteDate
from TB_S_Position a
left join TB_Position_Element b
on a.PositionId = b.PositionId
left join TB_Enterprise c
on a.EnterpriseId = c.EnterpriseId
left join (select * from DictItem where [Type] = 'City') as d
on b.CityId = d.ItemId
left join Stu_Interview e
on a.ApplyId = e.ApplyId
left join TB_Enterprise_Display f
on a.EnterpriseId = f.EnterpriseId

where a.UserId = {0}
and(
{1} = 0
--已通知面试
or({1} = 31 and a.ApplyStatus = 31 and isnull(e.Id,0) > 0)
--不合适
or({1} = 61 and (a.ApplyStatus >= 61 or a.DeleteDate is not null))
or({1} <> 31 and a.ApplyStatus = {1})
)
order by a.CreateDate desc
", userId, status);
                var list = await conn.QueryAsync<AppliedJobDTO>(sql);

                list.ToList().ForEach(p => { 
                    //p.ApplyStatusName = ((ApplyStatus)p.ApplyStatus).GetEnumDescription(); 
                    if (p.DeleteDate.HasValue) 
                    {
                        p.ApplyStatus = (int)ApplyStatus.Refused;
                        p.ApplyStatusName = "不合适";
                        p.ModifyDate = p.DeleteDate.Value.ToString("yyyy-MM-dd");
                    }
                    else
                    {
                        if (p.ApplyStatus == (int)ApplyStatus.Apply)
                        {
                            p.ModifyDate = "";
                        }
                        switch (p.ApplyStatus)
                        {
                            case (int)ApplyStatus.Apply:
                                p.ApplyStatusName = "已成功申请，等待HR查阅";
                                break;
                            case (int)ApplyStatus.Read:
                                p.ApplyStatusName = "HR已查看简历";
                                break;
                            case (int)ApplyStatus.Interested:
                                p.ApplyStatusName = "通过初步筛选";
                                break;
                            case (int)ApplyStatus.Approved: 
                                {
                                    if (string.IsNullOrEmpty(p.InterviewTime))
                                    {
                                        p.ApplyStatus = 30; //仅用于配合前段
                                        p.ApplyStatusName = "待安排面试";
                                    }
                                    else
                                        p.ApplyStatusName = "收到面试通知";
                                }
                                break;
                            default:
                                p.ApplyStatus = (int)ApplyStatus.Refused;
                                p.ApplyStatusName = "不合适";
                                break;
                        }
                    }
                });

                if(status != 0)
                {
                    return list.Where(p => p.ApplyStatus == status).ToList();
                }

                return list.ToList();

            }
        }

        public async Task<AppliedJobDTO> GetAppliedJobDetailAsync(int applyId)
        {
            if (applyId <= 0) return null;
            using (var conn = DapperHelper.Get22Connection())
            {
                string sql = string.Format(@"
select a.ApplyId, a.PositionId PositionId, b.Position PositionName, a.EnterpriseId EtpId, c.Name EtpName
, d.ItemName CityName
,a.ApplyStatus ApplyStatus
, case a.ApplyTime when null then '' else CONVERT(varchar(100),a.ApplyTime,23) end ApplyTime
, case a.ModifyDate when null then '' else CONVERT(varchar(100),a.ModifyDate,23) end ModifyDate
,e.InterviewPlace
, case e.InterviewTime when null then '' else CONVERT(varchar(100),e.InterviewTime,20) end InterviewTime
,e.ContactMan,e.ContactTelephone
,f.LogoPic
,a.DeleteDate
,b.CityId
,b.PositionType
,dbo.fun_getIndustryName(c.Industry) IndustryName
,dbo.fun_gettopfunctions(b.FunctionIds) PositionFunctionIds
,dbo.fun_getPosFunctionName(b.FunctionIds) PositionFunctionName
from TB_S_Position a
left join TB_Position_Element b
on a.PositionId = b.PositionId
left join TB_Enterprise c
on a.EnterpriseId = c.EnterpriseId
left join (select * from DictItem where [Type] = 'City') as d
on b.CityId = d.ItemId
left join Stu_Interview e
on a.ApplyId = e.ApplyId
left join TB_Enterprise_Display f
on a.EnterpriseId = f.EnterpriseId

where a.ApplyId = {0}
order by a.CreateDate desc
", applyId);
                var p = (await conn.QueryAsync<AppliedJobDTO>(sql)).FirstOrDefault();

                if (p != null) 
                {
                    if (p.DeleteDate.HasValue)
                    {
                        p.ApplyStatus = (int)ApplyStatus.Refused;
                        p.ApplyStatusName = "不合适";
                        p.ModifyDate = p.DeleteDate.Value.ToString("yyyy-MM-dd");
                    }
                    else
                    {
                        if (p.ApplyStatus == (int)ApplyStatus.Apply)
                        {
                            p.ModifyDate = "";
                        }
                        switch (p.ApplyStatus)
                        {
                            case (int)ApplyStatus.Apply:
                                p.ApplyStatusName = "已成功申请，等待HR查阅";
                                break;
                            case (int)ApplyStatus.Read:
                                p.ApplyStatusName = "HR已查看简历";
                                break;
                            case (int)ApplyStatus.Interested:
                                p.ApplyStatusName = "通过初步筛选";
                                break;
                            case (int)ApplyStatus.Approved:
                                { 
                                    if(string.IsNullOrEmpty(p.InterviewTime))
                                        p.ApplyStatusName = "待安排面试";
                                    else
                                        p.ApplyStatusName = "收到面试通知";
                                }
                                break;
                            default:
                                p.ApplyStatus = (int)ApplyStatus.Refused;
                                p.ApplyStatusName = "不合适";
                                break;
                        }
                    }
                    if (p.PositionType != (int)PositionType.FullTime)
                        p.PositionType = 2;
                    if (!string.IsNullOrEmpty(p.PositionFunctionIds)) 
                    {
                        p.PositionFunctionIds = p.PositionFunctionIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
                    }
                    if (!string.IsNullOrEmpty(p.PositionFunctionName))
                    {
                        p.PositionFunctionName = p.PositionFunctionName.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
                    }
                    if (p.IndustryName.Length > 20)
                        p.IndustryName = p.IndustryName.Substring(0, 15) + "...";
                }
                return p;
            }
        }

        public async Task<int> GetApplyFeedbackCount(int userId, DateTime startDate) 
        {
            if (userId <= 0) return 0;
            using (var conn = DapperHelper.Get22Connection())
            {
                string sql = string.Format(@"
select count(1) from TB_S_Position
where ModifyDate > '{0}' and ModifyDate > CreateDate
and (ApplyStatus > 21 or DeleteDate is not null) and UserId = {1}
", startDate.ToString("yyyy-MM-dd HH:mm:ss"), userId);
                return (await conn.QueryAsync<int>(sql)).FirstOrDefault();
            }
        }

        public void UpdateStaus(int applyId, int status)
        {
            using (var conn = DapperHelper.Get22Connection())
            {
                string sql = @"update TB_S_Position set applystatus = @status, ModifyDate = getdate() where applyId = @applyId and applystatus <> @status";
                conn.Execute(sql, new { @applyId = applyId, @status = status });
            }
        }

        #endregion

    }

    public interface IApplicationRepo : IRepository<TB_S_Position>
    {

        IList<JobInApply> GetAppliedJobs(int userId);
        IList<JobInApply> GetAppliedJobs(int userId, int pageNo, int pageSize, out int recCount);

        /// <summary>
        /// 获得企业的职位申请列表
        /// </summary>
        PagedResult<ApplyItem> GetApplicationList(int enterpriseId, ApplicationSearchModel sm, int page, int pageSize, out int filterCnt);


        IList<TB_S_Position> GetFilterOutList(int enterpriseId, ApplicationSearchModel sm);

        /// <summary>
        /// 获取学生的职位申请列表
        /// </summary>
        PagedResult<ApplyItem> GetApplyList(int userId, int? applyStatus, int page, int pageSize);

        /// <summary>
        /// 获取单个应聘信息
        /// </summary>
        /// <param name="ApplyId"></param>
        /// <param name="enterpriseId"></param>
        /// <returns></returns>
        ApplyItem GetApplicationById(int ApplyId, int enterpriseId);
        void GetApplicationCount(int enterpriseId, string posId, out int allCount);

        void GetApplicationCount(int enterpriseId, out int interestedCount, out int approvedCount, out int needHandleCnt);

        Dictionary<int, string> GetApplyCurrentLocations(int enterpriseId, ApplicationSearchModel sm);
        Dictionary<string, string> GetApplySchools(int enterpriseId, ApplicationSearchModel sm);
        Dictionary<string, string> GetApplyMajors(int enterpriseId, ApplicationSearchModel sm);

        void CalcMatchDegree(int positionId);

        bool IsStuApplied(int stuId);
        bool IsStuAppliedWithFeedBack(int stuId);

        InterviewViewInfoModel GetInterviewViewInfoModel(int applyId, int enterpriseId);


        Task CreateAsync(int jobId, int userId);

        #region dapper

        Task<IList<AppliedJobDTO>> GetAppliedJobsAsync(int userId, int status);

        Task<AppliedJobDTO> GetAppliedJobDetailAsync(int applyId);

        /// <summary>
        /// 获取申请反馈的数量
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<int> GetApplyFeedbackCount(int userId, DateTime startDate);

        void UpdateStaus(int applyId, int status);

        #endregion
    }
}
