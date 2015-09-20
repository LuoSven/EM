using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topuc.Framework.Cache;
using Topuc22Top.Data.Infrastructure;
using Topuc22Top.Model.Entities;

namespace Topuc22Top.Data.Repositories
{
    public class AssessmentRepo : RepositoryBase<Assessment>, IAssessmentRepo
    {
        private readonly ICache cache;
        public AssessmentRepo(IDatabaseFactory databaseFactory, ICache cache)
            : base(databaseFactory)
        {
            this.cache = cache;
        }

        public IList<Assessment_Question> GetQuestions(int assessmentId)
        {
            return cache.Get(Settings.Key_AssessmentQuestions + assessmentId.ToString(), () =>
            {
                var query = from q in DataContext.Assessment_Question
                            where q.AssessmentId == assessmentId
                            select q;
                return query.OrderBy(q => q.Sequence).ToList();
            });
        }


        public IList<Assessment_Question> GetTopQuestionByResult(int id)
        {
            //var assessmentResult = DataContext.Assessment_Result.Find(id);
            //int[] arr = Array.ConvertAll<string, int>(assessmentResult.Adjust.Split(','), delegate(string s) { int resultint = 0; int.TryParse(s, out  resultint); return resultint; });
            //var query = from q in DataContext.Assessment_Question
            //            where arr.Contains(q.QuestionId)
            //            select q;
            //return query.OrderBy(q => q.Sequence).ToList();
            //20150413改为
            var assessmentResult = DataContext.Assessment_Result.Find(id);
            int[] arr = Array.ConvertAll<string, int>(assessmentResult.Adjust.Split(','), delegate(string s) { int resultint = 0; int.TryParse(s, out  resultint); return resultint; });
            var query = from q in DataContext.Assessment_Question
                        where arr.Contains(q.QuestionId)
                        orderby q.Sequence descending
                        select q;
            return query.OrderBy(q => q.Sequence).ToList();
        }

        public long ChangeTestedCount(int assessmentId)
        {
            var entity = (from a in DataContext.Assessment
                        where a.AssessmentId == assessmentId
                        select a).FirstOrDefault();
            if (entity != null)
            { 
                var baseCount = entity.BaseCount ?? 0;
                var testedCount = entity.TestedCount ?? 0;
                entity.TestedCount = testedCount + 1;
                DataContext.SaveChanges();
                return entity.TestedCount.Value + baseCount;
            }
            return -1;
        }

    }
    public interface IAssessmentRepo : IRepository<Assessment>
    {
        IList<Assessment_Question> GetQuestions(int assessmentId);

        IList<Assessment_Question> GetTopQuestionByResult(int id);

        long ChangeTestedCount(int assessmentId);

    }
}
