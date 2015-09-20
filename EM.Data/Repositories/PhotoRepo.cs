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
using Topuc22Top.Data.Dapper;

using Dapper;

namespace Topuc22Top.Data.Repositories
{
    public class PhotoRepo : RepositoryBase<Photo>, IPhotoRepo
    {
        public PhotoRepo(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }
        public IList<Photo> GetPhotos(int enterpriseId, int? count = null)
        {
            var query = from m in DataContext.Photo
                        where m.EnterpriseId == enterpriseId
                        select m;
            if (count.HasValue)
            {
                return query.OrderByDescending(x => x.UploadDate).Take(count.Value).ToList();
            }
            else
            {
                return query.OrderByDescending(x => x.UploadDate).ToList();
            }
        }

        public IList<Photo> GetSelectedPhotos(int enterpriseId, int takeNo, string proPhotoIds)
        {
            List<int> idArr = new List<int>() { };
            if (!string.IsNullOrEmpty(proPhotoIds))
            {
                idArr = proPhotoIds.Split(',').Select(idItemStr =>
                {
                    int idItemId = 0;
                    int.TryParse(idItemStr, out idItemId);
                    return idItemId;
                }).Where(p => p > 0).Distinct().ToList();
            }
            var query = from m in DataContext.Photo
                        where m.EnterpriseId == enterpriseId
                        && idArr.Contains(m.PhotoId)
                        select m;
            return query.OrderByDescending(p => p.UploadDate).Take(takeNo).ToList();
        }

        public int GetPhotoCount(int enterpriseId, int albumId)
        {
            var query = from m in DataContext.Photo
                        where m.EnterpriseId == enterpriseId && m.AlbumId == albumId
                        select m;
            return query.Count();
        }
        public IList<Photo> GetEtpAlbumPhotos(int albumId, int page, int pageSize, out int count)
        {
            var query = from m in DataContext.Photo
                        where m.AlbumId == albumId
                        select m;
            count = query.Count();

            return query.OrderByDescending(x => x.UploadDate).Skip(pageSize * (page - 1)).Take(pageSize).ToList();
        }

        public string GetAlbumPhotoTitle(int albumId)
        {
            var query = (from m in DataContext.Photo
                         join a in DataContext.TB_Enterprise_Album on m.PhotoId equals a.CoverId
                         where a.AlbumId == albumId
                         select m.Title).FirstOrDefault();
            if (query != null && query.Count() > 0)
            {
                return query.ToString();
            }
            else
            {
                query = (from m in DataContext.Photo
                         where m.AlbumId == albumId
                         select m.Title).FirstOrDefault();
                if (query != null)
                {
                    return query.ToString();
                }
                else
                {
                    return string.Empty;
                }
            }
        }
        public IList<Photo> GetEtpAlbumPhotos(int albumId)
        {
            int coverId = DataContext.TB_Enterprise_Album.Find(albumId).CoverId ?? 0;
            return DataContext.Photo.Where(x => x.AlbumId == albumId)
                .OrderByDescending(x => x.PhotoId == coverId)
                .ThenByDescending(x => x.UploadDate).ToList();
        }

        public async Task<IList<Photo>> GetEtpAlbumPhotosAsync(int albumId)
        {
            return await DataContext.Photo.Where(x => x.AlbumId == albumId).OrderByDescending(p => p.UploadDate).ToListAsync();
        }

        public int GetPhotoCount(int enterpriseId)
        {
            return DataContext.Photo.Where(x => x.EnterpriseId == enterpriseId).Count();
        }

        #region
        public async Task<IList<Photo>> GetSomeEtpPhotosAsync(int enterpriseId, int? count = null)
        {
            using (TopucDB context = new TopucDB())
            {
                var query = from m in context.Photo
                            where m.EnterpriseId == enterpriseId
                            select m;
                if (count.HasValue)
                {
                    return await query.OrderByDescending(x => x.UploadDate).Take(count.Value).ToListAsync();
                }
                else
                {
                    return await query.OrderByDescending(x => x.UploadDate).ToListAsync();
                }
            }
        }

        public async Task<IList<Photo>> GetEtpAlbumPhotosAsync(int albumId, int? count = null)
        {
            using (TopucDB context = new TopucDB())
            {
                var query = from m in context.Photo
                            where m.AlbumId == albumId
                            select m;
                if (count.HasValue)
                {
                    return await query.OrderByDescending(x => x.UploadDate).Take(count.Value).ToListAsync();
                }
                else
                {
                    return await query.OrderByDescending(x => x.UploadDate).ToListAsync();
                }
            }
        }

        public async Task<PagedResult<Photo>> GetEtpPhotosAsync(int enterpriseId, int albumId = 0, int page = 1, int pageSize = 10)
        {
            PagedResult<Photo> result = new PagedResult<Photo>();
            result.CurrentPage = page;
            result.PageSize = pageSize;
            using (TopucDB context = new TopucDB())
            {
                var query = from q in context.Photo
                            where q.EnterpriseId == enterpriseId
                            select q;

                if (albumId != 0)
                {
                    query = query.Where(p => p.AlbumId == albumId);
                }

                result.RowCount = query.Count();

                result.Results = await query.OrderByDescending(a => a.UploadDate).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            }
            return result;
        }

        public async Task<IList<Photo>> GetAllPhotosAsync(int enterpriseId, int? albumId = null)
        {
            var query = from p in DataContext.Photo
                        where p.EnterpriseId == enterpriseId    
                        select p;
            if (albumId.HasValue && albumId.Value != 0)
            {
                query = query.Where(p => p.AlbumId == albumId.Value);
            }
            return await query.OrderByDescending(x => x.UploadDate).ToListAsync();
        }

        public IList<Photo> GetAllPhotos(int enterpriseId, int? albumId = null)
        {
            var query = from p in DataContext.Photo
                        where p.EnterpriseId == enterpriseId
                        select p;
            if (albumId.HasValue && albumId.Value != 0)
            {
                query = query.Where(p => p.AlbumId == albumId.Value);
            }
            return query.OrderByDescending(x => x.UploadDate).ToList();
        }

        public KeyValuePair<string, int> GetCoverAndCnt(int id)
        {
            var photos = DataContext.Photo.Where(a => a.EnterpriseId == id);
            var query = from a in DataContext.TB_Enterprise_Album
                        where a.EnterpriseId == id && a.CoverId != null
                        select a;
            int cnt = photos.Count();
            string coverFile = string.Empty;
            var photo1 = photos.OrderByDescending(a => a.UploadDate).FirstOrDefault();
            if (photo1 != null)
            {
                coverFile = photo1.Path;
            }
            if (query.Any())
            {
                int coverId = query.OrderByDescending(a => a.CreateTime).Select(a => a.CoverId).FirstOrDefault().Value;
                var cover = photos.Where(p => p.PhotoId == coverId).FirstOrDefault();
                if (cover != null)
                {
                    coverFile = cover.Path;
                }
            }
            return new KeyValuePair<string, int>(coverFile, cnt);
        }

        public int getAlbumId(int id)
        {
            var query = from p in DataContext.Photo
                        where p.PhotoId == id
                        select p;
            if (query.Any())
            {
                return query.FirstOrDefault().AlbumId;
            }
            return 0;
        }

        #endregion

        public string GetUnDeletedPhotos(string photoIds)
        {
            if (string.IsNullOrEmpty(photoIds)) return string.Empty;
            var idArr = photoIds.Split(',').Select(idItemStr =>
                {
                    int idItemId = 0;
                    int.TryParse(idItemStr, out idItemId);
                    return idItemId;
                }).Where(p => p > 0).Distinct().ToList();
            var query = from p in DataContext.Photo
                        where idArr.Contains(p.PhotoId)
                        select p;
            photoIds = "";
            foreach (var item in query)
            {
                photoIds += item.PhotoId + ",";
            }
            photoIds = photoIds.Trim(',');
            return photoIds;
        }

        public async Task<IList<EtpPhotoDynDTO>> GetEtpPhotoDynamicList(string companyIds, int days)
        {
            using (var conn = DapperHelper.Get22Connection())
            {
                var startDate = DateTime.Now.AddDays((days > 0 ? -days : days)).Date;
                string sql = string.Format(@"
select a.EnterpriseId, b.Name EnterpriseName, a.Path, a.UploadDate 
from Photo a
left join TB_Enterprise b
on a.EnterpriseId = b.EnterpriseId
where a.EnterpriseId in ({0}) and a.UploadDate >= @startDate
and b.Status <> 99
and ISNULL(a.Path,'') <> ''",companyIds);
                var list = await conn.QueryAsync<EtpPhotoDynDTO>(sql, new { @startDate = startDate });
                return list.ToList();
            }
        }


        public async Task<IList<PhotoDTO>> GetPhotosByAlbumIdAsync(int AlbumId)
        {
            var list = await DapperHelper.SqlQuery22Async<PhotoDTO>(string.Format(@"select a.Path,a.Description,a.PhotoId from Photo a where a.AlbumId={0}", AlbumId));
            return list.ToList();
        }

    }
    public interface IPhotoRepo : IRepository<Photo>
    {
        /// <summary>
        /// 获取企业最近的四张图片
        /// </summary>
        /// <param name="enterpriseId"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        IList<Photo> GetPhotos(int enterpriseId, int? count = null);
        /// <summary>
        /// 获取企业的图片数
        /// </summary>
        /// <param name="enterpriseId"></param>
        /// <returns></returns>
        int GetPhotoCount(int enterpriseId);
        /// <summary>
        /// 异步获取企业最近指定数量的图片
        /// </summary>
        /// <param name="enterpriseId">公司ID</param>
        /// <param name="count">返回图片的数量</param>
        Task<IList<Photo>> GetSomeEtpPhotosAsync(int enterpriseId, int? count = null);

        Task<PagedResult<Photo>> GetEtpPhotosAsync(int enterpriseId, int albumId = 0, int page = 1, int pageSize = 10);


        Task<IList<Photo>> GetAllPhotosAsync(int enterpriseId, int? albumtId = null);

        IList<Photo> GetAllPhotos(int enterpriseId, int? albumId = null);


        Task<IList<Photo>> GetEtpAlbumPhotosAsync(int albumId, int? count = null);

        int GetPhotoCount(int enterpriseId, int albumId);
        IList<Photo> GetEtpAlbumPhotos(int albumId, int page, int pageSize, out int count);
        IList<Photo> GetEtpAlbumPhotos(int albumId);


        Task<IList<Photo>> GetEtpAlbumPhotosAsync(int albumId);
        string GetAlbumPhotoTitle(int albumId);

        int getAlbumId(int id);

        KeyValuePair<string, int> GetCoverAndCnt(int id);
        /// <summary>
        /// 获取指定数量的相册，优先给定的Ids
        /// </summary>
        /// <param name="enterpriseId"></param>
        /// <param name="takeNo"></param>
        /// <param name="photoIds"></param>
        /// <returns></returns>
        IList<Photo> GetSelectedPhotos(int enterpriseId, int takeNo, string proPhotoIds);

        string GetUnDeletedPhotos(string photoIds);

        Task<IList<PhotoDTO>> GetPhotosByAlbumIdAsync(int AlbumId);


        #region Dapper

        Task<IList<EtpPhotoDynDTO>> GetEtpPhotoDynamicList(string companyIds, int days);
        #endregion
    }
}
