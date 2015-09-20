using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topuc22Top.Data.Infrastructure;
using Topuc22Top.Model.Entities;
using System.Data.Entity;
using Topuc22Top.Common;
using Topuc22Top.Model.DTOs;

using Dapper;
using Topuc22Top.Data.Dapper;

namespace Topuc22Top.Data.Repositories
{

    public class OnlineVideoRepo : RepositoryBase<TB_Enterprise_OnlineVideo>, IOnlineVideoRepo
    {
        public OnlineVideoRepo(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }
        private const string DefultUrl = "http://static.zheyibu.com/Images/avatar/video-browser.png";

        public IList<TB_Enterprise_OnlineVideo> GetVideos(int enterpriseId, int? count = null)
        {
            var query = from m in DataContext.TB_Enterprise_OnlineVideo
                        where m.EnterpriseId == enterpriseId && m.AlbumType != (int)OnlineAlbumType.Life
                        select m;
            if (count.HasValue)
            {
                return query.OrderByDescending(x => x.CreateTime).Take(count.Value).ToList();
            }
            else
            {

                return query.OrderByDescending(x => x.CreateTime).ToList();
            }
        }

        public int GetVideosCount(int enterpriseId)
        {
            var query = from m in DataContext.TB_Enterprise_OnlineVideo
                        where m.EnterpriseId == enterpriseId && m.AlbumType != (int)OnlineAlbumType.Life
                        select m;
            return query.Count();
        }

        public int GetVideosCount(int enterpriseId, int videoType)
        {
            var query = from m in DataContext.TB_Enterprise_OnlineVideo
                        where m.EnterpriseId == enterpriseId && m.VideoType != videoType
                        select m;
            return query.Count();
        }

        #region
        public async Task<IList<TB_Enterprise_OnlineVideo>> GetVideosAsync(int enterpriseId, int? count = null)
        {
            using (TopucDB context = new TopucDB())
            {
                var query = from m in context.TB_Enterprise_OnlineVideo
                            where m.EnterpriseId == enterpriseId && m.AlbumType.Value != (int)OnlineAlbumType.Life
                            select m;
                if (count.HasValue)
                {
                    return await query.OrderByDescending(x => x.CreateTime).Take(count.Value).ToListAsync();
                }
                else
                {

                    return await query.OrderByDescending(x => x.CreateTime).ToListAsync();
                }
            }
        }
        #endregion


        #region Dapper
        public async Task<IList<EtpVideoDynDTO>> GetEtpVideoDynamicListAsync(string companyIds, int days)
        {
            var startDate = DateTime.Now.AddDays((days > 0 ? -days : days)).Date;
            var list = await DapperHelper.SqlQuery22Async<EtpVideoDynDTO>(string.Format(@"select * from
 (
select a.EnterpriseId, b.Name EnterpriseName, a.ImgUrl, a.OnlineKey,cast(@Online as varchar(1)) +'-'+cast(a.VideoType as varchar(1))  as Type,a.CreateTime,a.Title  from TB_Enterprise_OnlineVideo a
left join TB_Enterprise b on a.EnterpriseId = b.EnterpriseId
where a.EnterpriseId in ({0}) and a.CreateTime >= @startDate  and b.Status <> 99  and ISNULL(a.ImgUrl,'') <> '' and ISNULL(a.OnlineKey,'') <> ''
union all
(
select  a.EnterpriseId, b.Name EnterpriseName,a.ImgUrl ,a.CloudFileId as OnlineKey,cast(@EtpCloud as varchar(1)) +'-'+cast(a.VideoType as varchar(1))  as Type,a.CreateDate as CreateTime,a.Title   from TB_Enterprise_CloudVideo a 
left join TB_Enterprise b on a.EnterpriseId = b.EnterpriseId
where a.EnterpriseId  in ({0}) and a.CreateDate >= @startDate and a.Status=@Status  and ISNULL(a.CloudFileId,'') <> ''
)
) as a
order by a.CreateTime desc 
", companyIds), new { DefultUrl = DefultUrl, Online = UploadVideoType.Online, EtpCloud = UploadVideoType.Cloud, startDate = startDate, Status = VideoUploadStatus.CanPlay });
            return list.ToList();
        }





        public IList<EtpVideoDTO> GetListDto(int enterpriseid, int pageIndex, int pageSize, out int totalCnt)
        {
            var Videos = DapperHelper.SqlQuery22<EtpVideoDTO>(@"select * from (
(
select a.Title ,a.VideoId,a.VideoType,a.ImgUrl ,a.OnlineKey ,1 as VideoStatus,@Online as UploadVideoType,a.CreateTime as UpdateTime from TB_Enterprise_OnlineVideo a 
where a.EnterpriseId=@EnterpriseId
)
union all
(
select a.Title,a.VideoId,a.VideoType,a.ImgUrl,a.CloudFileId as OnlineKey,a.Status as VideoStatus,@EtpCloud as UploadVideoType,a.CreateDate as UpdateTime  from TB_Enterprise_CloudVideo a 
where a.EnterpriseId=@EnterpriseId and a.Status<>@DeleteStatus
)
) as a
order by a.UpdateTime desc ",
                 new { EnterpriseId = enterpriseid, Online = UploadVideoType.Online, EtpCloud = UploadVideoType.Cloud, DeleteStatus =VideoUploadStatus.Delete});
            totalCnt = Videos.Count();
            return Videos.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        }

        public async Task<IList<EtpVideoDTO>> GetVideosAsyncDto(int enterpriseId, int? count = null)
        {
            var Videos = await DapperHelper.SqlQuery22Async<EtpVideoDTO>(@"select * from (
(
select a.Title ,a.VideoId,a.VideoType,a.ImgUrl as Url,a.OnlineKey ,1 as VideoStatus,@Online as UploadVideoType,a.CreateTime as UpdateTime from TB_Enterprise_OnlineVideo a 
where a.EnterpriseId=@EnterpriseId 
)
union all
(
select a.Title,a.VideoId,a.VideoType,a.ImgUrl as Url,a.CloudFileId as OnlineKey,a.Status as VideoStatus,@EtpCloud as UploadVideoType,a.CreateDate as UpdateTime  from TB_Enterprise_CloudVideo a 
where a.EnterpriseId=@EnterpriseId  and  a.Status =@Status
)
) as a
order by a.UpdateTime desc ",
                 new { EnterpriseId = enterpriseId, DefultUrl = DefultUrl, Online = (int)UploadVideoType.Online, EtpCloud = (int)UploadVideoType.Cloud, Status = (int)VideoUploadStatus.CanPlay });

            if (count.HasValue)
            {
                return Videos.Take(count.Value).ToList();
            }
            else
            {

                return Videos.ToList();
            }
        }

        #endregion

    }

    public interface IOnlineVideoRepo : IRepository<TB_Enterprise_OnlineVideo>
    {
        IList<TB_Enterprise_OnlineVideo> GetVideos(int enterpriseId, int? count = null);
        Task<IList<TB_Enterprise_OnlineVideo>> GetVideosAsync(int enterpriseId, int? count = null);
        Task<IList<EtpVideoDTO>> GetVideosAsyncDto(int enterpriseId, int? count = null);
        int GetVideosCount(int enterpriseId);
        int GetVideosCount(int enterpriseId, int videoType);

        IList<EtpVideoDTO> GetListDto(int enterpriseid, int pageIndex, int pageSize, out int totalCnt);

        #region Dapper
        Task<IList<EtpVideoDynDTO>> GetEtpVideoDynamicListAsync(string companyIds, int days);


        #endregion
    }

}
