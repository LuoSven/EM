using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topuc.Framework.Cache;
using Topuc22Top.Data.Infrastructure;
using Topuc22Top.Data.Repositories;
using Topuc22Top.Model.Entities;

namespace Topuc22Top.Data.ResultModel
{
    public class JobInApply
    {
        public int ApplyID { get; set; }

        public int JobID { get; set; }
        public string JobName { get; set; }

        public int EtpID { get; set; }
        public string EtpName { get; set; }

        public DateTime ApplyTime { get; set; }

        public int ApplyStatus { get; set; }

        public int ResumeID { get; set; }
        public string ResumeName { get; set; }

        public string City { get; set; }

        public DateTime? ModifyDate { get; set; }

        public Stu_Interview InterView { get; set; }

        //----Green 2014-7-28 职位是否过期字段
        public int PositionStatus { get; set; }
        public DateTime DeadLine { get; set; }
        //---------------------------------
    }

    public class ApplyItem
    {
        public int ApplyId { get; set; }
        public int PositionId { get; set; }
        public string Position { get; set; }
        public int stuCity { get; set; }
        public int jobCity { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public DateTime ApplyTime { get; set; }
        public int ApplyStatus { get; set; }

        //以下是2014-8-15 新增字段
        //public bool Gender { get; set; }
        //2014-12-19修改类型为bool?，因为存在未填情况
        public bool? Gender { get;set;}
        public int? CurrentLocation { get; set; }
        public int? DegreeId { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public int MatchDegree { get; set; }
        public IList<string> SchoolNames { get; set; }
        public IList<string> Majors { get; set; }
        public int OrderStatus { get; set; }
        public bool IsFilterOut { get; set; }

        public string FilterOutResons { get; set; }

        public DateTime? ModifyDate { get; set; }

        public string EtpName { get; set; }

        public int EtpId { get; set; }

        public string Function { get; set; }

        public string VideoUrl { get; set; }

        public bool IsInvited { get; set; }
        /// <summary>
        /// 应聘备注，备注列表
        /// </summary>
        public IList<Apply_EtpNote> NoteList { get; set; }
        /// <summary>
        /// 收藏日期
        /// </summary>
        public DateTime? FavoriteTime { get; set; }
        /// <summary>
        /// 进回收箱的日期
        /// </summary>
        public DateTime? DeleteDate { get; set; }
        public int EtpNoteCount { get; set; }
    }

    //职位及应聘状态
    public class PositionAndApplyStatus
    {
        public int PositionId { get; set; }
        public string PositionName { get; set; }
        public int PositionStatus { get; set; }
        //过期时间
        public DateTime ExpiredDate { get; set; }
        //public int ApplyCount { get; set; }
        public int CityId { get; set; }
        public string CityName { get; set; }
        public int SalaryMin { get; set; }
        public int SalaryMax { get; set; }
        public Nullable<int> InternSalaryType { get; set; }

        public string SalaryRange 
        {
            get 
            {
                IDictItemRepo dictItemRepo = new DictItemRepo(new DatabaseFactory(), new InMemoryCache());
                return dictItemRepo.GetSalaryRange(SalaryMin, SalaryMax, InternSalaryType ?? 0);
            } 
        }
        public int PositionType { get; set; }
        public string RecruitCount { get; set; }
        public string DegreeIds { get; set; }

        public string DegreeNames
        {
            get
            {
                IDictItemRepo dictItemRepo = new DictItemRepo(new DatabaseFactory(), new InMemoryCache());
                return dictItemRepo.GetDegreeNames(DegreeIds, "、");
            }
        }
    }

    /// <summary>
    /// 用来企业端前后台传输信息用，AppItem太大不适合传送信息
    /// /// </summary>
    public class Apply
    {
        public int ApplyId { get; set; }
        public string Position { get; set; }
        public int UserId { get; set; }
        public int PushId { get; set; }
        public int PositionId { get; set; }
        public string UserAvatarUrl {get;set;}
        public string UserName { get; set; }
        public string jobCityName { get; set; }
        public string ApplyTime { get; set; }
        public int ApplyStatus { get; set; }
        public string ApplyStatusName { get; set; }
        public int? DegreeId { get; set; }
        public string GenderName { get; set; }
        public string CurrentLocationName { get; set; }
        public string DegreeName { get; set; }
        public string PositionUrl { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string SchoolName { get; set; }
        public string Major { get; set; }
        public int OrderStatus { get; set; }
        public IList<ApplyNote> NoteList { get; set; }
        public bool IsFavorite { get; set; }
        public ApplyInterView InterViewInfo { get; set; }

    }
    //用来前后台传输的面试信息
    public class ApplyInterView
    {
        public string InterviewTime { get; set; }
        public string InterviewPlace { get; set; }
        public string strContact { get; set; }
    }
    //用来前后台传输的备注信息
    public class ApplyNote
    {
        public int Id { get; set; }
        public string CreateDateD { get; set; }
        public string CreateDateH { get; set; }
        public string Note { get; set; }
    }

}
