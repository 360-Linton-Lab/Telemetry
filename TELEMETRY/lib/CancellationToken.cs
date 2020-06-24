//#if NET_3_5
namespace TELEMETRY.lib
{
    public class CancellationToken
    {
        public static CancellationToken None { get; } = new CancellationToken();

        private volatile bool isCancelled;

        public bool IsCancellationRequested => isCancelled;

        public void Cancel() => isCancelled = true;
    }
}
//#endif
