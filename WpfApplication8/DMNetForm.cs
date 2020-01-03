using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApplication8
{
    public class DMNetForm
    {
        public DMNetForm()
        {

        }

        public DMNetForm(int part)
        {

        }

        Dictionary<string, object> fromdic = new Dictionary<string, object>();
        public Dictionary<string, object> GetSendObj { get { return fromdic; } }
    }
}
