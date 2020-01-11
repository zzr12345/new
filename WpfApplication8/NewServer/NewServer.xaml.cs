using Newtonsoft.Json.Linq;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Config;
using SuperSocket.SocketBase.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfApplication8.NewServer
{
    /// <summary>
    /// NewServer.xaml 的交互逻辑
    /// </summary>
    public partial class NewServer : Window
    {
        public static NewServer instance;
        public SolidColorBrush Green = new SolidColorBrush(Colors.ForestGreen);
        public SolidColorBrush Red = new SolidColorBrush(Colors.Red);
        public SolidColorBrush PaleGreen = new SolidColorBrush(Colors.PaleGreen);
        public SolidColorBrush IndianRed = new SolidColorBrush(Colors.IndianRed);

        //存储session和对应ip端口号的泛型集合
        Dictionary<string, DTSession> sessionList = new Dictionary<string, DTSession>();
        //存储位置信息和对应的ip端口号的泛型集合
        Dictionary<string, string> AddressList = new Dictionary<string, string>();
        //存储左侧容器中控件对应编号的泛型集合
        Dictionary<int, NewCheckBox> sNumL = new Dictionary<int, NewCheckBox>();
        //存储右侧容器中控件对应编号的泛型集合
        Dictionary<int, NewCheckBox> sNumR = new Dictionary<int, NewCheckBox>();
        //存储左侧编号和对应的控件value的泛型集合
        Dictionary<string, int> stNumL = new Dictionary<string, int>();
        //存储右侧编号和对应的控件value的泛型集合
        Dictionary<string, int> stNumR = new Dictionary<string, int>();

        Dictionary<int, string> ipList = new Dictionary<int, string>();
        //存储图片名称和对应打的url的泛型集合
        Dictionary<string, string> imageList = new Dictionary<string, string>();
        //存储视频名称和对应打的url的泛型集合
        Dictionary<string, string> videoList = new Dictionary<string, string>();

        Dictionary<string, string> contectList = new Dictionary<string, string>();
        //存储左侧添加的控件
        List<NewCheckBox> NcbL = new List<NewCheckBox>();
        //存储右侧添加的控件
        List<NewCheckBox> NcbR = new List<NewCheckBox>();


        //存储控件位置
        List<Thickness> Tkn = new List<Thickness>();

        string baseUrl = "http://new-service.oss-cn-hangzhou.aliyuncs.com/";

        public int inWidth;
        public int inHeighL;
        public int inHeighR;
        public int leftAll;
        public int rightAll;

        public double ShowHeigh;
        public double ShowWidthL;
        public double ShowHeighR;
        public int i2 = 0;

        #region Socket连接参数
        DTServer appServer;
        ServerConfig serverConfig;
        //创建连接的IP地址
        string ipAddress_Connect;
        //接收消息的IP地址
        string ipAddress_Receive;
        //断开连接的IP地址
        string ipAddress_Close;
        //string ip = "127.0.0.1";
        //端口号
        int port = 50001;
        #endregion

        public NewServer()
        {
            InitializeComponent();

            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            // 设置全屏
            WindowState = WindowState.Maximized;
            // 窗口固定大小
            ResizeMode = ResizeMode.NoResize;
            Title = "产线投屏系统";
            instance = this;
            var ell = Lib.Http.GetchnList();
            foreach (JObject E2 in ell.content)
            {
                comboBox10.Items.Add(E2["data"]["5df2fff7051d095a7e45f70e"]);
                contectList.Add(E2["data"]["5df2fff7051d095a7e45f70e"].ToString(), E2["id"].ToString());
            }
            comboBox.IsEnabled = false;
            button2.IsEnabled = false;
            //for(int i = 0; i < 20; i++)
            //{
            //    Projects.Add(new Project(i.ToString()+"-"+i.ToString(), "1-1.JPG"));
            //}
        }

        #region 控制事件
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ShowHeigh = scroll.ActualHeight;
            ShowWidthL = scroll.ActualWidth;
            ShowHeighR = scrollR.ActualHeight;
            gridLeft.Height = scroll.ActualHeight;
            gridRight.Height = scrollR.ActualHeight;
            gridLeft.Width = gridRight.Width = scroll.ActualWidth;
            textBlock.Text = "未开启";
            textBlock.Background = Red;
            inWidth = (int)(gridLeft.Width) / (154 + 20);
            inHeighL = (int)(gridLeft.Height) / (84 + 20);
            inHeighR = (int)(gridRight.Height) / (84 + 20);
            leftAll = inWidth * inHeighL;
            rightAll = inWidth * inHeighR;
            for (int i = 0; i < 100; i++)
            {
                Thickness T = new Thickness(20 + i % inWidth * 174, 20 + i / inWidth * 104, 0, 0);
                Tkn.Add(T);
            }
            //            var host = Dns.GetHostName();
            //#pragma warning disable 618
            //            var localHost = Dns.GetHostByName(host);
            //#pragma warning restore 618
            //            foreach (IPAddress E in localHost.AddressList)
            //            {
            //                var xx = E.AddressFamily;
            //                comboBox.Items.Add(E);
            //            }
            //            int iz = 0;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult m = MessageBox.Show("是否要关闭并退出投屏系统", "退出", MessageBoxButton.YesNo, MessageBoxImage.Information);
            if (m == MessageBoxResult.Yes)
            {

            }
            else
            {
                e.Cancel = true;
            }
        }

        /// <summary>
        /// 服务器开关
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void On_btn_Click(object sender, RoutedEventArgs e)
        {
            if (On_btn.Content.ToString() == "开启服务")
            {
                Init();
            }
            else
            {
                textBlock.Text = "未开启";
                textBlock.Background = Red;
                On_btn.Content = "开启服务";

                appServer.Stop();
                Thread.Sleep(100);
                sessionList.Clear();
                AddressList.Clear();
                sNumL.Clear();
                sNumR.Clear();
                stNumL.Clear();
                stNumR.Clear();
                NcbL.Clear();
                NcbR.Clear();
                gridLeft.Children.Clear();
                gridRight.Children.Clear();
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            string result = null;
            List<int> inn = new List<int>();
            inn.AddRange(sNumR.Keys);
            gridRight.Children.Clear();
            gridRight.Height = ShowHeighR;

            if (comboBox.SelectionBoxItem.ToString() == "")
            {
                MessageBox.Show("请选择要投影的内容");
                return;
            }

            if (inn.Count == 0)
            {
                MessageBox.Show("请选择要发送的客户端");
                return;
            }
            string thi = comboBox.SelectionBoxItem.ToString();
            if (imageList.Keys.Contains(thi))
            {
                result = "{\"cmd\":61,\"name\":\"" + thi + "\",\"url\":\"" + baseUrl + imageList[thi] + "\"}";
            }
            else if (videoList.Keys.Contains(thi))
            {
                result = "{\"cmd\":63,\"name\":\"" + thi + "\",\"url\":\"" + baseUrl + videoList[thi] + "\"}";
            }

            foreach (int es in inn)
            {
                sendSth(sessionList[ipList[es]], result);
            }
            int i = 0;
            foreach (NewCheckBox n in NcbR)
            {
                n.textBlock1.Content = comboBox.SelectionBoxItem.ToString();
                n.grid.Background = IndianRed;
                AddCheckBoxLeftRes(inn[i], n);
                i++;
            }
            NcbR.Clear();
            sNumR.Clear();
        }

        int ui = 1;
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            string ss="";
            if (ui == 1)
            {
                ss = "3-8";
            }
            if (ui == 2)
            {
                ss = "8-2";
            }
            if (ui == 3)
            {
                ss = "1-1";
            }
            AddCheckBoxLeft(ss);
            ui++;
            //gridRight.Children.Clear();
            //gridRight.Height = ShowHeighR;
            //string result = "{\"cmd\":64}";
            //List<int> inn = new List<int>();
            //inn.AddRange(sNumR.Keys);
            //int i = 0;
            //foreach (int es in inn)
            //{
            //    sendSth(sessionList[ipList[es]], result);
            //}
            //foreach (NewCheckBox n in NcbR)
            //{
            //    n.textBlock1.Content = "(空闲)";
            //    n.grid.Background = PaleGreen;
            //    AddCheckBoxLeftRes(inn[i], n);
            //    i++;
            //}
            //NcbR.Clear();
            //sNumR.Clear();
        }
        #endregion

        #region 监听事件
        public void Init()
        {
            appServer = new DTServer();
            serverConfig = new ServerConfig
            {
                //Ip = ip
                Port = port
            };
            if (!appServer.Setup(serverConfig))
            {
                MessageBox.Show("初始化失败！");
                return;
            }
            if (!appServer.Start())
            {
                MessageBox.Show("服务器启动失败！");
                return;
            }
            AddressList.Clear();
            textBlock.Text = "运行中";
            textBlock.Background = Green;
            On_btn.Content = "关闭服务";
            appServer.NewSessionConnected += appServer_NewSessionConnected;
            appServer.NewRequestReceived += appServer_NewRequestReceived;
            appServer.SessionClosed += appServer_SessionClosed;
        }

        public void appServer_NewSessionConnected(DTSession session)
        {
            var host = session.LocalEndPoint;
            ipAddress_Connect = session.RemoteEndPoint.ToString();
            sessionList.Add(ipAddress_Connect, session);
        }

        List<string> rList = new List<string>();
        string l;
        public void appServer_NewRequestReceived(DTSession session, DTRequestInfo requestInfo)
        {
            //requestInfo.cmd 接收的数据头
            //requestInfo.Body 接收的内容
            try
            {
                int cmd = requestInfo.cmd;
                string node = requestInfo.node;
                ipAddress_Receive = session.RemoteEndPoint.ToString();
                if (cmd == 84)
                {
                    if (isNum(node.Substring(0, 1)) && isNum(node.Last().ToString()))
                    {
                        if (AddressList.Values.Contains(node))
                        {
                            string resultNew = "{\"cmd\":86}";
                            sendSth(sessionList[ipAddress_Receive], resultNew);
                            return;
                        }
                        AddressList.Add(ipAddress_Receive, node);
                        string[] s = node.Split('-');//用-进行分割
                        l = s[0];
                        rList.Add(s[1]);
                        int Num = Convert.ToInt32(s[0]) * 100 + Convert.ToInt32(s[1]);//转换
                        ipList.Add(Num, ipAddress_Receive);
                        AddCheckBoxLeft(node);
                        string result = "{\"cmd\":85}";
                        sendSth(sessionList[ipAddress_Receive], result);
                    }
                }
                else if (cmd == 127)
                {
                    string result = "{\"cmd\":128}";
                    sendSth(sessionList[ipAddress_Receive], result);
                }
                else if (cmd == 67)
                {
                    if (AddressList.Keys.Contains(ipAddress_Close))
                    {
                        RemoveCheckBox(AddressList[ipAddress_Close]);
                        AddressList.Remove(ipAddress_Close);
                    }
                }
                else if (cmd == 87)
                {
                    if (isNum(node.Substring(0, 1)) && isNum(node.Last().ToString()))
                    {
                        RemoveCheckBox(AddressList[ipAddress_Receive]);
                        if (AddressList.Values.Contains(node))
                        {
                            string resultNew = "{\"cmd\":86}";
                            sendSth(sessionList[ipAddress_Receive], resultNew);
                            return;
                        }
                        string[] s = node.Split('-');//用-进行分割
                        int Num = Convert.ToInt32(s[0]) * 100 + Convert.ToInt32(s[1]);//转换
                        ipList.Add(Num, ipAddress_Receive);
                        AddCheckBoxLeft(node);
                        AddressList[ipAddress_Receive] = node;
                        string result = "{\"cmd\":85}";
                        sendSth(sessionList[ipAddress_Receive], result);
                    }
                }
            }
            catch (Exception E)
            {
                MessageBox.Show(E.Message);
            }
        }

        public void appServer_SessionClosed(DTSession session, CloseReason value)
        {
            try
            {
                ipAddress_Close = session.RemoteEndPoint.ToString();
                sessionList.Remove(ipAddress_Close);
                if (AddressList.Keys.Contains(ipAddress_Close))
                {
                    string na = AddressList[ipAddress_Close];
                    string[] sp = na.Split('-');
                    rList.Remove(sp[1]);
                    RemoveCheckBox(AddressList[ipAddress_Close]);
                    AddressList.Remove(ipAddress_Close);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            CheckBox E = sender as CheckBox;
            var n = E.Name;
            var str = e.Source.ToString();
            if (str.Contains("True"))
            {
                int ss = stNumL[n.ToString()];
                NewCheckBox nc = sNumL[ss];
                var ne = nc.textBlock.Text;
                RemoveCheckBoxLeft(ss);
                AddCheckBoxRight(ss, nc);
            }
            else if (str.Contains("False"))
            {
                int ns = stNumR[n.ToString()];
                NewCheckBox nc = sNumR[ns];
                RemoveCheckBoxRight(ns);
                AddCheckBoxLeftRes(ns, nc);
            }
        }

        List<string> neirongList = new List<string>();
        List<string> vList = new List<string>();

        private void comboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            comboBox.IsEnabled = true;
            button2.IsEnabled = true;
            comboBox.Items.Clear();
            imageList.Clear();
            videoList.Clear();
            neirongList.Clear();
            vList.Clear();
            string id = comboBox10.SelectedItem.ToString();
            var eq = Lib.Http.GetList(contectList[id]);
            if (eq.code != 0)
            {
                MessageBox.Show(eq.msg);
                return;
            }
            var f = eq.data.Values();
            foreach (object o in f)
            {
                if (!(o is JArray)) continue;
                if (!o.ToString().Contains("sizeStr")) continue;
                JArray t = (JArray)o;
                foreach (JObject E1 in t)
                {
                    if (E1["type"].ToString().Contains("image"))
                    {
                        neirongList.Add(E1["name"].ToString());
                        imageList.Add(E1["name"].ToString(), E1["path"].ToString());
                    }
                    else if (E1["type"].ToString().Contains("video"))
                    {
                        vList.Add(E1["name"].ToString());
                        videoList.Add(E1["name"].ToString(), E1["path"].ToString());
                    }
                }
                neirongList.Sort(new CustomComparer());
                vList.Sort(new CustomComparer());
                foreach (string a in neirongList)
                {
                    comboBox.Items.Add(a);
                }
                foreach (string a in vList)
                {
                    comboBox.Items.Add(a);
                }
            }
        }
        #endregion

        public static void sendSth(DTSession session, string thing)
        {
            byte[] rsp = Encoding.UTF8.GetBytes(thing);
            byte[] re = new byte[] { Convert.ToByte(rsp.Length) };
            byte[] result = new byte[rsp.Length + 4];
            re.CopyTo(result, 3);
            rsp.CopyTo(result, 4);
            session.Send(result, 0, result.Length);
        }

        public bool isNum(string num)
        {
            if (num == null || num.Length == 0)
                return false;
            ASCIIEncoding ascii = new ASCIIEncoding();
            //把string类型的参数保存到数组里
            byte[] bytestr = ascii.GetBytes(num);
            //遍历这个数组里的内容
            foreach (byte c in bytestr)
            {
                //判断是否为数字
                if (c < 48 || c > 57)
                {
                    return false;
                }
            }
            return true;
        }

        //在线程中显示提示消息
        public void ShowMsg(string msg)
        {
            instance.Dispatcher.Invoke((Action)delegate ()
            {
                MessageBox.Show(msg);
            });
        }

        public void RemoveCheckBox(string body)
        {
            string[] s = body.Split('-');//用-进行分割
            int Num = Convert.ToInt32(s[0]) * 100 + Convert.ToInt32(s[1]);//转换
            ipList.Remove(Num);
            if (sNumL.Keys.Contains(Num))
            {
                RemoveCheckBoxLeft(Num);
            }
            else if (sNumR.Keys.Contains(Num))
            {
                RemoveCheckBoxRight(Num);
            }
        }

        Dictionary<string, NewCheckBox> gdsk = new Dictionary<string, NewCheckBox>();

        #region 控件操作
        public void AddCheckBoxLeft(string body)
        {
            string[] s = body.Split('-');//用-进行分割
            int Num = Convert.ToInt32(s[0]) * 100 + Convert.ToInt32(s[1]);//转换
            instance.Dispatcher.Invoke((Action)delegate ()
            {
                NewCheckBox n1 = new NewCheckBox();
                n1.Height = 84;
                n1.Width = 154;
                n1.checkBox.Name = "a" + i2.ToString();
                n1.textBlock.Text = body;
                n1.grid.Background = PaleGreen;

                n1.HorizontalAlignment = HorizontalAlignment.Left;
                n1.VerticalAlignment = VerticalAlignment.Top;
                //if (104 + i1 / inWidth * 104 > instance.gridLeft.Height)
                //{
                //    instance.gridLeft.Height += 104;
                //}
                //n1.Margin = new Thickness(20 + i1 % intel * 174, 20 + i1 / intel * 104, instance.gridLeft.ActualWidth - 174 - i1 % intel * 174, instance.gridLeft.ActualHeight - 104 - i1 / intel * 104);
                //n1.Margin = new Thickness(20 + i1 % inWidth * 174, 20 + i1 / inWidth * 104, 0, 0);
                //n1.MouseEnter += new System.Windows.Input.MouseEventHandler(Mouse_Enter);
                n1.checkBox.Click += new RoutedEventHandler(CheckBox_Click);

                sNumL.Add(Num, n1);
                sNumL = sNumL.OrderBy(p => p.Key).ToDictionary(p => p.Key, o => o.Value);
                gdsk.Add(body, n1);
                gdsk=gdsk.OrderBy(p => p.Key, new CustomComparer()).ToDictionary(p => p.Key, o => o.Value);
                NcbL.Clear();
                gridLeft.Children.Clear();
                NcbL.AddRange(sNumL.Values);
                if (NcbL.Count > leftAll)
                {
                    int count = NcbL.Count - leftAll;
                    int bel = (count + 4) / inWidth;
                    gridLeft.Height = ShowHeigh + bel * 104;
                }
                for (int i = 0; i < NcbL.Count; i++)
                {
                    NcbL[i].Margin = Tkn[i];
                    gridLeft.Children.Add(NcbL[i]);
                }
                stNumL.Add("a" + i2.ToString(), Num);
                i2++;
            });
        }

        public void AddCheckBoxLeftRes(int Num, NewCheckBox nc)
        {
            nc.checkBox.Name = "a" + i2.ToString();
            nc.checkBox.IsChecked = false;
            sNumL.Add(Num, nc);
            sNumL = sNumL.OrderBy(p => p.Key).ToDictionary(p => p.Key, o => o.Value);
            NcbL.Clear();
            gridLeft.Children.Clear();
            NcbL.AddRange(sNumL.Values);
            if (NcbL.Count > leftAll)
            {
                int count = NcbL.Count - leftAll;
                int bel = (count + 4) / inWidth;
                gridLeft.Height = ShowHeigh + bel * 104;
            }
            for (int i = 0; i < NcbL.Count; i++)
            {
                NcbL[i].Margin = Tkn[i];
                gridLeft.Children.Add(NcbL[i]);
            }
            stNumL.Add("a" + i2.ToString(), Num);
            i2++;
        }

        public void RemoveCheckBoxLeft(int Num)
        {
            instance.Dispatcher.Invoke((Action)delegate ()
            {
                sNumL.Remove(Num);
                NcbL.Clear();
                instance.gridLeft.Children.Clear();
                NcbL.AddRange(sNumL.Values);
                if (NcbL.Count >= leftAll && NcbL.Count % inWidth == 0)
                {
                    gridLeft.Height -= 104;
                }
                for (int i = 0; i < NcbL.Count; i++)
                {
                    NcbL[i].Margin = Tkn[i];

                    instance.gridLeft.Children.Add(NcbL[i]);
                }
            });
        }

        public void AddCheckBoxRight(int Num, NewCheckBox n)
        {
            n.checkBox.Name = "a" + i2.ToString();
            n.checkBox.IsChecked = true;
            sNumR.Add(Num, n);
            sNumR = sNumR.OrderBy(p => p.Key).ToDictionary(p => p.Key, o => o.Value);
            instance.Dispatcher.Invoke((Action)delegate ()
            {
                NcbR.Clear();
                gridRight.Children.Clear();
                NcbR.AddRange(sNumR.Values);
                for (int i = 0; i < NcbR.Count; i++)
                {
                    NcbR[i].Margin = Tkn[i];
                    gridRight.Children.Add(NcbR[i]);
                }
                if (NcbR.Count > rightAll && (NcbR.Count - 1) % inWidth == 0)
                {
                    instance.gridRight.Height += 104;
                }
                stNumR.Add("a" + i2.ToString(), Num);
                i2++;
            });
        }


        public void RemoveCheckBoxRight(int Num)
        {
            instance.Dispatcher.Invoke((Action)delegate ()
            {
                sNumR.Remove(Num);
                NcbR.Clear();
                instance.gridRight.Children.Clear();
                NcbR.AddRange(sNumR.Values);
                if (NcbR.Count >= rightAll && NcbR.Count % inWidth == 0)
                {
                    gridRight.Height -= 104;
                }
                for (int i = 0; i < NcbR.Count; i++)
                {
                    NcbR[i].Margin = Tkn[i];
                    instance.gridRight.Children.Add(NcbR[i]);
                }
            });
        }
        #endregion

        #region 临时1
        private void textBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key != System.Windows.Input.Key.Enter) return;
            if (textBox.Text == null)
            {
                MessageBox.Show("请输入发送内容");
                return;
            }
            List<int> inn = new List<int>();
            inn.AddRange(sNumR.Keys);
            if (inn.Count == 0)
            {
                MessageBox.Show("未选择要发送的客户端");
                return;
            }
            gridRight.Children.Clear();
            gridRight.Height = ShowHeighR;

            int i = 0;
            foreach (int es in inn)
            {
                sendSth(sessionList[ipList[es]], textBox.Text);
            }
            foreach (NewCheckBox n in NcbR)
            {
                n.textBlock1.Content = comboBox.SelectionBoxItem.ToString();
                n.grid.Background = IndianRed;
                AddCheckBoxLeftRes(inn[i], n);
                i++;
            }
            NcbR.Clear();
            sNumR.Clear();
        }
        #endregion

        #region 临时2
        public void timeTest()
        {
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Enabled = true;
            timer.Interval = 600000;
            timer.Start();
            timer.Elapsed += new System.Timers.ElapsedEventHandler(getss);
        }

        private void getss(object source, System.Timers.ElapsedEventArgs e)
        {
            Console.WriteLine("");
        }
        #endregion

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            if (gridRight.Children.Count != 0)
            {
                MessageBox.Show("请取消已选中的节点");
                return;
            }
            string result;
            for (int i = 0; i < rList.Count; i++)
            {
                foreach (string s in neirongList)
                {
                    if (s.Contains("-" + rList[i]))
                    {
                        int Num = Convert.ToInt32(l) * 100 + Convert.ToInt32(rList[i]);
                        result = "{\"cmd\":61,\"name\":\"" + s + "\",\"url\":\"" + baseUrl + imageList[s] + "\"}";
                        sendSth(sessionList[ipList[Num]], result);
                    }
                }
            }
            int j = 0;
            foreach (NewCheckBox n in gridLeft.Children)
            {
                n.grid.Background = IndianRed;
                n.textBlock1.Content = neirongList[j];
                j++;
            }
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            List<NewCheckBox> nl = new List<NewCheckBox>();
            nl.AddRange(NcbL);
            foreach (NewCheckBox n in nl)
            {
                int num = stNumL[n.checkBox.Name];
                RemoveCheckBoxLeft(num);
                AddCheckBoxRight(num, n);
            }
        }

        private void button4_Click(object sender, RoutedEventArgs e)
        {
            List<NewCheckBox> nl = new List<NewCheckBox>();
            nl.AddRange(NcbR);
            foreach (NewCheckBox n in nl)
            {
                int num = stNumR[n.checkBox.Name];
                RemoveCheckBoxRight(num);
                AddCheckBoxLeftRes(num, n);
            }
        }
    }

    class CustomComparer : IComparer<object>
    {
        public int Compare(object x, object y)
        {
            if (x == null || y == null)
                throw new ArgumentException("参数不能为空");
            string fileA = x as string;
            string fileB = y as string;
            char[] arr1 = fileA.ToCharArray();
            char[] arr2 = fileB.ToCharArray();
            int i = 0, j = 0;
            while (i < arr1.Length && j < arr2.Length)
            {
                if (char.IsDigit(arr1[i]) && char.IsDigit(arr2[j]))
                {
                    string s1 = "", s2 = "";
                    while (i < arr1.Length && char.IsDigit(arr1[i]))
                    {
                        s1 += arr1[i];
                        i++;
                    }
                    while (j < arr2.Length && char.IsDigit(arr2[j]))
                    {
                        s2 += arr2[j];
                        j++;
                    }
                    if (int.Parse(s1) > int.Parse(s2))
                    {
                        return 1;
                    }
                    if (int.Parse(s1) < int.Parse(s2))
                    {
                        return -1;
                    }
                }
                else
                {
                    if (arr1[i] > arr2[j])
                    {
                        return 1;
                    }
                    if (arr1[i] < arr2[j])
                    {
                        return -1;
                    }
                    i++;
                    j++;
                }
            }
            if (arr1.Length == arr2.Length)
            {
                return 0;
            }
            else
            {
                return arr1.Length > arr2.Length ? 1 : -1;
            }
        }
    }
}
