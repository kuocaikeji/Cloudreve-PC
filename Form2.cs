using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace CR_网盘
{

    public partial class Form2 : Form
    {
        //===============声明区==============\\
        String Cookie;
        Class1 path = new Class1();
        JObject _jo;

        public bool no_file_load = false;
        //查询文件url
        public string files_url_api = "https://pan.baixiongz.com/api/v3/directory/%2F";
        //查询使用空间url
        public string file_size = "https://pan.baixiongz.com/api/v3/user/storage";
        //删除文件url
        public string delte_file_url = "https://pan.baixiongz.com/api/v3/object";

        //==============函数区=================\\
        public Form2(string Cookies, JObject jo)
        {
            InitializeComponent();
            Cookie = Cookies;
            _jo = jo;
            CheckForIllegalCrossThreadCalls = false;



            this.Region = new Region(path.GetRoundRectPath(new RectangleF(0, 0, this.Width, this.Height), 10f));
            this.button1.Region = new Region(path.GetRoundRectPath(new RectangleF(0, 0, this.button1.Width, this.button1.Height), 10f));

            file_load("");

            label3.Text = jo["data"]["nickname"].ToString() + "        " + jo["data"]["group"]["name"];
        }
        //进度条
        private void Form2_Load(object sender, EventArgs e)
        {

            this.progressBar1.Region = new Region(path.GetRoundRectPath(new RectangleF(0, 0, this.progressBar1.Width, this.progressBar1.Height), 5f));



            JObject jo = new https().httpget(file_size, Cookie);
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
        //文件加载
        public void file_load(string files_url)
        {
            https http = new https();
            JObject files = new JObject();
            files = http.httpget(files_url_api + files_url, Cookie);
            label2.Text = "文件名                                 时间                大小";
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
                    Button butt = new Button();//文件名
                    this.panel1.Controls.Add(butt);
                    butt.Location = new Point(0, 30 * b);
                    butt.Width = 300;
                    butt.TextAlign = ContentAlignment.MiddleLeft;
                    butt.Text = file["name"].ToString();
                    butt.Name = file["name"].ToString();


                    Button date = new Button();//文件上传时间
                    this.panel1.Controls.Add(date);
                    date.Location = new Point(300, 30 * b);
                    date.Width = 150;
                    date.Text = file["date"].ToString();
                    date.Name = file["name"].ToString();

                    Button size = new Button();//文件大小
                    this.panel1.Controls.Add(size);
                    size.Location = new Point(450, 30 * b);
                    size.Width = 150;
                    size.Name = file["name"].ToString();
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

                    Button delete = new Button();
                    this.panel1.Controls.Add(delete);
                    delete.Location = new Point(600, 30 * b);
                    delete.Width = 150;
                    delete.Text = "删除";

                    file.Add("Cookie", Cookie);
                    file.Add("delte_file_url", delte_file_url);

                    delete.Name = file.ToString();
                    delete.Click += new System.EventHandler(this.delete);

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
        //禁止修改窗口大小
        private void Form2_SizeChanged(object sender, EventArgs e)
        {
            this.Width = 1080;
            this.Height = 650;
        }
        //返回主页
        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
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
            https http = new https();
            JObject file = (JObject)JsonConvert.DeserializeObject(((Button)button).Name);
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
        //点击文件名打开文件
        public void button_files(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            textBox1.Text += "/" + button.Name;
            file_load(textBox1.Text);
        }
        //更改路径后发生变化
        private void button2_Click(object sender, EventArgs e)
        {
            file_load(textBox1.Text);
        }
        //立即返回主页
        private void button3_Click(object sender, EventArgs e)
        {

            textBox1.Text = "";
            file_load(textBox1.Text);
        }
        //打开创建文件面板
        private void button4_Click(object sender, EventArgs e)
        {
          //  Form3 anotherForm = new Form3(Cookie, textBox1.Text, _jo, this);
         //   anotherForm.ShowDialog();
        }
        //判断是否刷新文件
        private void timer2_Tick(object sender, EventArgs e)
        {
            if (no_file_load == true)
            {
                no_file_load = false;
                file_load("");
            }
        }
        public string files_url = "";//文件上传
        //上传文件
        public void button5_Click(object sender, EventArgs e)
        {
            //获取文件路径
            OpenFileDialog fileDialog1 = new OpenFileDialog();
            fileDialog1.InitialDirectory = "d://";
            fileDialog1.Filter = "xls files (All files (*.*)|*.*";
            fileDialog1.FilterIndex = 1;
            fileDialog1.RestoreDirectory = true;
            if (fileDialog1.ShowDialog() == DialogResult.OK)//如果查找到文件
            {
                files_url = fileDialog1.FileName;
            }
            else//如果文件为空
            {
                files_url = "";
            }
        }

        private void progressBar1_Click(object sender, EventArgs e)
        {

        }
    }

}
