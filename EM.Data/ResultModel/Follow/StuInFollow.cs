using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Topuc22Top.Common;

namespace Topuc22Top.Data.ResultModel
{
    /// <summary>
    /// 企业关注者基本信息（一般是学生）
    /// </summary>
    public class StuInFollow
    {
        public int StuID { get; set; }

        public string StuName { get; set; }

        public FollowStatus FollowStatus { get; set; }

        public bool Gender { get; set; }

        public string SchoolName { get; set; }
        //public string MajorId { get; set; }
        public string MajorNameExt { get; set; }
        //public string MajorName
        //{
        //    get
        //    {

        //        return string.Empty;
        //        //return FormalMajorManager.GetMajorNames(MajorId, "+");
        //        //PageHelper.ChooseString(MajorId, MajorNameExt, Components.ComponentType.Professional);
        //    }
        //}

        public DateTime? Birthday { get; set; }
        public string Age
        {
            get
            {
                return PageHelper.GetAge(Birthday);
            }
        }

    }
}
