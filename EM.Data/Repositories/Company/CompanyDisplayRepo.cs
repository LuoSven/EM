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
    public class CompanyDisplayRepo : RepositoryBase<TB_Enterprise_Display>, ICompanyDisplayRepo
    {
        public CompanyDisplayRepo(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }

        public Dictionary<int, string> GetBackColorList()
        {
            var query = from c in DataContext.TB_Enterprise_Display
                        where c.BackgroudColor != string.Empty
                        select c;
            return query.ToDictionary(c => c.EnterpriseId, c => c.BackgroudColor);
        }
        public bool HasSloganPic(int enterpriseId)
        {
            var query = (from m in DataContext.TB_Enterprise_Display
                         where m.EnterpriseId == enterpriseId
                         select m).FirstOrDefault();
            if (query != null && query.SloganPic.HasValue && query.SloganPic.Value == true)
            {
                return true;
            }

            return false;
        }
        public bool HasLogoPic(int enterpriseId)
        {
            var query = (from m in DataContext.TB_Enterprise_Display
                         where m.EnterpriseId == enterpriseId
                         select m).FirstOrDefault();
            if (query != null && query.LogoPic.HasValue && query.LogoPic.Value == true)
            {
                return true;
            }

            return false;
        }
        public bool HasBannerPic(int enterpriseId)
        {
            var query = (from m in DataContext.TB_Enterprise_Display
                         where m.EnterpriseId == enterpriseId
                         select m).FirstOrDefault();
            if (query != null && query.BannerPic.HasValue && query.BannerPic.Value == true)
            {
                return true;
            }

            return false;
        }

        public async Task<bool> HasBannerPicAsync(int enterpriseId)
        {
            using (TopucDB DataContext = new TopucDB())
            {
                return await DataContext.TB_Enterprise_Display.AnyAsync(m => m.EnterpriseId == enterpriseId && m.BannerPic.HasValue && m.BannerPic.Value);
            }
        }

        public bool HasQAStatus(int enterpriseId)
        {
            return DataContext.TB_Enterprise_Display.Any(m => m.EnterpriseId == enterpriseId && m.QAStatus.HasValue && m.QAStatus.Value == true);
        }

        public async Task<bool> HasQAStatusAsync(int enterpriseId)
        {
            using (TopucDB DataContext = new TopucDB())
            {
                return await DataContext.TB_Enterprise_Display.AnyAsync(m => m.EnterpriseId == enterpriseId && m.QAStatus.HasValue && m.QAStatus.Value == true);
            }
        }
        public int? GetMoonLightPic(int enterpriseId)
        {
            var query = (from m in DataContext.TB_Enterprise_Display
                         where m.EnterpriseId == enterpriseId
                         select m).FirstOrDefault();
            if (query != null)
            {
                return query.MoonLightPic;
            }
            else
            {
                return null;
            }
        }

        public int? GetMicroPageMusic(int enterpriseId)
        {
            var query = (from m in DataContext.TB_Enterprise_Display
                         where m.EnterpriseId == enterpriseId
                         select m).FirstOrDefault();
            if (query != null)
            {
                return query.MicroPageMusic;
            }
            else
            {
                return null;
            }
        }

        public TB_Enterprise_Display GetByEtpId(int enterpriseId)
        {
            return (from m in DataContext.TB_Enterprise_Display
                    where m.EnterpriseId == enterpriseId
                    select m).FirstOrDefault();
        }

        public async Task<TB_Enterprise_Display> GetByEtpIdAsync(int enterpriseId)
        {
            return await (from m in DataContext.TB_Enterprise_Display
                          where m.EnterpriseId == enterpriseId
                          select m).FirstOrDefaultAsync();
        }

    }
    public interface ICompanyDisplayRepo : IRepository<TB_Enterprise_Display>
    {

        Dictionary<int, string> GetBackColorList();
        /// <summary>
        /// 判断企业是否有宣传图片
        /// </summary>
        /// <param name="enterpriseId"></param>
        /// <returns></returns>
        bool HasSloganPic(int enterpriseId);
        /// <summary>
        /// 判断企业是否有logo图片
        /// </summary>
        /// <param name="enterpriseId"></param>
        /// <returns></returns>
        bool HasLogoPic(int enterpriseId);
        /// <summary>
        /// 判断企业是否有banner图片
        /// </summary>
        /// <param name="enterpriseId"></param>
        /// <returns></returns>
        bool HasBannerPic(int enterpriseId);
        Task<bool> HasBannerPicAsync(int enterpriseId);
        /// <summary>
        /// 判断企业是否开启QA
        /// </summary>
        /// <param name="enterpriseId"></param>
        /// <returns></returns>
        bool HasQAStatus(int enterpriseId);
        Task<bool> HasQAStatusAsync(int enterpriseId);

        /// <summary>
        /// 获得顶部背景图
        /// </summary>
        /// <param name="enterpriseId"></param>
        /// <returns></returns>
        int? GetMoonLightPic(int enterpriseId);

        /// <summary>
        /// 企业微官网H5页面的背景音乐
        /// </summary>
        /// <param name="enterpriseId"></param>
        /// <returns></returns>
        int? GetMicroPageMusic(int enterpriseId);
        TB_Enterprise_Display GetByEtpId(int enterpriseId);

        Task<TB_Enterprise_Display> GetByEtpIdAsync(int enterpriseId);
    }
}
