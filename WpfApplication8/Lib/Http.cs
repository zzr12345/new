using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApplication8.Lib
{
    class Http
    {

        public static Response<JObject> GetList(string id)
        {
            var url = String.Format("http://59.110.48.109:10013/data/{0}/appId/5df2ff67fa2f6e4fe0acb5e7", id);
            var rspJson = post(url, null);
            return dealResponse<JObject>(rspJson);
        }

        public static Response<JArray> GetchnList()
        {
            var url = String.Format("http://59.110.48.109:10013/getViewData/0/5df2ff89390c1c6d44ba6b5f/all?page=1&size=20");
            var rspJson = post(url, null);
            return dealResponse<JArray>(rspJson);
        }


        /// <summary>
        /// http post 请求
        /// </summary>
        /// <param name="url">请求路径</param>
        /// <param name="body">请求体</param>
        /// <returns></returns>
        public static string post(string url, Object body)
        {
            String param = JsonConvert.SerializeObject(body);
            byte[] data = Encoding.UTF8.GetBytes(param);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.KeepAlive = false;//是否保持活动
            request.Method = "POST";//请求方法，通常是Get或者Post
            request.ContentType = "application/json";//请求体格式
            request.ContentLength = data.Length;//请求体长度

            Stream sm = request.GetRequestStream();
            sm.Write(data, 0, data.Length);
            sm.Close();
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream streamResponse = response.GetResponseStream();
            StreamReader streamRead = new StreamReader(streamResponse, Encoding.UTF8);
            Char[] readBuff = new Char[256];
            int count = streamRead.Read(readBuff, 0, 256);
            string content = "";
            while (count > 0)
            {
                String outputData = new String(readBuff, 0, count);
                content += outputData;
                count = streamRead.Read(readBuff, 0, 256);
            }
            response.Close();
            return content;
        }

        public static string get(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.KeepAlive = false;//是否保持活动
            request.Method = "GET";//请求方法，通常是Get或者Post
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream streamResponse = response.GetResponseStream();
            StreamReader streamRead = new StreamReader(streamResponse, Encoding.UTF8);
            Char[] readBuff = new Char[256];
            int count = streamRead.Read(readBuff, 0, 256);
            string content = "";
            while (count > 0)
            {
                String outputData = new String(readBuff, 0, count);
                content += outputData;
                count = streamRead.Read(readBuff, 0, 256);
            }
            response.Close();
            return content;
        }

        private static Response<T> getErrResponse<T>(string msg)
        {
            return new Response<T>
            {
                code = 666,
                msg = msg,
            };
        }

        private static Response<T> dealResponse<T>(string json)
        {
            Response<T> response = null;
            try
            {
                response = JsonConvert.DeserializeObject<Response<T>>(json);
            }
            catch(Exception e)
            {

                return getErrResponse<T>(e.Message);
            }
            return response;
        }
    }
}
