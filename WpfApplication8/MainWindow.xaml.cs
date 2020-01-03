using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApplication8
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public string text { get; set; } = "jj";
        Socket socket;
        SerialPort _sp;
        Stopwatch getTime = new Stopwatch();
        string item = null;
        public MainWindow()
        {
            InitializeComponent();
            socket = null;
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    grid[i, j] = 0;
            //socket.Select(checkRead, checkWrite, checkError, 1000
        }

        public void reset()
        {
            int port = 888;//端口  
            TcpClient tcpClient;//创建TCP连接对象
            IPAddress[] serverIP = Dns.GetHostAddresses("127.0.0.1");//定义IP地址   (要解析的IP地址)
            IPAddress localAddress = serverIP[0];//IP地址 
            TcpListener tcpListener = new TcpListener(localAddress, port);
            tcpListener.Start();
            Console.WriteLine("服务器启动成功，等待用户接入…");
            while (true)
            {
                try
                {
                    tcpClient = tcpListener.AcceptTcpClient();//每接收一个客户端则生成一个TcpClient  
                    NetworkStream networkStream = tcpClient.GetStream();//获取网络数据流
                    BinaryReader reader = new BinaryReader(networkStream);//定义流数据读取对象
                    BinaryWriter writer = new BinaryWriter(networkStream);//定义流数据写入对象
                    while (true)
                    {
                        try
                        {
                            string strReader = reader.ReadString();//接收消息
                            string[] strReaders = strReader.Split(new char[] { ' ' });//截取客户端消息
                            Console.WriteLine("有客户端接入，客户IP：" + strReaders[0]);//输出接收的客户端IP地址  
                            Console.WriteLine("来自客户端的消息：" + strReaders[1]);//输出接收的消息  
                            string strWriter = "我是服务器，欢迎光临";//定义服务端要写入的消息
                            writer.Write(strWriter);//向对方发送消息  
                        }
                        catch
                        {
                            break;
                        }
                    }
                }
                catch
                {
                    break;
                }
            }

        }

        public void set()
        {
            {
                TcpClient tcpClient = new TcpClient();//创建一个TcpClient对象，自动分配主机IP地址和端口号  
                tcpClient.Connect("127.0.0.1", 888);//连接服务器，其IP和端口号为127.0.0.1和888  
                if (tcpClient != null)//判断是否连接成功
                {
                    Console.WriteLine("连接服务器成功");
                    NetworkStream networkStream = tcpClient.GetStream();//获取数据流
                    BinaryReader reader = new BinaryReader(networkStream);//定义流数据读取对象
                    BinaryWriter writer = new BinaryWriter(networkStream);//定义流数据写入对象
                    string localip = "127.0.0.1";//存储本机IP，默认值为127.0.0.1
                    IPAddress[] ips = Dns.GetHostAddresses(Dns.GetHostName());//获取所有IP地址
                    foreach (IPAddress ip in ips)
                    {
                        if (!ip.IsIPv6SiteLocal)//如果不是IPV6地址
                            localip = ip.ToString();//获取本机IP地址
                    }
                    writer.Write(localip + " 你好服务器，我是客户端");//向服务器发送消息  
                    while (true)
                    {
                        try
                        {
                            string strReader = reader.ReadString();//接收服务器发送的数据  
                            if (strReader != null)
                            {
                                Console.WriteLine("来自服务器的消息：" + strReader);//输出接收的服务器消息
                            }
                        }
                        catch
                        {
                            break;//接收过程中如果出现异常，退出循环  
                        }
                    }
                }
                Console.WriteLine("连接服务器失败");
            }
        }

        public void service()
        {
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            IPEndPoint point = new IPEndPoint(ip, 50000);
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                socket.Bind(point);
                socket.Listen(10);
                Thread thread = new Thread(AcceptInfo);
                thread.IsBackground = true;
                thread.Start(socket);
            }
            catch(Exception e)
            {
                textBox.Text += e.Message;
            }
        }

        Dictionary<string, Socket> dic = new Dictionary<string, Socket>();

        public void AcceptInfo(object o)
        {
            Socket socket = o as Socket;
            while (true)
            {
                try
                {
                    Socket tSocket = socket.Accept();
                    string point = tSocket.RemoteEndPoint.ToString();
                    Console.WriteLine(point + "连接成功！");
                    item = point;
                    Thread th = new Thread(Receive1);
                    th.IsBackground = true;
                    th.Start(tSocket);
                }
                catch(Exception ex)
                {
                    Console.WriteLine("ex错误："+ex.Message);
                    break;
                }
            }
        }

        public void Receive1(object o)
        {
            Socket dsocket = o as Socket;
            while (true)
            {
                try
                {
                    byte[] buffer = new byte[1024 * 1024];
                    int n = dsocket.Receive(buffer);
                    string words = Encoding.UTF8.GetString(buffer, 0, n);
                    Console.WriteLine(dsocket.RemoteEndPoint.ToString() + ":" + words);
                }
                catch(Exception exc)
                {
                    Console.WriteLine("exc错误:"+exc.Message);
                    break;
                }
            }
        }

        public void send()
        {
            try
            {
                string msg = textBox1.Text;
                byte[] buffer = Encoding.UTF8.GetBytes(msg);
                dic[item].Send(buffer);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void greet()
        {
            IPAddress localip=null;
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress[] ips= Dns.GetHostAddresses(Dns.GetHostName());
            foreach (IPAddress ip in ips)
            {
                if (!ip.IsIPv6SiteLocal)
                    localip = ip;
            }
            IPEndPoint point = new IPEndPoint(localip, 888);
            socket.Connect(point);
            Thread th = new Thread(Receive);
            th.IsBackground = true;
            th.Start();
        }

        public void Receive()
        {
            while (true)
            {
                try
                {
                    byte[] buffer = new byte[1024 * 1024 * 3];
                    int r = socket.Receive(buffer);
                    if (r == 0)
                    {
                        break;
                    }
                    if (buffer[0] == 0)
                    {
                        string s = Encoding.UTF8.GetString(buffer, 1, r - 1);
                        MessageBox.Show(socket.RemoteEndPoint + ":" + s);
                    }
                    else if (buffer[0] == 1)
                    {
                        SaveFileDialog sfd = new SaveFileDialog();
                        sfd.InitialDirectory = @"C:\Users\xu.huang\Desktop";
                        sfd.Title = "请选择要保存的文件";
                        sfd.Filter = "所有文件|*.*";
                        sfd.ShowDialog(this);
                        string path = sfd.FileName;
                        using (FileStream fsWrite = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write))
                        {
                            fsWrite.Write(buffer, 1, r - 1);
                        }
                        MessageBox.Show("保存成功");
                    }
                    else if (buffer[0] == 2)
                    {
                        Zd();
                    }
                }
                catch
                {

                }
            }
        }

        public void Zd()
        {
            
        }

        public void Open()
        {
            Close();
            _sp = new SerialPort();
            _sp.BaudRate = 9600;
            _sp.DataBits = 8;
            _sp.StopBits = System.IO.Ports.StopBits.One;
            _sp.Parity = System.IO.Ports.Parity.None;
            //串口发送间隔较长,设置超时时间时多设点
            _sp.ReadTimeout = 100;
            _sp.WriteTimeout = -1;
            _sp.ReceivedBytesThreshold = 8;
            RegistryKey keyCom = Registry.LocalMachine.OpenSubKey("Hardware\\DeviceMap\\SerialComm");
            if (keyCom != null)
            {
                string[] sSubKeys = keyCom.GetValueNames();
                // 寻找串口
                foreach (string key in sSubKeys)
                {
                    string value = (string)keyCom.GetValue(key);
                    _sp.PortName = value;
                    try
                    {
                        _sp.Open();
                        _sp.Write(Command.No3_Start1, 0, Command.No3_Start1.Length);
                        Thread.Sleep(1000);
                        _sp.Read(Command.rtcmd_McuVerQr, 0, Command.rtcmd_McuVerQr.Length);
                        break;
                    }
                    //catch (TimeoutException ex)
                    //{
                    //    MessageBox.Show(ex.Message);
                    //}
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message);
                    }
                }
            }
            if (!_sp.IsOpen)
            {
                throw new Exception("串口未连接");
            }
        }

        public static class Command
        {
            #region 3号产测
            //进入产测(亮屏提示、最大功率，不开PTC):
            public static byte[] No3_Start = new byte[] { 0xFE, 0x02, 0x14, 0x02, 0x33, 0x0D, 0x1D };

            //接收返回数据
            public static byte[] rtcmd_No3 = new byte[134];

            public static byte[] rtcmd_McuVerQr = new byte[100];

            public static byte[] rtcmd_No3Com = new byte[] { 0xFE, 0x04, 0x00, 0x03, 0x00, 0x01, 0xD5, 0xC5 };

            //进入产测(亮屏提示、最大功率，不开PTC)
            public static byte[] No3_Start1 = new byte[] { 0xFE, 0x02, 0x15, 0x02, 0x33, 0x5C, 0xDD };

            //产测开PTC(不亮屏提示、最大功率，开PTC)
            public static byte[] No3_OpenPtc = new byte[] { 0xFE, 0x02, 0x16, 0x02, 0x33, 0xAC, 0xDD };

            //产测关PTC(不亮屏提示、最大功率，关闭PTC)
            public static byte[] No3_ONPtc = new byte[] { 0xFE, 0x02, 0x17, 0x02, 0x33, 0xFD, 0x1D };
            #endregion
        }

        private Socket serverSock;
        private byte[] msgBuff = new byte[50];
        private int[,] grid = new int[4, 4];
        public int GetGrid(int x, int y)
        {
            return grid[x, y];
        }

        public void SetGrid(int x, int y, int value)
        {
            grid[x, y] = value;
        }

        public void Start(int port)
        {
            // Create the listener socket in this machines IP address

            serverSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSock.Bind(new IPEndPoint(IPAddress.Loopback, port));	// For use with localhost 127.0.0.1
            serverSock.Listen(10);

            // Setup a callback to be notified of connection requests
            serverSock.BeginAccept(new AsyncCallback(OnConnectRequest), serverSock);

            Console.WriteLine("*** Grid Server {0} Started {1} *** ", port, DateTime.Now.ToString("G"));
        }

        public void OnConnectRequest(IAsyncResult ar)
        {
            Socket listener = (Socket)ar.AsyncState;
            NewConnection(listener.EndAccept(ar));
            listener.BeginAccept(new AsyncCallback(OnConnectRequest), listener);
        }

        public void NewConnection(Socket clientSock)
        {
            // Program blocks on Accept() until a client connects.
            // SocketChatClient client = new SocketChatClient( listener.AcceptSocket() );
            // SocketChatClient client = new SocketChatClient(sockClient);
            // m_aryClients.Add(client);
            Console.WriteLine("Client {0}, joined", clientSock.RemoteEndPoint);

            String connectedMsg = "Connected to " + clientSock.LocalEndPoint + "success \n\r";
            // Convert to byte array and send.
            Byte[] byteMsg = System.Text.Encoding.ASCII.GetBytes(connectedMsg.ToCharArray());
            clientSock.Send(byteMsg, byteMsg.Length, 0);

            SetupRecieveCallback(clientSock);
        }

        public void SetupRecieveCallback(Socket sock)
        {
            try
            {
                AsyncCallback recieveData = new AsyncCallback(OnRecievedData);
                sock.BeginReceive(msgBuff, 0, msgBuff.Length, SocketFlags.None, recieveData, sock);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Recieve callback setup failed! {0}", ex.Message);
            }
        }

        public void OnRecievedData(IAsyncResult ar)
        {
            Socket sock = (Socket)ar.AsyncState;
            // Check if we got any data
            try
            {
                int nBytesRec = sock.EndReceive(ar);
                if (nBytesRec > 0)
                {
                    // Get the received message 
                    string sRecieved = Encoding.ASCII.GetString(msgBuff, 0, nBytesRec);
                    // Process it
                    ProcessMessage(sock, sRecieved);

                    SetupRecieveCallback(sock);
                }
                else
                {
                    // If no data was recieved then the connection is probably dead
                    Console.WriteLine("disconnect from server {0}", sock.RemoteEndPoint);
                    sock.Shutdown(SocketShutdown.Both);
                    sock.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message, "Unusual error druing Recieve!");
            }
        }

        public void ProcessMessage(Socket sock, string msg)
        {
            string[] messages = msg.Split(' ');
            int count = messages.Length;
            if (count > 0)
            {
                string first = messages[0].ToLower();
                //if client want get a grid value 
                if (first == "get" && (count == 3))
                {
                    string answer = GetGrid(int.Parse(messages[1]), int.Parse(messages[2])).ToString();
                    answer = string.Format("Grid [{0}][{1}] ={2}", messages[1], messages[2], answer);
                    Byte[] byteAnswer = System.Text.Encoding.ASCII.GetBytes(answer.ToCharArray());
                    // send back the value of corresponding grid 
                    sock.Send(byteAnswer);
                }
                else
                {
                    //if client want set a grid value
                    if (first == "set" && (count == 4))
                    {
                        SetGrid(int.Parse(messages[1]), int.Parse(messages[2]), int.Parse(messages[3]));
                    }
                    else
                    {
                        Console.WriteLine(msg);
                    }
                }
            }
        }

        #region 事件
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                set();
            });
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            Start(50000);
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            send();
        }
        #endregion

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            NewServer.NewServer s = new NewServer.NewServer();
            s.Show();
            this.Close();
        }
    }
}
