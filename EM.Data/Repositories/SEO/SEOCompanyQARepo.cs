using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topuc22Top.Data.Infrastructure;
using Topuc22Top.Model.Entities;


namespace Topuc22Top.Data.Repositories
{
    public class SEOCompanyQARepo : RepositoryBase<SEO_CompanyQA>, ISEOCompanyQARepo
    {
        public SEOCompanyQARepo(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }

        public List<SEO_CompanyQA> GetSeoCompanyQAList(int id, int page, int pagesize, out int totalCnt)
        {
            var list = from p in DataContext.SEO_CompanyQA
                       where p.EtpId == id
                       && p.Status == 1
                       && p.LastUpdatedDate != null
                       select p;
            totalCnt = list.Count();
            //var pageQAList = list.OrderByDescending(a => a.CreateDate).Skip((page - 1) * pagesize).Take(pagesize).ToList();
            var pageQAList = list.ToList().OrderByDescending(p => p.Question, new QAComparer()).Skip((page - 1) * pagesize).Take(pagesize).ToList();
            return pageQAList;
        }

        public SEO_CompanyQA GetById(int etpId, int qaId, bool isAddVCount = false)
        {
            var entity = (from p in DataContext.SEO_CompanyQA
                          where p.EtpId == etpId && p.Id == qaId
                          && p.Status == 1
                          select p).FirstOrDefault();
            if (entity != null && isAddVCount)
            {
                entity.VCount = (entity.VCount ?? 0) + 1;
                DataContext.SaveChanges();
            }
            return entity;
        }

        public List<SEO_CompanyQA> GetHighVCountSeoCompanyQAList(int etpId, int count)
        {
            var list = from p in DataContext.SEO_CompanyQA
                       where p.EtpId == etpId
                       && p.Status == 1
                       && p.LastUpdatedDate != null
                       orderby p.VCount descending
                       select p;
            return list.ToList().OrderByDescending(p => p.Question, new QAComparer()).Take(count).ToList();
        }
        /// <summary>
        /// 获取问答ID和公司ID，以逗号分隔
        /// </summary>
        /// <returns></returns>
        public IList<string> GetEffectionCompanyQA()
        {
            var exceptArr = new int[] { 31,99};
            var query = from m in this.DataContext.SEO_CompanyQA
                        join c in this.DataContext.TB_Enterprise on m.EtpId equals c.EnterpriseId
                        where
                        c.CustomerLevel != 99 &&
                        c.CustomerLevel != null &&
                        !exceptArr.Contains(c.Status)
                        select m.EtpId + "," + m.Id;

            return query.ToList<string>();
        }

        private class QAComparer : IComparer<string>
        {
            public int Compare(string x, string y)
            {
                return GetKeywordCount(x) - GetKeywordCount(y);
            }

            private int GetKeywordCount(string str)
            {
                var keyList= new List<string>(){"招聘","求职","工作","应届生","大学生","薪酬","待遇","工资"};
                var count = 0;
                foreach (var key in keyList)
                {
                    count += (str.Contains(key) ? 1 : 0);
                }
                return count;
            }
        }

    }
    
    public interface ISEOCompanyQARepo : IRepository<SEO_CompanyQA>
    {
        List<SEO_CompanyQA> GetSeoCompanyQAList(int id, int page, int pagesize, out int totalCnt);

        SEO_CompanyQA GetById(int etpId, int qaId, bool isAddVCount = false);

        List<SEO_CompanyQA> GetHighVCountSeoCompanyQAList(int etpId, int count);
        /// <summary>
        /// 获取问答ID和公司ID，以逗号分隔
        /// </summary>
        /// <returns></returns>
        IList<string> GetEffectionCompanyQA();
    }

}
