using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApplication8
{
    public class Response<T>
    {
        public int code;
        public string msg;
        public T data;
        public T content;
    }

    public class ResponseY
    {
        public int cmd;
        public string node;
    }
}
