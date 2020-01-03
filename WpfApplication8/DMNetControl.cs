using LitJson;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

namespace WpfApplication8
{
    class DMNetControl: MonoBehaviour
    {
        public delegate void RespondOfHttpJD(JsonData jsondata);
        public delegate void RespondOfHttpDic(JsonData jsondata);
        public static DMNetControl Instance;
        private Socket ClientSocket;
        private const string Ip = "127.0.0.1";
        private const int Port = 2017;

        public void Awake()
        {
            Instance = this;
            ClientSocket = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
            IPAddress ipAddress = IPAddress.Parse(Ip);
            IPEndPoint point = new IPEndPoint(ipAddress, Port);
            ClientSocket.Connect(point);
        }

        #region Socket通信代码
        /// <summary>
        /// 向服务器发送消息，并接收来自服务器的回复消息
        /// </summary>
        /// <param name="_strBytes">发送给服务器的字节</param>
        /// <param name="_respondFunction">服务器返回给客户端的回调数据</param>
        void ReceiveMessage(byte[] _strBytes, RespondOfHttpJD _respondFunction)
        {
            try
            {
                this.ClientSocket.Send(_strBytes);
                _respondFunction?.Invoke(AsynRecive(ClientSocket));
            }
            catch (Exception ex)
            {
                Debug.LogError("写Error日志" + ex.ToString());
            }
        }

        /// <summary>
        /// Socket 消息回传
        /// </summary>
        /// <returns></returns>
        public JsonData AsynRecive(Socket socket)
        {
            try
            {
                byte[] msg = new byte[1024];
                var da = Encoding.UTF8.GetString(msg);
                int recv = socket.Receive(msg);
                //因为SuperSocket发送的数据带有\r\n换行符，所以在接收到数据的时候要进行特殊处理，
                //要把‘\r\n’去掉，否则json转换会出错
                string str = Encoding.UTF8.GetString(msg, 0, recv).Replace("\r\n", " ");
                JsonData jd = JsonMapper.ToObject(str);
                return jd;
            }
            catch (Exception ex)
            {
                socket.Close();
                return "Socket 消息回传,写Error日志" + ex.Message;
            }
        }
        /// <summary>
        /// 发送给服务器的数据
        /// </summary>
        /// <param name="_scriptName">服务器端，协议类名</param>
        /// <param name="_netForm">发送给服务器的json数据</param>
        /// <param name="respondFunction">服务器返回给客户端的数据</param>
        public void RequestOfSocket(string _scriptName, DMNetForm _netForm, RespondOfHttpJD respondFunction)
        {
            string jdStr = JsonMapper.ToJson(_netForm.GetSendObj);
            string jd = _scriptName + jdStr + "\r\n";
            Debug.LogError(jd);
            byte[] strBytes = Encoding.UTF8.GetBytes(jd);
            ReceiveMessage(strBytes, respondFunction);
        }
        #endregion

        #region Http通信代码，暂时用不到
        private IEnumerator<WWW> RequestServer(byte[] _strBytes, RespondOfHttpJD _respondFunction)
        {
            using (WWW www = new WWW("http://127.0.0.1:2017", _strBytes))
            {
                yield return www;
                if (www.error == null)
                {
                    string wwwStr = www.text;
                    Debug.Log("wwwStr:" + wwwStr);
                    JsonData jd2 = JsonMapper.ToObject(wwwStr.Replace("[[", "[{").Replace("]]", "}]"));
                    JsonData jd = JsonMapper.ToObject(wwwStr);
                    _respondFunction?.Invoke(jd);
                }
                else
                {
                    Debug.Log(www.error);
                }
            }
        }

        public void RequestOfHttp(string _scriptName, DMNetForm _netForm, RespondOfHttpJD respondFunction)
        {
            string jdStr = JsonMapper.ToJson(_netForm.GetSendObj);
            string jddd = _scriptName + jdStr + "\r\n";
            Debug.LogError(jddd);
            byte[] strBytes = Encoding.UTF8.GetBytes(jddd);
            StartCoroutine(RequestServer(strBytes, respondFunction));
        }
        #endregion
    }
}
