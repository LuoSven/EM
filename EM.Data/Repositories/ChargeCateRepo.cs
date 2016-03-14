using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EM.Data.Infrastructure;
using EM.Model.Entities;
using EM.Model.VMs;
using EM.Utils;
using EM.Common;
using EM.Model.DTOs;
using EM.Data.Dapper;
using EM.Model.SMs;

namespace EM.Data.Repositories
{
    public class ChargeCateRepo : RepositoryBase<EM_Charge_Cate>, IChargeCateRepo
    {
        public ChargeCateRepo(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }

        public List<KeyValueVM> GetList(int RoleTypeId,CateDropType CateListType)
        {
            //仅仅是函数不一样

            
            var sql = "select * from dbo.{0}(@RoleTypeId)";
            switch(CateListType)
            {
                 
                case CateDropType.Form:
                case CateDropType.Search:
                sql = string.Format(sql, "FC_GetRoleChildrenCateIds");//包含子项
                break;
                case CateDropType.Report:
                sql = string.Format(sql, "FC_GetRoleParentCateIds");//包含父类
                break;
            }
            //生成分类ID
            var CateList = DapperHelper.SqlQuery<int>(sql, new { RoleTypeId = RoleTypeId }).ToList();

            //根据Id转化为实体
            var result = DataContext.EM_Charge_Cate.Where(o => CateList.Contains(o.Id)).Select(o => new KeyValueVM()
            {
                Value = o.CateName,
                Key = o.Id.ToString()
            }).ToList();
            return result;
        }

        public List<KeyValueVM> GetGroupList(int RoleTypeId, CateDropType CateListType)
        {
            //仅仅是函数不一样


            var sql = "select * from dbo.{0}(@RoleTypeId)";
            switch (CateListType)
            {

                case CateDropType.Form:
                case CateDropType.Search:
                    sql = string.Format(sql, "FC_GetRoleChildrenCateIds");//包含子项
                    break;
                case CateDropType.Report:
                    sql = string.Format(sql, "FC_GetRoleParentCateIds");//包含父类
                    break;
            }
            //生成分类ID
            var CateList = DapperHelper.SqlQuery<int>(sql, new { RoleTypeId = RoleTypeId }).ToList();

            //根据Id转化为实体
            var query = from cate in DataContext.EM_Charge_Cate
                        join pcate in DataContext.EM_Charge_Cate on cate.ParentId equals pcate.Id
                        where CateList.Contains(cate.Id)
                        select new
                        {
                            Value = cate.CateName,
                            Key = cate.Id.ToString(),
                            ParentId = pcate.Id,
                            ParentName = pcate.CateName
                        };

            var groupList = from cate in query
                            group cate by new { cate.ParentId, cate.ParentName }
                                into gCate
                                select new 
                                {
                                    Key = gCate.Key.ParentName,
                                    Value = gCate.Key.ParentId.ToString(),
                                    Items = gCate.Select(o => new KeyValueVM {
                                        Key = o.Key,
                                        Value = o.Value,
                                    }).ToList(),
                                };
            return groupList.ToList().Select(o => new KeyValueVM() { Key=o.Key,Value=o.Value,Items=o.Items}).ToList();
        }



        public string GetCateName(int id)
        {
            var name = DapperHelper.SqlQuery<string>("select CateName from  EM_Charge_Cate where Id=@Id ", new { Id = id }).FirstOrDefault();
            return name ?? "";
        }

    }


    public interface IChargeCateRepo : IRepository<EM_Charge_Cate>
    {
        /// <summary>
        /// 获取分类信息（Key是Id,Value是Name）
        /// </summary>
        /// <param name="RoleTypeId"></param>
        /// <param name="CateListType"></param>
        /// <returns></returns>
        List<KeyValueVM> GetList(int RoleTypeId, CateDropType CateListType);

        List<KeyValueVM> GetGroupList(int RoleTypeId, CateDropType CateListType);

        string GetCateName(int id);
    }
}
