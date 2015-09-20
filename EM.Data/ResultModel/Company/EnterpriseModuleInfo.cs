using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Topuc22Top.Data.ResultModel
{
    public class EnterpriseModuleInfo
    {
        public bool HasLogo { get; set; }
        public bool HasBanner { get; set; }
        public bool HasSloganPic { get; set; }
        public bool HasDesc { get; set; }
        public bool HasWebSite { get; set; }
        public bool HasVideo { get; set; }
        public bool HasPhoto { get; set; }
        public bool HasBusiness { get; set; }
        public bool HasSolgan { get; set; }
        public bool HasStaffGrowth { get; set; }
        public bool HasNews { get; set; }
        public bool HasAddress { get; set; }
        public bool HasThirdLink { get; set; }
        public bool HasOpenQA { get; set; }

        /// <summary>
        /// 还没有完成的模块
        /// </summary>
        public int RemainCount { get { return GetRemainCount(); } set { remainCount = value; } }

        private int remainCount;

        private int GetRemainCount()
        {
            remainCount = 0;
            if (!HasLogo)
            {
                remainCount++;
            }
            if (!HasBanner)
            {
                remainCount++;
            }
            if (!HasSloganPic)
            {
                remainCount++;
            }
            if (!HasDesc)
            {
                remainCount++;
            }

            if (!HasWebSite)
            {
                remainCount++;
            }

            if (!HasVideo)
            {
                remainCount++;
            }

            if (!HasPhoto)
            {
                remainCount++;
            }

            if (!HasBusiness)
            {
                remainCount++;
            }

            if (!HasSolgan)
            {
                remainCount++;
            }

            if (!HasStaffGrowth)
            {
                remainCount++;
            }

            if (!HasNews)
            {
                remainCount++;
            }

            if (!HasAddress)
            {
                remainCount++;
            }

            if (!HasThirdLink)
            {
                remainCount++;
            }

            if (!HasOpenQA)
            {
                remainCount++;
            }
            return remainCount;
        }
    }
}
