using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Topuc22Top.Data.ResultModel
{
    public class ArticleDetailResult
    {
        public int ArticleId { get; set; }
        public int ArticleType { get; set; }
        public string ArticleTitle { get; set; }
        public string ArticleContent { get; set; }
        public string ArticleAuthor { get; set; }
        public string UrlHead { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public bool IsActive { get; set; }
        public string Creator { get; set; }
        public int Favor { get; set; }
        public string Thumb { get; set; }
        public int Times { get; set; }
    }

    public class RecommendedArticleResult
    {
        public int ArticleId { get; set; }
        public string ArticleTitle { get; set; }
        public string Thumb { get; set; }
    }
}
