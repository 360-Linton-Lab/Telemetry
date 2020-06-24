using TELEMETRY.Domain;
using System.Security.Principal;
using System;
using TaskScheduler;

namespace TELEMETRY
{
    class Program
    {
        public static bool IsHighIntegrity()
        {
            WindowsIdentity current = WindowsIdentity.GetCurrent();
            WindowsPrincipal windowsPrincipal = new WindowsPrincipal(current);
            return windowsPrincipal.IsInRole(WindowsBuiltInRole.Administrator);
        }



        static void Main(string[] args)
        {
            try
            {
                if (!IsHighIntegrity())
                {
                    Console.WriteLine("\n[X] Not in high integrity, Unable to Telemetry!\n");
                    System.Environment.Exit(0);
                }


                // try to parse the command line arguments, show usage on failure and then bail
                var parsed = ArgumentParser.Parse(args);
                if (parsed.ParsedOk == false)
                    Info.ShowUsage();
                else
                {
                    // Try to execute the command using the arguments passed in

                    var commandName = args.Length != 0 ? args[0] : "";

                    var commandFound = new CommandCollection().ExecuteCommand(commandName, parsed.Arguments);

                    // show the usage if no commands were found for the command name
                    if (commandFound == false)
                        Info.ShowUsage();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("\r\n[!] Unhandled TELEMETRY exception:\r\n");
                Console.WriteLine(e);
            }
        }
    }
}
