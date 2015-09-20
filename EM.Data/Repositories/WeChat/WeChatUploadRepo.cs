using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topuc22Top.Common;
using Topuc22Top.Data.Infrastructure;
using Topuc22Top.Model.Entities;

namespace Topuc22Top.Data.Repositories
{
    public class WeChatUploadRepo : RepositoryBase<WeChatUpload>, IWeChatUploadRepo
    {
        public WeChatUploadRepo(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }
        public void AddObject(int userId, string openId, int mediaType, string mediaId, string thumbMediaId, int status)
        {
            if (string.IsNullOrEmpty(openId) || mediaType == 0 || string.IsNullOrEmpty(mediaId))
                return;
            var obj = new WeChatUpload()
            {
                UserId = userId,
                WeChatOpenId = openId,
                MediaType = mediaType,
                MediaId = mediaId,
                ThumbMediaId = thumbMediaId,
                Status = status,
                CreateDate = DateTime.Now
            };
            DataContext.WeChatUpload.Add(obj);
            DataContext.SaveChanges();
        }
        public void UpdateStatus(string ids, int status)
        {
            string sql = string.Format("update WeChatUpload set Status = {0} where UploadId in '{1}'", (int)status, ids);
            DataContext.Database.ExecuteSqlCommand(sql);
        }
        public void LinkSuccess(int id, string link)
        {
            string sql = string.Format("update WeChatUpload set Link = '{1}', Status = {2} where UploadId = {0}", id.ToString(), link, (int)WeChatUploadStatus.Success);
            DataContext.Database.ExecuteSqlCommand(sql);
        }
        public void LinkFailed(int id) 
        {
            //失败2次后Status置为Fail
            string sql = string.Format(@"
IF (SELECT isnull(FailTimes,0) FROM WeChatUpload where UploadId = {0}) < 4
   update WeChatUpload set FailTimes = isnull(FailTimes,0) + 1, Status = {1} where UploadId = {0}
ELSE
   BEGIN
	update WeChatUpload set Link = '', Status = {2} where UploadId = {0}
   END", id.ToString(), (int)WeChatUploadStatus.Wait, (int)WeChatUploadStatus.Fail);
            DataContext.Database.ExecuteSqlCommand(sql);
        }
        public void StatusSetFailed(int id)
        {
            //Status置为Failed
            string sql = string.Format("update WeChatUpload set Link = '', Status = {2} where UploadId = {0}", id.ToString(), (int)WeChatUploadStatus.Fail);
            DataContext.Database.ExecuteSqlCommand(sql);
        }

        public void SyncUserId()
        {
            //根据OpenId来更新UserId
            string sql = "exec SP_WeChatUpload_UserIdSync";
            DataContext.Database.ExecuteSqlCommand(sql);
        }

        public string GetVideoUrlByUserId(int userId)
        {
            return DataContext.WeChatUpload.Where(p => p.UserId == userId).OrderByDescending(p => p.CreateDate).Select(p => p.Link).FirstOrDefault();
        }

        public void UploadRecord(int userId, string openId, int mediaType, string mediaId, string thumbMediaId, int status)
        {
            if (string.IsNullOrEmpty(openId) || mediaType == 0 || string.IsNullOrEmpty(mediaId))
                return;
            //openid 唯一
            var entity = DataContext.WeChatUpload.Where(p => p.WeChatOpenId == openId).FirstOrDefault();
            if (entity == null)
                entity = new WeChatUpload();

            entity.MediaType = mediaType;
            entity.MediaId = mediaId;
            entity.ThumbMediaId = thumbMediaId;
            entity.WeChatOpenId = openId;
            entity.CreateDate = DateTime.Now;
            entity.Status = status;
            entity.UserId = userId;
            //entity.Link = "";
            entity.FailTimes = 0;

            if(entity.UploadId == 0)
                DataContext.WeChatUpload.Add(entity);
            
            DataContext.SaveChanges();
        }
        public void DeleteByUserId(int UserId)
        {
        var  video=   DataContext.WeChatUpload.Where(o => o.UserId == UserId).FirstOrDefault();
          if(video!=null)
          {
              DataContext.WeChatUpload.Remove(video);
              DataContext.SaveChanges();
          }
        }

    }
    public interface IWeChatUploadRepo : IRepository<WeChatUpload>
    {
        void AddObject(int userId, string openId, int mediaType, string mediaId, string thumbMediaId, int status);
        void UpdateStatus(string ids, int status);
        void LinkSuccess(int id, string link);
        void LinkFailed(int id);
        void StatusSetFailed(int id);
        void SyncUserId();
        string GetVideoUrlByUserId(int userId);
        void UploadRecord(int userId, string openId, int mediaType, string mediaId, string thumbMediaId, int status);


        void DeleteByUserId(int UserId);
    }
}
