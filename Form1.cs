using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CR_网盘
{
    public partial class Form1 : MetroFramework.Forms.MetroForm
    {
        public Form1()
        {
            InitializeComponent();

            //FormBorderStyle = FormBorderStyle.FixedSingle;//设置窗口无法被拉伸

        }
        //账号注册url
        public string zhzc = "https://p.qingstore.cn/signup";
        //账号登陆url
        public string zhdl = "https://p.qingstore.cn/api/v3/user/session";

            private void Form1_Load(object sender, EventArgs e)
        {//控件加入圆角

            Class1 path = new Class1();


        }

        //判断是否显示密码
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBox1.Checked == false)
            {
                this.Password.UseSystemPasswordChar = true;
            }
            else
            {
                this.Password.UseSystemPasswordChar = false;
            }
        }



        private GraphicsPath GetRoundedRectPath(Rectangle rect, int radius)
        {
            int diameter = radius;

            Rectangle arcRect = new Rectangle(rect.Location, new Size(diameter, diameter));

            GraphicsPath path = new GraphicsPath();

            //   左上角   
            path.AddArc(arcRect, 185, 90);

            //   右上角   
            arcRect.X = rect.Right - diameter;

            path.AddArc(arcRect, 275, 90);

            //   右下角   
            arcRect.Y = rect.Bottom - diameter;

            path.AddArc(arcRect, 356, 90);

            //   左下角   
            arcRect.X = rect.Left;

            arcRect.Width += 2;

            arcRect.Height += 2;

            path.AddArc(arcRect, 90, 90);

            path.CloseFigure();

            return path;
        }

        public string Cookie = "";
        //登录
        private void button1_Click(object sender, EventArgs e)
        {
            label4.Text = "登录中";

            dl(userName.Text, Password.Text);

        }
        //账号注册
        private void button2_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(zhzc);
        }


        //登录引用
        private void dl(string _userName, string _Password)
        {
            string jsonParam = "{\"userName\":\"" + _userName + "\",\"Password\":\"" + _Password + "\",\"captchaCode\":\"\"}";
            https dl = new https();
            JObject jo =  dl.httppost(zhdl, jsonParam, "111", out Cookie);
            if (jo["cod"].ToString().EndsWith("0"))
            {
                Console.WriteLine("////" + Cookie);
                if (jo["code"].ToString() == "0")
                {

                    label4.Text = "登录成功";
                    //new出主菜单,并传入cookie,和json信息
                    Form4 anotherForm = new Form4(Cookie, jo);
                    //实例化主菜单
                    anotherForm.Show();
                    //隐藏登录界面
                    this.Hide();

                }
                else if (jo["code"].ToString() == "40001")
                {
                    label4.Text = jo["msg"].ToString();
                }

            }
            else {
                label4.Text = jo["cod"].ToString();
            }

        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            this.Width = 400;
            this.Height = 230;
        }
    }
}
