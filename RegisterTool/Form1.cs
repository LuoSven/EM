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

        private readonly IUserAccountRepo userAccountrepo = new UserAccountRepo(new DatabaseFactory());

        private readonly IUserRoleRepo userRoleRepo = new UserRoleRepo(new DatabaseFactory());
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var roleList = userRoleRepo.GetList();
            foreach (var item in roleList)
            {
                RoleId.DataSource = roleList;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var Msg = "";
            Msg = textBox1.Text == "" ? "账号为空，请重新输入" : "";
            Msg = textBox2.Text == "" ? "密码为空，请重新输入" : "";
            Msg = textBox3.Text == "" ? "手机为空，请重新输入" : "";
            Msg = textBox4.Text == "" ? "用户名为空，请重新输入" : "";

            if (Msg != "")
            {
                MessageBox.Show(Msg);
                return;
            }
            var Account = new EM_User_Account()
            {
                LoginEmail = textBox1.Text,
                Password = textBox2.Text,
                Mobile = textBox3.Text,
                UserName = textBox4.Text
            };
            var IsRepeat = userAccountrepo.IsRepeat(Account);
            if (IsRepeat.Item1)
            {
                Account.Password = DESEncrypt.Encrypt(Account.Password);
                Account.ModifyTime = DateTime.Now;
                Account.CreateTime = DateTime.Now;
                Account.Status = (int)AccountStatus.Allow;
                Account.RoleId = Convert.ToInt32(RoleId.SelectedValue);
                userAccountrepo.Add(Account);

                var result = userAccountrepo.SaveChanges();
                if (result > 0)
                    MessageBox.Show("保存成功");
                else
                    MessageBox.Show("保存失败");
            }
            else
                MessageBox.Show(IsRepeat.Item2);
        }
    }
}
