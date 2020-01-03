using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Config;
using SuperSocket.SocketBase.Protocol;
using SuperSocket.SocketEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfApplication8
{
    /// <summary>
    /// Service.xaml 的交互逻辑
    /// </summary>
    public partial class Service : Window
    {
        public static Service instance;
        SHZServer appServer;
        ServerConfig serverConfig;
        //string ip = "169.254.197.183";
        //string ip = "127.0.0.1";
        int prort = 50000;
        public Service()
        {
            InitializeComponent();
            instance = this;
        }

        public void Init()
        {
            appServer = new SHZServer();
            serverConfig = new ServerConfig
            {
                Port = prort,
                //Ip = ip
            };
            if (!appServer.Setup(serverConfig))
            {
                InfoLbl.Content+= "初始化失败！\r\n";
                return;
            }
            if (!appServer.Start())
            {
                InfoLbl.Content+= "服务器启动失败！\r\n";
                return;
            }
            InfoLbl.Content+= "启动服务器成功！\r\n";
            //appServer.NewRequestReceived += appServer_NewRequestReceived;
            //var bootstrap = BootstrapFactory.CreateBootstrap();
            //AppServer appServer;
            //appServer.NewRequestReceived += new RequestHandler<AppSession, StringRequestInfo>(appServer_NewRequestReceived);
        }

        string ipAddress_Receive;

        void appServer_NewRequestReceived(AppSession session, StringRequestInfo requestInfo)
        {
            //requestInfo.Key 是请求的命令行用空格分隔开的第一部分
            //requestInfo.Parameters 是用空格分隔开的其余部分
            //requestInfo.Body 是出了请求头之外的所有内容
            ipAddress_Receive = session.RemoteEndPoint.ToString();
            InfoLbl.Content += ("收到" + ipAddress_Receive + "数据: " + requestInfo.Key + " " + requestInfo.Body);
        }


        public string GetLocalIp()
        {
            ///获取本地的IP地址
            string AddressIP = string.Empty;
            var a = Dns.GetHostEntry(Dns.GetHostName()).AddressList;
            foreach (IPAddress _IPAddress in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
            {
                if (_IPAddress.AddressFamily.ToString() == "InterNetwork")
                {
                    AddressIP = _IPAddress.ToString();
                }
            }
            return AddressIP;
        }

        //static void appServer_NewRequestReceived(AppSession session, StringRequestInfo requestInfo)
        //{
        //    switch (requestInfo.Key.ToUpper())
        //    {
        //        case ("ECHO"):
        //            session.Send(requestInfo.Body);
        //            break;

        //        case ("ADD"):
        //            session.Send(requestInfo.Parameters.Select(p => Convert.ToInt32(p)).Sum().ToString());
        //            break;

        //        case ("MULT"):

        //            var result = 1;

        //            foreach (var factor in requestInfo.Parameters.Select(p => Convert.ToInt32(p)))
        //            {
        //                result *= factor;
        //            }

        //            session.Send(result.ToString());
        //            break;
        //    }
        //}

        /// <summary>
        /// 开启服务器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //GetLocalIp();
            Init();
        }
        /// <summary>
        /// 关闭服务器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            appServer.Stop();
        }
        /// <summary>
        /// 向客户端发送消息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            try
            {
                SHZSession.instance.SendMessage(txtBoxMsg.Text);
            }
            catch (System.Exception)
            {
                MessageBox.Show("当前没有客户端链接！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
