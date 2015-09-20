using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topuc22Top.Data.Infrastructure;
using Topuc22Top.Data.ResultModel;
using Topuc22Top.Model.Entities;
using System.Data.Entity;
using LinqKit;
using Topuc22Top.Common;

namespace Topuc22Top.Data.Repositories
{
    public class InterestPushRepo : RepositoryBase<TB_Position_Element>, IInterestPushRepo
    {
        public InterestPushRepo(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }

        #region 基础质量的查询
        private IQueryable<TB_Enterprise> BaseJobRecommendCompanys()
        {

            return BaseJobRecommendCompanys(null);
        }
        private IQueryable<TB_Enterprise> BaseJobRecommendCompanys(int? location)
        {
            DateTime lastmonth = DateTime.Today.AddMonths(-1);

            int countlimit = 1;
            IQueryable<TB_Enterprise> etplist;
            if (location.HasValue)
            {
                if (location.Value == 1001 || location.Value == 1003)
                {
                    countlimit = 5;
                }
                if (location.Value == 2016001 || location.Value == 2016003)
                {
                    countlimit = 4;
                }
                if (location.Value == 2014001 || location.Value == 2008001 || location.Value == 2019001 || location.Value == 2007001)
                {
                    countlimit = 3;
                }
                if (location.Value == 2007005 || location.Value == 1004 || location.Value == 1002 || location.Value == 2004001 || location.Value == 2015001 || location.Value == 2016017 || location.Value == 2023001)
                {
                    countlimit = 2;
                }
            }

            etplist = from e in DataContext.TB_Enterprise
                      join p in DataContext.TB_Position_Element on e.EnterpriseId equals p.EnterpriseId into pg
                      where e.CustomerLevel != (int)CustomerLevel.InternalTest && e.Status != (int)EtpStatus.Shadow
                      && pg.Where(a => a.DeployTime >= lastmonth && a.PositionStatus == (int)PositionStatus.Publish && a.Deadline >= DateTime.Today).Count() >= countlimit
                      select e;

            return etplist;

        }

        private IQueryable<TB_Position_Element> BaseRecommendJob()
        {
            DateTime lasttime = DateTime.Today.AddDays(-40);

            var etplist = BaseJobRecommendCompanys().Take(100);

            //var etplist = BaseRecommendCompanys();
            var poslist = from p in DataContext.TB_Position_Element
                          join e in etplist on p.EnterpriseId equals e.EnterpriseId
                          join d in DataContext.TB_Position_BigTxt on p.PositionId equals d.PositionId
                          where
                              //d.PosDescription.Replace(" ", "").Length > 100 &&
                          p.PositionStatus == 1 && p.Deadline > DateTime.Today
                          select p;

            return poslist;


        }
        #endregion

        #region 基础的推送IQueryable

        private IQueryable<EnterpriseResult> GetRecommendCompanysQuery(string city, string industry, int bantch = 10)
        {
            var list = BaseJobRecommendCompanys().Where(e => e.Status != (int)EtpStatus.Shadow);


            if (!string.IsNullOrEmpty(city))
            {
                var orcondition = PredicateBuilder.False<TB_Enterprise>();
                foreach (var cid in city.Split(','))
                {

                    orcondition = orcondition.Or(a => ("," + a.City + ",").Contains("," + cid + ","));
                }
                list = list.Where(orcondition.Expand());

            }
            if (!string.IsNullOrEmpty(industry))
            {
                var orcondition = PredicateBuilder.False<TB_Enterprise>();
                foreach (var indu in industry.Split(','))
                {
                    orcondition = orcondition.Or(a => ("," + a.Industry + ",").Contains("," + indu + ","));
                }
                list = list.Where(orcondition.Expand());
            }

            var resultlist = list.Select(e => new EnterpriseResult
            {
                EnterpriseId = e.EnterpriseId,
                Name = e.Name,
                CityId = e.City,
                IndustryId = e.Industry,
                ModeId = e.Mode.HasValue ? e.Mode.Value : 0,
                IsFamous = e.IsFamous
            }).Take(bantch);
            return resultlist;
        }

        private IQueryable<SimpleJobResult> GetSimilarJobsQuery(int? positiontype, string func, int? city, string exceptpids, int bantch, int exceptetpid = 0)
        {
            DateTime lasttime = DateTime.Today.AddDays(-40);

            List<int> exceptpidlist = new List<int>();
            if (!string.IsNullOrEmpty(exceptpids))
            {
                exceptpidlist = GetIntList(exceptpids).Distinct().ToList();
            }

            //IQueryable<EnterpriseResult> etplist = BaseRecommendCompanys().Take(200);

            //if (city.HasValue)
            //{
            //    etplist = BaseRecommendCompanys(city.Value).Take(200);
            //}



            var baselist = from p in DataContext.TB_Position_Element
                           //join e in DataContext.TB_Enterprise on p.EnterpriseId equals e.EnterpriseId
                           where p.DeployTime > lasttime && p.PositionStatus == 1 && p.Deadline > DateTime.Today
                           && p.EnterpriseId != 0 && p.EnterpriseId != exceptetpid
                           //orderby e.IsFamous descending
                           select p;

            if (exceptpidlist.Count > 0)
            {

                baselist = baselist.Where(a => !exceptpidlist.Contains(a.PositionId));
            }


            if (positiontype.HasValue)
            {
                if (positiontype.Value == 1)
                {
                    baselist = baselist.Where(a => a.PositionType == 1);
                }
                else
                {
                    baselist = baselist.Where(a => a.PositionType != 1);
                }
            }
            else
            {
                baselist = baselist.Where(a => a.PositionType == 1);
            }

            //IQueryable<TB_Position_Element> resultlist = baselist.Take(0);



            if (city.HasValue)
            {
                baselist = baselist.Where(a => a.CityId == city.Value);
            }




            if (!string.IsNullOrEmpty(func))
            {
                var orcondition = PredicateBuilder.False<TB_Position_Element>();
                foreach (var f in func.Split(','))
                {
                    orcondition = orcondition.Or(a => ("," + a.FunctionIds + ",").Contains("," + f + ","));

                }
                baselist = baselist.Where(orcondition.Expand());

            }
            else
            {

                //return (new  List<SimpleJobResult>).to;
            }

            var simpleposlist = (from p in baselist
                                 join e in DataContext.TB_Enterprise on p.EnterpriseId equals e.EnterpriseId
                                 //join d in context.TB_Position_BigTxt on p.Pos.PositionId equals d.PositionId
                                 //where d.PosDescription.Replace(" ", "").Length > 100
                                 orderby e.IsFamous descending
                                 select new SimpleJobResult
                                 {
                                     PositionId = p.PositionId,
                                     PositionType = p.PositionType,
                                     CityId = p.CityId,
                                     DeployTime = p.CreateDate,
                                     EnterpriseId = p.EnterpriseId,
                                     EtpName = e.Name,
                                     Position = p.Position,
                                     SalaryMax = p.SalaryMax,
                                     SalaryMin = p.SalaryMin,
                                     Function = p.FunctionIds,
                                     Industry = e.Industry,
                                     Tags = p.Tags
                                 }).Take(bantch);





            return simpleposlist;
        }

        #endregion



        public List<EnterpriseResult> GetRecommendCompanys(string city, string industry, int bantch = 10)
        {
            //IQueryable<EnterpriseResult> recommend;
            //初始化
            var resultlist = GetRecommendCompanysQuery(city, industry, bantch);

            return resultlist.ToList();

        }

        public async Task<List<EnterpriseResult>> GetRecommendCompanysAsync(string city, string industry, int? exceptetpid, int bantch = 10)
        {
            //IQueryable<EnterpriseResult> recommend;
            //初始化
            using (TopucDB context = new TopucDB())
            {
                int countlimit = 1;

                if (!string.IsNullOrEmpty(city))
                {
                    if (city == "1001" || city == "1003")
                    {
                        countlimit = 5;
                    }
                    if (city == "2016001" || city == "2016003")
                    {
                        countlimit = 4;
                    }
                    if (city == "2014001" || city == "2008001" || city == "2019001" || city == "2007001")
                    {
                        countlimit = 3;
                    }
                    if (city == "2007005" || city == "1004" || city == "1002" || city == "2004001" || city == "2015001" || city == "2016017" || city == "2023001")
                    {
                        countlimit = 2;
                    }
                }


                var lastmonth = DateTime.Now.AddMonths(-1);

                var etplist = from e in context.TB_Enterprise
                              join p in context.TB_Position_Element on e.EnterpriseId equals p.EnterpriseId into pg
                              where e.CustomerLevel != (int)CustomerLevel.InternalTest && e.Status != (int)EtpStatus.Shadow
                              && pg.Where(a => a.DeployTime >= lastmonth && a.PositionStatus == 1 && a.Deadline >= DateTime.Today).Count() >= countlimit
                              select e;


                if (exceptetpid.HasValue)
                {
                    etplist = etplist.Where(a => a.EnterpriseId != exceptetpid.Value);
                }

                if (!string.IsNullOrEmpty(city))
                {
                    var orcondition = PredicateBuilder.False<TB_Enterprise>();
                    foreach (var cid in city.Split(','))
                    {

                        orcondition = orcondition.Or(a => ("," + a.City + ",").Contains("," + cid + ","));
                    }
                    etplist = etplist.Where(orcondition.Expand());

                }
                if (!string.IsNullOrEmpty(industry))
                {
                    var orcondition = PredicateBuilder.False<TB_Enterprise>();
                    foreach (var indu in industry.Split(','))
                    {
                        orcondition = orcondition.Or(a => ("," + a.Industry + ",").Contains("," + indu + ","));
                    }
                    etplist = etplist.Where(orcondition.Expand());
                }

                var resultlist = etplist.Select(e => new EnterpriseResult
                {
                    EnterpriseId = e.EnterpriseId,
                    Name = e.Name,
                    CityId = e.City,
                    IndustryId = e.Industry,
                    ModeId = e.Mode.HasValue ? e.Mode.Value : 0,
                    IsFamous = e.IsFamous
                }).Take(bantch);
                return await resultlist.ToListAsync();

            }
            //var resultlist = GetRecommendCompanysQuery(city, industry, bantch, exceptetpid);

            //return await resultlist.ToListAsync();

        }

        public async Task<List<EnterpriseResult>> GetRecommendCompanysAsync(string city, string industry, int bantch = 100)
        {
            //IQueryable<EnterpriseResult> recommend;
            //初始化
            using (TopucDB context = new TopucDB())
            {
                
                var lastmonth = DateTime.Now.AddMonths(-1);

                var etplist = from e in context.TB_Enterprise
                              join p in context.TB_Position_Element on e.EnterpriseId equals p.EnterpriseId into pg
                              where e.CustomerLevel != (int)CustomerLevel.InternalTest && e.Status != (int)EtpStatus.Shadow
                              && pg.Where(a => a.DeployTime >= lastmonth && a.PositionStatus == 1 && a.Deadline >= DateTime.Today).Count() >= 1
                              select e;


                if (!string.IsNullOrEmpty(city))
                {
                    var orcondition = PredicateBuilder.False<TB_Enterprise>();
                    foreach (var cid in city.Split(','))
                    {

                        orcondition = orcondition.Or(a => ("," + a.City + ",").Contains("," + cid + ","));
                    }
                    etplist = etplist.Where(orcondition.Expand());

                }
                if (!string.IsNullOrEmpty(industry))
                {
                    var orcondition = PredicateBuilder.False<TB_Enterprise>();
                    foreach (var indu in industry.Split(','))
                    {
                        orcondition = orcondition.Or(a => ("," + a.Industry + ",").Contains("," + indu + ","));
                    }
                    etplist = etplist.Where(orcondition.Expand());
                }

                var resultlist = etplist.Select(e => new EnterpriseResult
                {
                    EnterpriseId = e.EnterpriseId,
                    Name = e.Name,
                    CityId = e.City,
                    IndustryId = e.Industry,
                    ModeId = e.Mode.HasValue ? e.Mode.Value : 0,
                    IsFamous = e.IsFamous
                }).Take(bantch);
                return await resultlist.ToListAsync();

            }
            //var resultlist = GetRecommendCompanysQuery(city, industry, bantch, exceptetpid);

            //return await resultlist.ToListAsync();

        }


        public List<SimpleJobResult> GetSimilarJobs(int? positiontype, string func, int? city, string exceptpids, int bantch = 10, int exceptetpid = 0)
        {

            return GetSimilarJobsQuery(positiontype, func, city, exceptpids, bantch).ToList();
        }

        public async Task<List<SimpleJobResult>> GetSimilarJobsAsync(int? positiontype, string func, int? city, string exceptpids, int bantch = 10, int exceptetpid = 0)
        {
            using (TopucDB context = new TopucDB())
            {

                DateTime lasttime = DateTime.Today.AddDays(-40);

                List<int> exceptpidlist = new List<int>();
                if (!string.IsNullOrEmpty(exceptpids))
                {
                    exceptpidlist = GetIntList(exceptpids).Distinct().ToList();
                }


                var baselist = from p in context.TB_Position_Element
                               //join e in DataContext.TB_Enterprise on p.EnterpriseId equals e.EnterpriseId
                               where p.DeployTime > lasttime && p.PositionStatus == 1 && p.Deadline > DateTime.Today
                               && p.EnterpriseId != 0 && p.EnterpriseId != exceptetpid
                               //orderby e.IsFamous descending
                               select p;

                if (exceptpidlist.Count > 0)
                {

                    baselist = baselist.Where(a => !exceptpidlist.Contains(a.PositionId));
                }


                if (positiontype.HasValue)
                {
                    if (positiontype.Value == 1)
                    {
                        baselist = baselist.Where(a => a.PositionType == 1);
                    }
                    else
                    {
                        baselist = baselist.Where(a => a.PositionType != 1);
                    }
                }
                else
                {
                    baselist = baselist.Where(a => a.PositionType == 1);
                }



                if (city.HasValue)
                {
                    baselist = baselist.Where(a => a.CityId == city.Value);
                }




                if (!string.IsNullOrEmpty(func))
                {
                    var orcondition = PredicateBuilder.False<TB_Position_Element>();
                    foreach (var f in func.Split(','))
                    {
                        orcondition = orcondition.Or(a => ("," + a.FunctionIds + ",").Contains("," + f + ","));

                    }
                    baselist = baselist.Where(orcondition.Expand());

                }
                else
                {

                    //return (new  List<SimpleJobResult>).to;
                }

                var simpleposlist = (from p in baselist
                                     join e in context.TB_Enterprise on p.EnterpriseId equals e.EnterpriseId
                                     //join d in context.TB_Position_BigTxt on p.Pos.PositionId equals d.PositionId
                                     //where d.PosDescription.Replace(" ", "").Length > 100
                                     orderby e.IsFamous descending
                                     select new SimpleJobResult
                                     {
                                         PositionId = p.PositionId,
                                         PositionType = p.PositionType,
                                         CityId = p.CityId,
                                         DeployTime = p.CreateDate,
                                         EnterpriseId = p.EnterpriseId,
                                         EtpName = e.Name,
                                         Position = p.Position,
                                         SalaryMax = p.SalaryMax,
                                         SalaryMin = p.SalaryMin,
                                         Function = p.FunctionIds,
                                         Industry = e.Industry,
                                         Tags = p.Tags
                                     }).Take(bantch);

                return await simpleposlist.ToListAsync();
            }
            //return await GetSimilarJobsQuery(positiontype, func, city, exceptpids, bantch).ToListAsync();
        }

        public async Task<List<SimpleJobResult>> GetSimilarJobsAsync(int? positiontype, string func, int? city, int bantch = 100)
        {
            using (TopucDB context = new TopucDB())
            {

                DateTime lasttime = DateTime.Today.AddDays(-40);




                var baselist = from p in context.TB_Position_Element
                               //join e in DataContext.TB_Enterprise on p.EnterpriseId equals e.EnterpriseId
                               where p.DeployTime > lasttime && p.PositionStatus == 1 && p.Deadline > DateTime.Today
                               && p.EnterpriseId != 0
                               //orderby e.IsFamous descending
                               select p;


                if (positiontype.HasValue)
                {
                    if (positiontype.Value == 1)
                    {
                        baselist = baselist.Where(a => a.PositionType == 1);
                    }
                    else
                    {
                        baselist = baselist.Where(a => a.PositionType != 1);
                    }
                }
                else
                {
                    baselist = baselist.Where(a => a.PositionType == 1);
                }



                if (city.HasValue)
                {
                    baselist = baselist.Where(a => a.CityId == city.Value);
                }




                if (!string.IsNullOrEmpty(func))
                {
                    var orcondition = PredicateBuilder.False<TB_Position_Element>();
                    foreach (var f in func.Split(','))
                    {
                        orcondition = orcondition.Or(a => ("," + a.FunctionIds + ",").Contains("," + f + ","));

                    }
                    baselist = baselist.Where(orcondition.Expand());

                }
                else
                {

                    //return (new  List<SimpleJobResult>).to;
                }

                var simpleposlist = (from p in baselist
                                     join e in context.TB_Enterprise on p.EnterpriseId equals e.EnterpriseId
                                     //join d in context.TB_Position_BigTxt on p.Pos.PositionId equals d.PositionId
                                     //where d.PosDescription.Replace(" ", "").Length > 100
                                     orderby e.IsFamous descending
                                     select new SimpleJobResult
                                     {
                                         PositionId = p.PositionId,
                                         PositionType = p.PositionType,
                                         CityId = p.CityId,
                                         DeployTime = p.CreateDate,
                                         EnterpriseId = p.EnterpriseId,
                                         EtpName = e.Name,
                                         Position = p.Position,
                                         SalaryMax = p.SalaryMax,
                                         SalaryMin = p.SalaryMin,
                                         Function = p.FunctionIds,
                                         Industry = e.Industry,
                                         Tags = p.Tags
                                     }).Take(bantch);

                return await simpleposlist.ToListAsync();
            }
            //return await GetSimilarJobsQuery(positiontype, func, city, exceptpids, bantch).ToListAsync();
        }



        public List<SimpleJobResult> GetObjectiveJobs(int userid, int count)
        {
            var resultlist = new List<SimpleJobResult>();

            var stuobject = DataContext.TB_Stu_Objective.Where(a => a.UserId == userid).FirstOrDefault();
            if (stuobject != null)
            {
                DateTime lasttime = DateTime.Today.AddDays(-40);

                var baseposlist = from p in DataContext.TB_Position_Element
                                  //join e in DataContext.TB_Enterprise on p.EnterpriseId equals e.EnterpriseId
                                  where
                                      p.DeployTime > lasttime &&
                                  p.PositionStatus == 1
                                  && p.Deadline > DateTime.Today
                                  select p;
                var baseetplist = from e in DataContext.TB_Enterprise
                                  where e.Status != (int)EtpStatus.Shadow
                                  select e;



                if (stuobject.ObjectiveType.HasValue && stuobject.ObjectiveType.Value == (int)ObjectiveType.FullTime)
                {
                    baseposlist = baseposlist.Where(a => a.PositionType == 1);



                    if (!string.IsNullOrEmpty(stuobject.ObjectLocation))
                    {
                        var orcondition = PredicateBuilder.False<TB_Position_Element>();
                        foreach (var c in stuobject.ObjectLocation.Trim(',').Split(','))
                        {
                            int cityint = int.Parse(c);
                            orcondition = orcondition.Or(a => a.CityId == cityint);

                        }
                        baseposlist = baseposlist.Where(orcondition.Expand());
                    }



                    if (!string.IsNullOrEmpty(stuobject.ObjectFunction))
                    {
                        var orcondition = PredicateBuilder.False<TB_Position_Element>();
                        foreach (var f in stuobject.ObjectFunction.Trim(',').Split(','))
                        {
                            orcondition = orcondition.Or(a => ("," + a.FunctionIds + ",").Contains("," + f + ","));

                        }
                        baseposlist = baseposlist.Where(orcondition.Expand());
                    }




                    if (!string.IsNullOrEmpty(stuobject.ObjectIndustry))
                    {
                        var orcondition = PredicateBuilder.False<TB_Enterprise>();
                        foreach (var indu in stuobject.ObjectIndustry.Trim(',').Split(','))
                        {
                            orcondition = orcondition.Or(a => ("," + a.Industry + ",").Contains("," + indu + ","));

                        }
                        baseetplist = baseetplist.Where(orcondition.Expand());
                    }

                    var plist = baseposlist.ToList();

                    var elist = baseetplist.ToList();

                    var result = from p in baseposlist
                                 join e in baseetplist on p.EnterpriseId equals e.EnterpriseId
                                 orderby p.DeployTime descending
                                 select new SimpleJobResult
                                 {
                                     PositionId = p.PositionId,
                                     PositionType = p.PositionType,
                                     CityId = p.CityId,
                                     DeployTime = p.CreateDate,
                                     EnterpriseId = p.EnterpriseId,
                                     EtpName = e.Name,
                                     Position = p.Position,
                                     SalaryMax = p.SalaryMax,
                                     SalaryMin = p.SalaryMin,
                                     Function = p.FunctionIds,
                                     Industry = e.Industry,
                                     Tags = p.Tags
                                 };

                    var resultlist1 = result.ToList();

                    var resultlist2 = result.Take(count).ToList();

                    resultlist = result.Take(count).ToList();



                }
                else if (stuobject.ObjectiveType.HasValue && stuobject.ObjectiveType.Value == (int)ObjectiveType.Internship)
                {
                    baseposlist = baseposlist.Where(a => a.PositionType != 1);

                    if (!string.IsNullOrEmpty(stuobject.InternShipLocation))
                    {
                        var orcondition = PredicateBuilder.False<TB_Position_Element>();
                        foreach (var c in stuobject.InternShipLocation.Trim(',').Split(','))
                        {
                            int cityint = int.Parse(c);
                            orcondition = orcondition.Or(a => a.CityId == cityint);

                        }
                        baseposlist = baseposlist.Where(orcondition.Expand());
                    }

                    if (!string.IsNullOrEmpty(stuobject.InternShipFunction))
                    {
                        var orcondition = PredicateBuilder.False<TB_Position_Element>();
                        foreach (var f in stuobject.InternShipFunction.Trim(',').Split(','))
                        {
                            orcondition = orcondition.Or(a => ("," + a.FunctionIds + ",").Contains("," + f + ","));

                        }
                        baseposlist = baseposlist.Where(orcondition.Expand());
                    }

                    if (!string.IsNullOrEmpty(stuobject.InternShipIndustry))
                    {
                        var orcondition = PredicateBuilder.False<TB_Enterprise>();
                        foreach (var indu in stuobject.InternShipIndustry.Trim(',').Split(','))
                        {
                            orcondition = orcondition.Or(a => ("," + a.Industry + ",").Contains("," + indu + ","));

                        }
                        baseetplist = baseetplist.Where(orcondition.Expand());
                    }

                    var result = from p in baseposlist
                                 join e in baseetplist on p.EnterpriseId equals e.EnterpriseId
                                 orderby p.DeployTime descending
                                 select new SimpleJobResult
                                   {
                                       PositionId = p.PositionId,
                                       PositionType = p.PositionType,
                                       CityId = p.CityId,
                                       DeployTime = p.CreateDate,
                                       EnterpriseId = p.EnterpriseId,
                                       EtpName = e.Name,
                                       Position = p.Position,
                                       SalaryMax = p.SalaryMax,
                                       SalaryMin = p.SalaryMin,
                                       Function = p.FunctionIds,
                                       Industry = e.Industry,
                                       Tags = p.Tags
                                   };
                    resultlist = result.Take(count).ToList();

                }
                else
                {

                    var fulltimeposlist = baseposlist.Where(a => a.PositionType == 1);

                    if (!string.IsNullOrEmpty(stuobject.ObjectLocation))
                    {
                        var orcondition = PredicateBuilder.False<TB_Position_Element>();
                        foreach (var c in stuobject.ObjectLocation.Trim(',').Split(','))
                        {
                            int cityint = int.Parse(c);
                            orcondition = orcondition.Or(a => a.CityId == cityint);

                        }
                        fulltimeposlist = fulltimeposlist.Where(orcondition.Expand());
                    }

                    if (!string.IsNullOrEmpty(stuobject.ObjectFunction))
                    {
                        var orcondition = PredicateBuilder.False<TB_Position_Element>();
                        foreach (var f in stuobject.ObjectFunction.Trim(',').Split(','))
                        {
                            orcondition = orcondition.Or(a => ("," + a.FunctionIds + ",").Contains("," + f + ","));

                        }
                        fulltimeposlist = fulltimeposlist.Where(orcondition.Expand());
                    }

                    if (!string.IsNullOrEmpty(stuobject.ObjectIndustry))
                    {
                        var orcondition = PredicateBuilder.False<TB_Enterprise>();
                        foreach (var indu in stuobject.ObjectIndustry.Trim(',').Split(','))
                        {
                            orcondition = orcondition.Or(a => ("," + a.Industry + ",").Contains("," + indu + ","));

                        }
                        baseetplist = baseetplist.Where(orcondition.Expand());
                    }



                    var internposlist = baseposlist.Where(a => a.PositionType != 1);

                    if (!string.IsNullOrEmpty(stuobject.InternShipLocation))
                    {
                        var orcondition = PredicateBuilder.False<TB_Position_Element>();
                        foreach (var c in stuobject.InternShipLocation.Trim(',').Split(','))
                        {
                            int cityint = int.Parse(c);
                            orcondition = orcondition.Or(a => a.CityId == cityint);

                        }
                        internposlist = internposlist.Where(orcondition.Expand());
                    }

                    if (!string.IsNullOrEmpty(stuobject.InternShipFunction))
                    {
                        var orcondition = PredicateBuilder.False<TB_Position_Element>();
                        foreach (var f in stuobject.InternShipFunction.Trim(',').Split(','))
                        {
                            orcondition = orcondition.Or(a => ("," + a.FunctionIds + ",").Contains("," + f + ","));

                        }
                        internposlist = internposlist.Where(orcondition.Expand());
                    }

                    if (!string.IsNullOrEmpty(stuobject.InternShipIndustry))
                    {
                        var orcondition = PredicateBuilder.False<TB_Enterprise>();
                        foreach (var indu in stuobject.InternShipIndustry.Trim(',').Split(','))
                        {
                            orcondition = orcondition.Or(a => ("," + a.Industry + ",").Contains("," + indu + ","));

                        }
                        baseetplist = baseetplist.Where(orcondition.Expand());
                    }

                    var resultposlist = fulltimeposlist.Union(internposlist);

                    var result = from p in resultposlist
                                 join e in baseetplist on p.EnterpriseId equals e.EnterpriseId
                                 orderby p.DeployTime descending
                                 select new SimpleJobResult
                                 {
                                     PositionId = p.PositionId,
                                     PositionType = p.PositionType,
                                     CityId = p.CityId,
                                     DeployTime = p.CreateDate,
                                     EnterpriseId = p.EnterpriseId,
                                     EtpName = e.Name,
                                     Position = p.Position,
                                     SalaryMax = p.SalaryMax,
                                     SalaryMin = p.SalaryMin,
                                     Function = p.FunctionIds,
                                     Industry = e.Industry,
                                     Tags = p.Tags
                                 };
                    resultlist = result.Take(count).ToList();
                }

            }
            return resultlist;



        }

        private int[] GetIntList(string idStr)
        {
            if (idStr.Last() == ',')
                idStr = idStr.Substring(0, idStr.LastIndexOf(','));
            string[] list = idStr.Split(',');
            int len = list.Length;
            int[] idList = new int[len];
            for (int i = 0; i < len; i++)
            {
                idList[i] = Int32.Parse(list[i]);
            }
            return idList;

        }

    }

    public interface IInterestPushRepo : IRepository<TB_Position_Element>
    {

        List<EnterpriseResult> GetRecommendCompanys(string city, string industry, int bantch = 10);

        Task<List<EnterpriseResult>> GetRecommendCompanysAsync(string city, string industry, int? exceptetpid, int bantch = 10);

        Task<List<EnterpriseResult>> GetRecommendCompanysAsync(string city, string industry, int bantch = 100);

        /// <summary>
        /// 获取类似职位
        /// </summary>
        /// <param name="positiontype"></param>
        /// <param name="func"></param>
        /// <param name="city"></param>
        /// <param name="exceptetpid"></param>
        /// <param name="bantch"></param>
        /// <returns></returns>
        List<SimpleJobResult> GetSimilarJobs(int? positiontype, string func, int? city, string exceptpids, int bantch = 10, int exceptetpid = 0);

        /// <summary>
        /// 呵呵~又关掉一个功能，原来职位详情页推送相似职位用的
        /// </summary>
        /// <param name="positiontype"></param>
        /// <param name="func"></param>
        /// <param name="city"></param>
        /// <param name="exceptpids"></param>
        /// <param name="bantch"></param>
        /// <param name="exceptetpid"></param>
        /// <returns></returns>
        Task<List<SimpleJobResult>> GetSimilarJobsAsync(int? positiontype, string func, int? city, string exceptpids, int bantch = 10, int exceptetpid = 0);

        Task<List<SimpleJobResult>> GetSimilarJobsAsync(int? positiontype, string func, int? city, int bantch = 100);




        /// <summary>
        /// 准备废弃该方法 改成用solr搜索
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        List<SimpleJobResult> GetObjectiveJobs(int userid, int count);
    }
}
