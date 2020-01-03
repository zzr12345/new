using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApplication8
{
    class SHZSession:AppSession<SHZSession>
    {
        public static SHZSession instance;
        /// <summary>  
        /// 新连接  
        /// </summary>  
        protected override void OnSessionStarted()
        {
            base.OnSessionStarted();
            instance = this;
        }


        /// <summary>  
        /// 捕捉异常并输出  
        /// </summary>  
        /// <param name="e"></param>  
        protected override void HandleException(Exception e)
        {
            this.Send("\r\n Exception: {0}", e.Message);
        }

        /// <summary>  
        /// 未知的Command  
        /// </summary>  
        /// <param name="requestInfo"></param>  
        protected override void HandleUnknownRequest(StringRequestInfo requestInfo)
        {
            base.HandleUnknownRequest(requestInfo);
            Service.instance.Dispatcher.Invoke((Action)delegate () {
                Service.instance.InfoLbl.Content = "\r\n" + requestInfo.Body + "\r\n";
            });
        }

        /// <summary>  
        /// 连接关闭  
        /// </summary>  
        /// <param name="reason"></param>  
        protected override void OnSessionClosed(CloseReason reason)
        {
            Service.instance.Dispatcher.Invoke((Action)delegate () {
                Service.instance.InfoLbl.Content = "\r\n" + reason.ToString() + "\r\n";
            });
        }
        /// <summary>
        /// 向客户端发送消息
        /// </summary>
        /// <param name="msg"></param>
        public void SendMessage(string msg)
        {
            this.Send("Bibabo:" + msg);
            Service.instance.Dispatcher.Invoke((Action)delegate () {
                Service.instance.InfoLbl.Content = "\r\n" + "Bibabo：" + msg + "\r\n";
            });
        }
    }
}
