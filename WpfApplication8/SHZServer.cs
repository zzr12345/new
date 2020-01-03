using SuperSocket.SocketBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperSocket.SocketBase.Protocol;

namespace WpfApplication8
{
    class SHZServer : AppServer<SHZSession>
    {
        protected override void OnNewSessionConnected(SHZSession session)
        {
            base.OnNewSessionConnected(session);
            Service.instance.Dispatcher.Invoke((Action)delegate ()
            {
                Service.instance.InfoLbl.Content += "\r\n" + session.RemoteEndPoint.Address.ToString() + ":连接\r\n";
            });
        }

        protected override void OnSystemMessageReceived(string messageType, object messageData)
        {
            base.OnSystemMessageReceived(messageType, messageData);
            Service.instance.Dispatcher.Invoke((Action)delegate ()
            {
                Service.instance.InfoLbl.Content += "\r\n" + messageType+messageData+ ":获取消息";
            });
        }

        protected override void ExecuteCommand(SHZSession session, StringRequestInfo requestInfo)
        {
            base.ExecuteCommand(session, requestInfo);
            Service.instance.Dispatcher.Invoke((Action)delegate ()
            {
                string ipAddress_Receive = session.RemoteEndPoint.ToString();
                Service.instance.InfoLbl.Content += "收到" + ipAddress_Receive + "数据: " + requestInfo.Key + " " + requestInfo.Body;
            });
        }

        protected override void OnSessionClosed(SHZSession session, CloseReason reason)
        {
            base.OnSessionClosed(session, reason);
            Service.instance.Dispatcher.Invoke((Action)delegate ()
            {
                Service.instance.InfoLbl.Content += "\r\n" + session.LocalEndPoint.Address.ToString() + ":断开连接";
            });
        }

        protected override void OnStopped()
        {
            base.OnStopped();
            Service.instance.Dispatcher.Invoke((Action)delegate ()
            {
                Service.instance.InfoLbl.Content += "\r\n" + "服务器已停止" + "\r\n";
            });
        }
    }
}
