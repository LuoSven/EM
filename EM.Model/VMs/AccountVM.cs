using EM.Common;
using EM.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;

namespace EM.Model.VMs
{

    public class AccountVM
    {
        public AccountVM()
        {

        }
        public AccountVM(string CookieValue)
        {
            if (CookieValue == "")
                UserId = 0;
            else
            {
                CookieValue = DESEncrypt.Decrypt(CookieValue);
                var account = CookieValue.Split(StaticKey.SplitChar);
                UserId = account[0].ToInt();
                UserName = account[1];
                Mobile = account[2];
                SystemIds = account[3].ToInts();
                UserRole = account[4].ToInt();
                CompanyIds = account[5];
                RoleType = account[6].ToInt();
                CateIds = account[7];
                ViewRightType = account[8].ToInt();
            }



        }

        public string SetCookie()
        {
            string[] acconutCookie = { UserId.ToString(), UserName, Mobile, string.Join(",", SystemIds), UserRole.ToString(), CompanyIds, RoleType.ToString(), CateIds, ViewRightType.ToString() };
            return DESEncrypt.Encrypt(string.Join(StaticKey.Split, acconutCookie));
        }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Mobile { get; set; }
        public List<int> SystemIds { get; set; }

        public int UserRole { get; set; }


        public string Message { get; set; }

        public string CompanyIds { get; set; }
        public int RoleType { get; set; }
        public string CateIds { get; set; }
        public int ViewRightType { get; set; }
    }
}
