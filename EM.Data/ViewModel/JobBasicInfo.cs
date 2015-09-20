using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Topuc22Top.Data.ViewModel
{
    public class JobBasicInfo
    {
        public int EnterpriseId { get; set; }
        public string Enterprise { get; set; }
        public int PositionId { get; set; }
        public string Position { get; set; }
        public string Citys { get; set; }

        /// <summary>
        /// 用来存id的
        /// </summary>
        public string City { get; set; }
        public DateTime? DeployTime { get; set; }

        public string PosDescription { get; set; }



        public int CompareTo(JobBasicInfo jbi)
        {
            if (jbi == null)
                throw new ArgumentNullException("jobbasicinfo");
            return PositionId.CompareTo(jbi.PositionId);
        }


        //public override bool Equals(object obj)
        //{
        //    JobBasicInfo info = (JobBasicInfo)obj;
        //    return info.PositionId==PositionId;
        //}
    }


    /// <summary>  
    /// 自定义比较类  
    /// </summary>  
    public class JobBasicInfoEntityComparer : IEqualityComparer<JobBasicInfo>
    {
        public bool Equals(JobBasicInfo a, JobBasicInfo b)
        {
            if (Object.ReferenceEquals(a, b)) return true;
            if (Object.ReferenceEquals(a, null) || Object.ReferenceEquals(b, null))
                return false;

            return a.PositionId == b.PositionId && a.Position == b.Position;
        }

        public int GetHashCode(JobBasicInfo a)
        {
            if (Object.ReferenceEquals(a, null)) return 0;
            int hashName = a.Position == null ? 0 : a.Position.GetHashCode();
            int hashCode = a.PositionId.GetHashCode();

            return hashName ^ hashCode;
        }
    }

    public class CityCount
    {
        public string CityName { get; set; }
        public int Count { get; set; }
    }
}
