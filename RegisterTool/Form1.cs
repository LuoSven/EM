using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EM.Data.Repositories;
using EM.Data.Infrastructure;
using EM.Model.Entities;
using EM.Common;
using EM.Utils;

namespace RegisterTool
{
    public partial class Form1 : Form
    {

        private readonly IUserAccountRepo userAccountrepo;
        public Form1()
        {
            InitializeComponent();
            userAccountrepo = new UserAccountRepo(new DatabaseFactory());
        }

        private void Form1_Load(object sender, EventArgs e)
        {
          
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var Msg="";
            Msg = textBox1.Text == "" ? "账号为空，请重新输入" : "";
            Msg = textBox2.Text == "" ? "密码为空，请重新输入" : "";
            Msg = textBox3.Text == "" ? "手机为空，请重新输入" : "";
            Msg = textBox4.Text == "" ? "用户名为空，请重新输入" : "";

            if (Msg != "")
            {
                MessageBox.Show(Msg);
                return;
            }
            var Account=new EM_User_Account()
            {
                LoginEmaill = textBox1.Text,
                Password = textBox2.Text,
                Mobile = textBox3.Text,
                UserName = textBox4.Text
            };
            var IsRepeat = userAccountrepo.IsRepeat(Account);
         if(IsRepeat.Item1)
         {
             Account.Password=DESEncrypt.Encrypt(Account.Password);
             Account.ModifyTime = DateTime.Now;
             Account.CreateTime = DateTime.Now;
             Account.Status = (int)AccountStatus.Allow;
            userAccountrepo.Add(Account);
            userAccountrepo.SaveChanges();
             
         
         }
        }
    }
}
