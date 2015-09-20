using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topuc22Top.Data.Infrastructure;
using Topuc22Top.Model.Entities;
using System.Data.Entity;
using Topuc22Top.Data.ResultModel;
using Topuc22Top.Data.ViewModel;

namespace Topuc22Top.Data.Repositories
{
    public class CompanyContactRepo : RepositoryBase<TB_Enterprise_Contact>, ICompanyContactRepo
    {
        public CompanyContactRepo(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }

        public TB_Enterprise_Contact GetByEtpId(int id)
        {
            return DataContext.TB_Enterprise_Contact.Where(c => c.EnterpriseId == id).FirstOrDefault();
        }

        public SEOCompanyContact GetSeoCompanyContact(int id)
        {
            var item = (from e in DataContext.TB_Enterprise_Contact
                        where e.EnterpriseId == id
                        select new SEOCompanyContact
                        {
                            Address = e.Address,
                            Email = e.ContactEmail,
                            Phone = !string.IsNullOrEmpty(e.ContactTelephone) ? 
                            (
                                (!string.IsNullOrEmpty(e.ContactAreaCode) ? e.ContactAreaCode + "-" : "")
                                + (!string.IsNullOrEmpty(e.ContactTelephone) ? e.ContactTelephone + "-" : "")
                                + (!string.IsNullOrEmpty(e.ContactExt) ? e.ContactExt + "-" : "")
                            )
                            : e.ContactMobile
                        }).FirstOrDefault();
            item.Phone = (item.Phone ?? "").Replace("--", "-").Replace("--", "-").Trim('-');
            return item;
        }

        public void UpdateContact(int id, string code, string telephone, string ext) 
        {
            var contact = (from entity in DataContext.TB_Enterprise_Contact
                          where entity.EnterpriseId == id
                          select entity).FirstOrDefault();
            if(contact != null)
            {
                contact.ContactAreaCode = code;
                contact.ContactTelephone = telephone;
                contact.ContactExt = ext;
                DataContext.SaveChanges();
            }
        }
        public string[] GetEmailAndContactManByEtpId(int id)
        {
            string[] result = new string[2];
            var Email = "";
            var ContactMan = "";
            var t = GetByEtpId(id);
            if (t != null && !string.IsNullOrEmpty(t.ContactEmail))
                Email = t.ContactEmail;
            else
            {
                ICompanyAccountRepo icompanyAccountRepo = new CompanyAccountRepo(new DatabaseFactory());
                Email = icompanyAccountRepo.GetLoginEmail(id);
            }
            if (t != null && !string.IsNullOrEmpty(t.ContactMan))
                ContactMan = t.ContactMan;

            result[0] = Email;
            result[1] = ContactMan;

            return result;
        }
        public string GetContactManByEtpId(int id)
        {
            var ContactMan = "";
            var t = GetByEtpId(id);
            if (t != null && !string.IsNullOrEmpty(t.ContactEmail))
                ContactMan = t.ContactMan;
            return ContactMan;
        }
    }
    public interface ICompanyContactRepo : IRepository<TB_Enterprise_Contact>
    {

        TB_Enterprise_Contact GetByEtpId(int id);

        /// <summary>
        /// 根据id获取Email和联系人，默认是联系方式内的邮箱，无则返回account内邮箱
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        string[] GetEmailAndContactManByEtpId(int id);

        SEOCompanyContact GetSeoCompanyContact(int id);

        void UpdateContact(int id, string code, string telephone, string ext);
    }
}
