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
using Topuc22Top.Common;
namespace Topuc22Top.Data.Repositories
{
    public class StuCloudVideoRepo : RepositoryBase<TB_Stu_CloudVideo>, IStuCloudVideoRepo
    {
        public StuCloudVideoRepo(IDatabaseFactory databaseFActory)
            : base(databaseFActory)
        {
        }
        public bool Create(TB_Stu_CloudVideo cloudVideo)
        {
            DataContext.TB_Stu_CloudVideo.Add(cloudVideo);
         return     DataContext.SaveChanges()>0?true:false;
        }

         public bool IsHasUpload(int UserId)
        {
            var count = Dapper.DapperHelper.SqlQuery22<int>("select count(a.VideoId) from TB_Stu_CloudVideo a where a.UserId=@UserId and a.Status<>@VideoUploadStatus", new { UserId = UserId, VideoUploadStatus = VideoUploadStatus.CanPlay}).FirstOrDefault();
            return count>0?true:false;
        }
         public ViderResumeDTO GetByUserId(int UserId)
        {
            var video = Dapper.DapperHelper.SqlQuery22<ViderResumeDTO>(@"	select a.VideoId,a.CloudFileId as VideoUrl,a.Status as VideoStatus from TB_Stu_CloudVideo a 
where a.UserId=@UserId  and a.Status=@VideoUploadStatus order by a.CreateDate desc", new { UserId = UserId, VideoUploadStatus = VideoUploadStatus.CanPlay }).FirstOrDefault();
            return video;
        }

        public string GetCloudUrlByUserId(int UserId)
        {
            var url = Dapper.DapperHelper.SqlQuery22<string>(@"select  a.CloudFileId from TB_Stu_CloudVideo a
 where a.UserId=@UserId and a.Status=@VideoUploadStatus
 order by a.CreateDate desc", new { UserId = UserId, VideoUploadStatus = VideoUploadStatus.CanPlay }).FirstOrDefault();
            return url;
        }

    }
    public interface IStuCloudVideoRepo : IRepository<TB_Stu_CloudVideo>
    {
        bool Create(TB_Stu_CloudVideo cloudVideo);
        /// <summary>
        /// 判断是否有转码中的视频，如果有的话就不让上传
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        bool IsHasUpload(int UserId);
        ViderResumeDTO GetByUserId(int UserId);
        /// <summary>
        /// 企业端用，返回云视频播放地址，没有为空
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        string GetCloudUrlByUserId(int UserId);
    }
}
