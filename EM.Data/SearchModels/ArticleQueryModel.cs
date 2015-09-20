using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.ModelBinding;

namespace Topuc22Top.Data.SearchModels
{
    [ModelBinder]
    public class ArticleQueryModel
    {
        public string q { get; set; }
        public string tag { get; set; }
        public int type { get; set; }
        public int page { get; set; }
        public int pageSize { get; set; }
    }
}
