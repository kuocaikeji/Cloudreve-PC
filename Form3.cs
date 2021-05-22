using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CR_网盘
{
    public partial class Form3 : MetroFramework.Forms.MetroForm
    {
        //创建文件api路径
        string set_file_api = "https://pan.baixiongz.com/api/v3/directory";
        string Cookie;

        string file_path;

        Form4 f2;
        JObject _jo;
        public Form3(string Cookies , string path,JObject jo, Form4 from4)
        {
            InitializeComponent();
            Cookie = Cookies;
            file_path = path;
            _jo = jo;
            f2 = from4;
        }

        private void Form3_Load(object sender, EventArgs e)
        {

        }
        //禁止修改窗口大小
        private void Form3_SizeChanged(object sender, EventArgs e)
        {
            this.Width = 680;
            this.Height = 170;
        }
       
        //点击创建文件后
        private void button1_Click(object sender, EventArgs e)
        {
            https http = new https();
            string jsonParam = "{\"path\":" + "\"/" + file_path +"/"+ textBox1.Text + "\"}";
            JObject jo =  http.httpput(set_file_api,Cookie, jsonParam);
            if (jo["code"].ToString() == "0") {
               
                f2.file_load(file_path);
                this.Close();
            }
        }
    }
}
