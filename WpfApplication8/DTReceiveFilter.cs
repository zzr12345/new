using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApplication8
{
    /// <summary>
    /// 数据接收过滤器
    /// </summary>
    public class DTReceiveFilter : ReceiveFilterHelper<DTRequestInfo>
    {
        /// <summary>
        /// 重写方法
        /// </summary>
        /// <param name="readBuffer">过滤之后的数据缓存</param>
        /// <param name="offset">数据起始位置</param>
        /// <param name="length">数据缓存长度</param>
        /// <returns></returns>
        protected override DTRequestInfo ProcessMatchedRequest(byte[] readBuffer, int offset, int length)
        {
            byte[] newBuffer = new byte[readBuffer.Length - 4];
            Array.Copy(readBuffer, 4, newBuffer, 0, readBuffer.Length - 4);
            //返回构造函数指定的数据格式
            return new DTRequestInfo(Encoding.UTF8.GetString(newBuffer));
        }
    }
}
