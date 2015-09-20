using Dapper;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Topuc22Top.Data.Dapper;
using Topuc22Top.Data.Infrastructure;
using Topuc22Top.Data.ResultModel;
using Topuc22Top.Model.DTOs;
using Topuc22Top.Model.Entities;
namespace Topuc22Top.Data.Repositories
{


    public class CompanyAlbumRepo : RepositoryBase<TB_Enterprise_Album>, ICompanyAlbumRepo
    {
        public CompanyAlbumRepo(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }

        public async Task<IList<CompanyAlbumResult>> GetEnterpriseAlbumAsync(int enterpriseid, int? count = null)
        {
            using (TopucDB DataContext = new TopucDB())
            {
                var albums = from a in DataContext.TB_Enterprise_Album
                             join p in DataContext.Photo on a.CoverId.Value equals p.PhotoId into gp
                             from pp in gp.DefaultIfEmpty()

                             where a.EnterpriseId == enterpriseid && a.PhotoCount > 0
                             select new CompanyAlbumResult()
                             {
                                 AlbumId = a.AlbumId,
                                 CoverId = a.CoverId,
                                 CoverPath = pp.Path ?? string.Empty,
                                 CreateTime = a.CreateTime,
                                 EnterpriseId = a.EnterpriseId,
                                 Name = a.Name,
                                 PhotoCount = a.PhotoCount
                             };

                if (count.HasValue)
                {
                    return await albums.OrderByDescending(a => a.CreateTime).ThenByDescending(p => p.AlbumId).Take(count.Value).ToListAsync();
                }
                else
                {
                    return await albums.OrderByDescending(a => a.CreateTime).ThenByDescending(p => p.AlbumId).ToListAsync();
                }
            }
        }


        public async Task<bool> HasAlbumAsync(int enterpriseid)
        {
            using (TopucDB DataContext = new TopucDB())
            {
                return  await DataContext.TB_Enterprise_Album.Where(a=>a.EnterpriseId==enterpriseid&&a.PhotoCount>0).AnyAsync();
            }
        }

        public  IList<CompanyAlbumResult> GetEnterpriseAlbum(int enterpriseid, int? count = null)
        {

            var albums = from a in DataContext.TB_Enterprise_Album
                         join p in DataContext.Photo on a.CoverId.Value equals p.PhotoId into gp
                         from pp in gp.DefaultIfEmpty()

                         where a.EnterpriseId == enterpriseid && a.PhotoCount > 0
                         select new CompanyAlbumResult()
                         {
                             AlbumId = a.AlbumId,
                             CoverId = a.CoverId,
                             CoverPath = pp.Path ?? string.Empty,
                             CreateTime = a.CreateTime,
                             EnterpriseId = a.EnterpriseId,
                             Name = a.Name,
                             PhotoCount = a.PhotoCount
                         };

            if (count.HasValue)
            {
                return albums.OrderByDescending(a => a.CreateTime).ThenByDescending(p => p.AlbumId).Take(count.Value).ToList();
            }
            else
            {
                return albums.OrderByDescending(a => a.CreateTime).ThenByDescending(p => p.AlbumId).ToList();
            }

        }

        public IList<TB_Enterprise_Album> GetList(int enterpriseid, int? count = null)
        {
            var albums = from q in DataContext.TB_Enterprise_Album
                         where q.EnterpriseId == enterpriseid
                         select q;
            if (count.HasValue)
            {
                return albums.OrderByDescending(a => a.CreateTime).ThenByDescending(p => p.AlbumId).Take(count.Value).ToList();
            }
            else
            {
                return albums.OrderByDescending(a => a.CreateTime).ThenByDescending(p => p.AlbumId).ToList();
            }
        }

        public string GetAlbumName(int albumId)
        {
            var query = (from q in DataContext.TB_Enterprise_Album
                         where q.AlbumId == albumId
                         select q.Name).FirstOrDefault();
            if (query != null)
            {
                return query.ToString();
            }
            else
            {
                return string.Empty;
            }
        }

        public IList<TB_Enterprise_Album> GetList(int enterpriseid, int pageIndex, int pageSize, out int totalCnt)
        {
            var albums = from q in DataContext.TB_Enterprise_Album
                         where q.EnterpriseId == enterpriseid
                         select q;
            totalCnt = albums.Count();
            return albums.OrderByDescending(p => p.CreateTime).ThenByDescending(p => p.AlbumId).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        }

        public int GetOrCreateAlbumIdByName(int enterpriseId, string albumName, bool autoCreate = false)
        {
            var query = from q in DataContext.TB_Enterprise_Album
                         where q.EnterpriseId == enterpriseId && q.Name == albumName
                         select q.AlbumId;
            if (query.Any())
            {
                return query.FirstOrDefault();
            }
            else 
            {
                var entity = new TB_Enterprise_Album()
                {
                    EnterpriseId = enterpriseId,
                    Name = albumName,
                    CreateTime = DateTime.Now,
                    ModifyTime = DateTime.Now
                };
                DataContext.TB_Enterprise_Album.Add(entity);
                DataContext.SaveChanges();
                return entity.AlbumId;
            }
        }

     public async Task< IList<EtpAlbumsDTO>> GetListDTO(int enterpriseid, int pageIndex, int pageSize)
        {
                string sql = string.Format(@"
select  a.AlbumId,a.Name,a.PhotoCount,b.Path from TB_Enterprise_Album a
left join  Photo b on a.CoverId=b.PhotoId 
where a.EnterpriseId={0} 
order by a.CreateTime desc, a.AlbumId desc
", enterpriseid);
                var list = await DapperHelper.SqlQuery22Async < EtpAlbumsDTO>(sql);
                return list.ToList();
        }

    }
    public interface ICompanyAlbumRepo : IRepository<TB_Enterprise_Album>
    {
        Task<IList<CompanyAlbumResult>> GetEnterpriseAlbumAsync(int enterpriseid, int? count = null);

        IList<CompanyAlbumResult> GetEnterpriseAlbum(int enterpriseid, int? count = null);

        IList<TB_Enterprise_Album> GetList(int enterpriseid, int? count = null);

        IList<TB_Enterprise_Album> GetList(int enterpriseid, int pageIndex, int pageSize, out int totalCnt);

        Task<IList<EtpAlbumsDTO>> GetListDTO(int enterpriseid, int pageIndex, int pageSize);
        string GetAlbumName(int albumId);
        int GetOrCreateAlbumIdByName(int enterpriseId, string albumName, bool autoCreate = false);

        Task<bool> HasAlbumAsync(int enterpriseid);
    }

}
