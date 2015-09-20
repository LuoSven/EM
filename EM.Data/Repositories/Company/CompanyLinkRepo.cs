using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topuc22Top.Data.Infrastructure;
using Topuc22Top.Data.ResultModel;
using Topuc22Top.Model.Entities;
using System.Data.Entity;
using System.Xml;

namespace Topuc22Top.Data.Repositories
{


    public class CompanyLinkRepo : RepositoryBase<TB_Enterprise_Link>, ICompanyLinkRepo
    {
        public CompanyLinkRepo(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }

        public async Task<CompanyLinkResult> GetLinksAsync(int etpId)
        {
            using (TopucDB DataContext = new TopucDB())
            {
                CompanyLinkResult clr = new CompanyLinkResult();
                var link =await DataContext.TB_Enterprise_Link.Where(v => v.EnterpriseId == etpId).SingleOrDefaultAsync();
                if (link != null)
                {
                    if (!string.IsNullOrEmpty(link.LinkXml))
                    {
                        XmlDocument xmldoc = new XmlDocument();
                        xmldoc.LoadXml(link.LinkXml);
                        var root = xmldoc.ChildNodes;
                        if (root.Count > 0)
                        {
                            var items = root[0].ChildNodes;

                            for (int i = 0; i < items.Count; i++)
                            {
                                var name = items[i]["name"] != null ? items[i]["name"].InnerText : "";
                                var url = items[i]["url"] != null ? items[i]["url"].InnerText : "";

                                switch (name)
                                {
                                    case "Sina":
                                        clr.SinaLink = url;
                                        break;
                                    case "Tencent":
                                        clr.TencentLink = url;
                                        break;
                                    case "RenRen":
                                        clr.RenRenLink = url;
                                        break;
                                    case "Facebook":
                                        clr.Facebook = url;
                                        break;
                                    case "Twitter":
                                        clr.Twitter = url;
                                        break;
                                    case "Google":
                                        clr.Google = url;
                                        break;
                                    case "WeChat":
                                        clr.WeChat = url;
                                        break;
                                }
                            }
                        }
                    }
                }
                return clr;
            }

        }
    }
    public interface ICompanyLinkRepo : IRepository<TB_Enterprise_Link>
    {
        Task<CompanyLinkResult> GetLinksAsync(int etpId);

    }
}
