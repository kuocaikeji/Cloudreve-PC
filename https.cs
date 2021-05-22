using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CR_网盘
{
    class https
    {
        
        //get请求url,cookies
        public JObject httpget(string url, string cookies) {
            try
            {
                HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(url);

                if (cookies != "")//判断是否有cookies
                {
                    myRequest.Headers.Add("cookie", cookies);
                }
                myRequest.Method = "GET";

                HttpWebResponse myResponse = (HttpWebResponse)myRequest.GetResponse();

                StreamReader reader = new StreamReader(myResponse.GetResponseStream(), Encoding.UTF8);

                string content = reader.ReadToEnd();

                reader.Close();
                JObject jo = (JObject)JsonConvert.DeserializeObject(content);
                jo.Add("cod", "0");
                return jo;
            }
            catch {
                JObject jo = new JObject();
                jo.Add("cod", "程序出错,或网络出错");
                return jo;
            }
        }
        protected CookieContainer coos = new CookieContainer();
        //post请求
        public JObject httppost(string url,string jsonParam,string cookies,out string Cookie) {
            try
            {
                //json参数

                var request = (HttpWebRequest)HttpWebRequest.Create(new Uri(url));



                request.CookieContainer = new CookieContainer();

                request.Method = "POST";

               request.Headers.Add("Cookie", cookies );
                request.ContentType = "application/json;charset=UTF-8";

                byte[] byteData = Encoding.UTF8.GetBytes(jsonParam);

                int length = byteData.Length;

                request.ContentLength = length;

                request.CookieContainer = new CookieContainer();

                Stream writer = request.GetRequestStream();
                writer.Write(byteData, 0, length);
                writer.Close();
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                var responseString = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("utf-8")).ReadToEnd();


                //获取服务器的cookie
                Cookie = request.CookieContainer.GetCookieHeader(new Uri(url));



                JObject jo = (JObject)JsonConvert.DeserializeObject(responseString);
                jo.Add("cod", "0");
                return jo;
            }
            catch {
                JObject jo = new JObject();
                jo.Add("cod", "程序出错,或网络出错");
                Cookie = "";
                return jo;
            }
        }
        //DELETE请求
        public JObject httpdelete(string url, string cookies, string jsonParam) {

            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "DELETE";


            request.ContentType = "application/json;charset=UTF-8";
            request.Accept = "application/json, text/plain, */*";
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/56.0.2924.87 Safari/537.36";

            request.Headers.Add("cookie", cookies + ";path_tmp=");
            byte[] byteData = Encoding.UTF8.GetBytes(jsonParam);
            int length = byteData.Length;
            request.ContentLength = length;
            Stream writer = request.GetRequestStream();

            writer.Write(byteData, 0, length);
            writer.Close();

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            var responseString = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("utf-8")).ReadToEnd();


            JObject jo = (JObject)JsonConvert.DeserializeObject(responseString);

            Console.WriteLine(jo);

            return jo;
        }
        //PUT请求
        public JObject httpput(string url,string cookies,string jsonParam) {

            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "PUT";


            request.ContentType = "application/json;charset=UTF-8";
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/56.0.2924.87 Safari/537.36";

            request.Headers.Add("cookie", cookies + ";path_tmp=");
            byte[] byteData = Encoding.UTF8.GetBytes(jsonParam);
            int length = byteData.Length;
            request.ContentLength = length;
            Stream writer = request.GetRequestStream();

            writer.Write(byteData, 0, length);
            writer.Close();

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            var responseString = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("utf-8")).ReadToEnd();


            JObject jo = (JObject)JsonConvert.DeserializeObject(responseString);
            return jo;
        }
        public JObject httpPOST(string url, string cookies, string jsonParam)
        {

            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";

            
            request.ContentType = "application/json;charset=UTF-8";
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/56.0.2924.87 Safari/537.36";

            request.Headers.Add("cookie", cookies + ";path_tmp=");
            byte[] byteData = Encoding.UTF8.GetBytes(jsonParam);
            int length = byteData.Length;
            request.ContentLength = length;
            Stream writer = request.GetRequestStream();

            writer.Write(byteData, 0, length);
            writer.Close();

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            var responseString = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("utf-8")).ReadToEnd();


            JObject jo = (JObject)JsonConvert.DeserializeObject(responseString);
            return jo;
        }
        public string htttppostcook(string url,string cookies, string path,string range)
        {

                // 设置参数

                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;

                CookieContainer cookieContainer = new CookieContainer();

                request.CookieContainer = cookieContainer;

                request.AllowAutoRedirect = true;

                request.Method = "PUT";
                request.Headers.Add("cookie", cookies + ";path_tmp=");
                request.Headers.Add("content-range", range);
                
                string boundary = DateTime.Now.Ticks.ToString("X"); // 随机分隔线

                request.ContentType = "multipart/form-data;charset=utf-8;boundary=" + boundary;

                byte[] itemBoundaryBytes = Encoding.UTF8.GetBytes("\r\n--" + boundary + "\r\n");

                byte[] endBoundaryBytes = Encoding.UTF8.GetBytes("\r\n--" + boundary + "--\r\n");
                int pos = path.LastIndexOf("\\");
           
            


            string fileName = path.Substring(pos + 1);
                //请求头部信息 

                StringBuilder sbHeader = new StringBuilder(string.Format("Content-Disposition:form-data;name=\"file\";filename=\"{0}\"\r\nContent-Type:application/octet-stream\r\n\r\n", fileName));

                byte[] postHeaderBytes = Encoding.UTF8.GetBytes(sbHeader.ToString());
                FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
         
            byte[] bArr = new byte[fs.Length];

                fs.Read(bArr, 0, bArr.Length);

                fs.Close();
                Stream postStream = request.GetRequestStream();

           //     postStream.Write(itemBoundaryBytes, 0, itemBoundaryBytes.Length);

//                postStream.Write(postHeaderBytes, 0, postHeaderBytes.Length);

                postStream.Write(bArr, 0, bArr.Length);

              //  postStream.Write(endBoundaryBytes, 0, endBoundaryBytes.Length);

                postStream.Close();
            Console.WriteLine(bArr.Length);
            //发送请求并获取相应回应数据

            HttpWebResponse response = request.GetResponse() as HttpWebResponse;

                //直到request.GetResponse()程序才开始向目标网页发送Post请求

                Stream instream = response.GetResponseStream();

                StreamReader sr = new StreamReader(instream, Encoding.UTF8);

                //返回结果网页（html）代码

                string content = sr.ReadToEnd();
            return content;





        }
    }
}
