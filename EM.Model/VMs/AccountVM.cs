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
                UserId = Convert.ToInt32(account[0]);
                UserName = account[1];
                Mobile = account[2];
                var Ids = account[3].Split(',');
                SystemIds = new List<int>();
                foreach (var item in Ids)
                {
                    SystemIds.Add(Convert.ToInt32(item));
                }
                UserRole = Convert.ToInt32(account[4]);
                CompanyIds = account[5];
                
                RoleType =Convert.ToInt32( account[6]);

            }



        }

        public string SetCookie()
        {
            string[] acconutCookie = { UserId.ToString(), UserName, Mobile, string.Join(",", SystemIds), UserRole.ToString(), CompanyIds, RoleType.ToString() };
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
    }
}
