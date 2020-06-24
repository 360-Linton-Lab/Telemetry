//#if NET_3_5

namespace TELEMETRY.lib
{
    public class CancellationTokenSource
    {
        public CancellationToken Token { get; private set; }

        public CancellationTokenSource()
        {
            Token = new CancellationToken();
        }

        public void Cancel()
        {
            Token.Cancel();
        }
    }
}
//#endif

