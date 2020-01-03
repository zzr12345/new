using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Config;
using SuperSocket.SocketBase.Protocol;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace WpfApplication8
{

    public class DTServer:AppServer<DTSession, DTRequestInfo>
    {
        //Timer requestTimer = null;

        public DTServer() : base(new DefaultReceiveFilterFactory<DTReceiveFilter, DTRequestInfo>())
        {
            //定时发送请求压力的报文
            //double sendInterval = double.Parse(ConfigurationManager.AppSettings["sendInterval"]);
            //requestTimer = new Timer(sendInterval);
            //requestTimer.Elapsed += RequestTimer_Elapsed;
            //requestTimer.Enabled = true;
            //requestTimer.Start();
        }

        //private void RequestTimer_Elapsed(object sender, ElapsedEventArgs e)
        //{
        //    //发送请求报文
        //    var sessionList = GetAllSessions();
        //    //Logger.Error(sessionList);
        //    foreach (var session in sessionList)
        //    {
        //        Dictionary<string, string> routs = ConfigManager.GetAllConfig();
        //        try
        //        {
        //            foreach (var item in routs)
        //            {
        //                if (item.Key.ToString().Contains("rout2_"))
        //                {
        //                    string routeID = item.Key.ToString().Split('_')[1];
        //                    byte[] rout = ConvertHelper.strToToHexByte(routeID);
        //                    byte[] address = ConvertHelper.strToToHexByte(item.Value.ToString());
        //                    /// 合成报文 
        //                    List<byte> data = new List<byte>();
        //                    data.Add(rout[0]);
        //                    data.Add(0x04);//读取数据
        //                    data.Add(address[0]);
        //                    data.Add(address[1]);
        //                    data.Add(address[2]);
        //                    data.Add(address[3]);
        //                    byte[] checkcode = CRC16.crc_16(data.ToArray());
        //                    data.Add(checkcode[1]);
        //                    data.Add(checkcode[0]);
        //                    /// 发送报文
        //                    //使用字节抽屉存储
        //                    // ArraySegment<byte> sendData = new ArraySegment<byte>(data.ToArray());
        //                    session.Send(data.ToArray(), 0, data.ToArray().Length);
        //                    // Console.WriteLine("发送数据：" + ConvertHelper.byteToHexStr(data.ToArray()));
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            //写入日志
        //            /// Logger.Info(ex.Message);
        //        }
        //    }
        //}

        protected override void OnNewSessionConnected(DTSession session)
        {
            base.OnNewSessionConnected(session);
            Logger.Info(session.RemoteEndPoint);
        }

        protected override void ExecuteCommand(DTSession session, DTRequestInfo requestInfo)
        {
            base.ExecuteCommand(session, requestInfo);
        }
        protected override void OnStarted()
        {
            base.OnStarted();
        }

        [Obsolete("OnStart() is obsolete,Use OnStarted() instead")]
        protected override void OnStartup()
        {
            base.OnStartup();
        }

        protected override bool Setup(IRootConfig rootConfig, IServerConfig config)
        {
            return base.Setup(rootConfig, config);
        }
    }


}
