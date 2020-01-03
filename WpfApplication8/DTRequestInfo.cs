using Newtonsoft.Json;
using SuperSocket.SocketBase.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApplication8
{
    public class DTRequestInfo : IRequestInfo
    {

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="key">键值</param>
        public DTRequestInfo(string key)
        {
            this.Key = key;
            ResponseY response = dealResponse<object>(key);
            this.cmd = response.cmd;
            this.node = response.node;
        }

        /// <summary>
        /// 设备ID
        /// </summary>
        public string node { get; set; }

        public int cmd { get; set; }

        /// <summary>
        /// 请求信息缓存
        /// </summary>
        public string Key { get; set; }

        private static ResponseY dealResponse<T>(string json)
        {
            ResponseY response = null;
            try
            {
                response = JsonConvert.DeserializeObject<ResponseY>(json);
            }
            catch (Exception)
            {

            }
            return response;
        }
    }
}
