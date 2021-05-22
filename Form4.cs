using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
//代码更新2021/3/30
//代码更新者:舒荣森
namespace CR_网盘
{
    public partial class Form4 : MetroFramework.Forms.MetroForm
    {
        //========声明区=========\\
      
        //获取重构进度条的代码
        Class1 path = new Class1();
        //网络饼干,给服务器吃这个,让他六亲相认!!!!!!
        public string Cookie = "";
        public bool no_file_load = false;
        //用户右键选中的文件
        public Button MouseDoun_File = null;
        //用户的属性
        public JObject json = new JObject();
        //=========网盘路径=========\\

        //网盘ip_url
        public string url = "https://p.qingstore.cn/";
        //查询文件url
        public string files_url_api = "api/v3/directory%2F";
        //查询使用空间url
        public string file_size = "api/v3/user/storage";
        //删除文件url
        public string delte_file = "api/v3/object";
        //下载路径url
        public string download_file = "api/v3/file/download/";
        //上传路径url
        public string upload_file = "api/v3/file/upload/credential?";

        public string bc = "api/v3/callback/onedrive/finish/";
        //==============代码区==========\\

        //窗口初始化接受传入的cokie 和json
        public Form4(string _Cookie , JObject _jo)
        {
            //控件现世
            InitializeComponent();
            //===========同步========\\
            //同步一下网络饼干
            Cookie = _Cookie;
            //同步一下用户属性
            json = _jo;
            //===========初始化数据区=======\\
            //初始化内存容量大小
            progressbar();
            file_load("");
        }
        //文件加载
        public void file_load(string files_url)
        {
            https http = new https();
            JObject files = new JObject();
            files = http.httpget(url+files_url_api + files_url, Cookie);
            //label2.Text = "文件名                                 时间                大小";
            int b = 0;

            Control[] control = new Control[this.panel1.Controls.Count];
            this.panel1.Controls.CopyTo(control, 0);

            foreach (Control c in control)
            {
                if (c is Button)
                {
                    this.panel1.Controls.Remove(c);
                }
            }

            if (files["code"].ToString() == "0") //链接成功
            {
                foreach (JObject file in files["data"]["objects"])
                {              
                    file.Add("Cookie", Cookie);
                    file.Add("delte_file_url", url+delte_file);

                    textBox1.Text = file["path"].ToString().Substring(1, file["path"].ToString().Length-1);

                    Button butt = new Button();//文件名
                    this.panel1.Controls.Add(butt);
                    butt.Location = new Point(0, 30 * b);
                    butt.Width = 100;
                    butt.TextAlign = ContentAlignment.MiddleLeft;
                    butt.Text = file["name"].ToString();
                    butt.Name = file.ToString();
                    butt.ContextMenuStrip = contextMenuStrip1;
                    butt.MouseDown += move_r_Click;
                    butt.Tag = file["name"].ToString();

                    Button date = new Button();//文件上传时间
                    this.panel1.Controls.Add(date);
                    date.Location = new Point(105, 30 * b);
                    date.Width = 150;
                    date.Text = file["date"].ToString();
                    date.Name = file.ToString();
                    date.ContextMenuStrip = contextMenuStrip1;
                    date.MouseDown += move_r_Click;
                    date.Tag = file["name"].ToString();

                    Button size = new Button();//文件大小
                    this.panel1.Controls.Add(size);
                    size.Location = new Point(260, 30 * b);
                    size.Width = 100;
                    size.Name = file.ToString();
                    size.ContextMenuStrip = contextMenuStrip1;
                    size.MouseDown += move_r_Click;
                    size.Tag = file["name"].ToString();
                    if (file["size"].ToString() != "0")
                    {
                        size.Text = file["size"].ToString();
                    }
                    else
                    {
                        size.Text = "-";
                    }

                    if (file["type"].ToString() == "dir")
                    {
                        butt.Click += new System.EventHandler(this.button_files);
                        date.Click += new System.EventHandler(this.button_files);
                        size.Click += new System.EventHandler(this.button_files);
                    }





                    b++;


                }


            }
            else if (files["code"].ToString() == "404")
            {
                MessageBox.Show("路径不存在");
                file_load("");
                textBox1.Text = "";
            }
            this.panel1.AutoScroll = true;
            this.panel1.AutoScrollMinSize = new Size(800, 450);
        }
        //点击文件名打开文件
        public void button_files(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            if (textBox1.Text == "")
            {
                textBox1.Text += button.Tag;
            }
            else
            {
                textBox1.Text += "/" + button.Tag;
            }
            file_load(textBox1.Text);
        }
        //删除文件按钮(多线程)
        public void delete(object sender, EventArgs e)
        {
            Button button = (Button)sender;

            Thread thread = new Thread(new ParameterizedThreadStart(delete_file));
            thread.Start((object)button);


        }
        //删除文件
        public void delete_file(object button)
        {
      
        }
        //刷新内存容量,超大超好用
        public void  progressbar()
        {
            this.progressBar1.Region = new Region(path.GetRoundRectPath(new RectangleF(0, 0, this.progressBar1.Width, this.progressBar1.Height), 5f));



            JObject jo = new https().httpget(url+file_size, Cookie);
            int a = int.Parse((long.Parse(jo["data"]["used"].ToString()) / 1024 / 1024).ToString());
            int b = int.Parse((long.Parse(jo["data"]["total"].ToString()) / 1024 / 1024).ToString());
            //设置进度条参数
            AimProgressBar progress10P = new AimProgressBar();
            progress10P.Maximum = b;
            progress10P.Step = 1;
            progress10P.Value = a;
            Console.WriteLine(b + "/" + a);

            progressBar1.Controls.Add(progress10P);
            progress10P.Dock = DockStyle.Fill;
        }
        //兵解
        private void button1_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process tt = System.Diagnostics.Process.GetProcessById(System.Diagnostics.Process.GetCurrentProcess().Id);
            tt.Kill(); //表演绝活,我杀我自己(退出程序)
        }
        //访问基础文件
        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            file_load(""); 
        }
        //访问路径文件
        private void button3_Click(object sender, EventArgs e)
        {
            file_load(textBox1.Text);
        }
        //右键上传文件
        private void 上传文件ToolSripMenuItem_Click(object sender, EventArgs e) {

            Thread thread1 = new Thread(new ThreadStart(up_file));
            thread1.SetApartmentState(ApartmentState.STA);
            //调用Start方法执行线程
            thread1.Start();


        }
        //上传
        public void up_file() {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = false;//限定上传文件数量
            dialog.Title = "请选择文件,选择后即可上传(不能上传文件夹)";
            dialog.Filter = "上传的文件(*.*)|*.*";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string file = dialog.FileName;
                FileInfo fi = new FileInfo(file);
                https http = new https();
                JObject jo = http.httpget(url + upload_file + "path=%2F&" + "size=" + fi.Length + "&" + "name=" + fi.Name + "&type=onedrive", Cookie);
                string range = "bytes 0-" + (fi.Length - 1) + "/" + (fi.Length);

                if (jo["data"]["policy"].ToString() != "")
                {

                    string a = http.htttppostcook(jo["data"]["policy"].ToString(), Cookie, file, range);

                    jo = http.httpPOST(jo["data"]["token"].ToString(), Cookie, a);

                    //如果上传成功
                    if (jo["code"].ToString() == "0")
                    {
                        this.no_file_load = true;
                    }
                    else 
                    {
                        MessageBox.Show("文件上传失败");
                    }

                }
                else {
                    MessageBox.Show("文件上传失败(文件小于10mb)");
                }

            }
        }
        //右键下载文件
        private void 下载文件ToolStripMenuItem_Click(object sender, EventArgs e) {
            https http = new https ();
            JObject file = (JObject)JsonConvert.DeserializeObject(MouseDoun_File.Name);
            if (file["type"].ToString() == "file")
            {
                JObject download = http.httpput(url + download_file + file["id"].ToString(), Cookie, "");
                System.Diagnostics.Process.Start(download["data"].ToString());
            }
            else {
                MessageBox.Show("文件夹无法下载或者下载失败");
            }
          
        }
        //右键删除文件
        private void 删除文件ToolStripMenuItem_Click(object sender, EventArgs e) {
            if (MouseDoun_File != null)
            {
               
                https http = new https();
                Console.WriteLine(MouseDoun_File.Name);
                JObject file = (JObject)JsonConvert.DeserializeObject(MouseDoun_File.Name);
                string jsonParam;

                if (file["type"].ToString() == "dir")
                {
                    jsonParam = "{\"items\":[]," + "\"dirs\":[\"" + file["id"] + "\"]}";
                }
                else
                {
                    jsonParam = "{\"items\":[\"" + file["id"] + "\"]," + "\"dirs\":[]}";
                }

                JObject jo = http.httpdelete(file["delte_file_url"].ToString(), file["Cookie"].ToString(), jsonParam);

                //如果成功删除
                if (jo["code"].ToString() == "0")
                {
                    this.no_file_load = true;
                }
                else if (jo["code"].ToString() == "203")
                {
                    MessageBox.Show("文件删除失败");
                }
            }
        }
        //右键选取的是那个文件
        private void move_r_Click(object sender , EventArgs e) {
            Button button = (Button)sender;
            MouseDoun_File = button;
        }
        //检测是否需要刷新文件目录
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (no_file_load == true) {
                no_file_load = false;
                file_load("");
            }
        }
        //创建文件夹
        private void add_file_Click(object sender, EventArgs e)
        {
            Form3 anotherForm = new Form3(Cookie, textBox1.Text, json, this);
            anotherForm.ShowDialog();
        }

    }
}
