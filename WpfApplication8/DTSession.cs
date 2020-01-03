using SuperSocket.SocketBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApplication8
{
    public class DTSession : AppSession<DTSession, DTRequestInfo>
    {

        protected override void HandleException(Exception e)
        {
            base.HandleException(e);
        }

        protected override void OnSessionStarted()
        {
            base.OnSessionStarted();
        }
        protected override int GetMaxRequestLength()
        {
            return base.GetMaxRequestLength();
        }

        protected override void HandleUnknownRequest(DTRequestInfo requestInfo)
        {
            base.HandleUnknownRequest(requestInfo);
        }
    }
}
