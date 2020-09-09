using SteamAccountChange.Common;
using SteamAccountChange.View;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows;

namespace SteamAccountChange
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// 应用启动
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // 如果存在，直接返回
            if (ExistsCurrentProcess())
            {
                System.Environment.Exit(0);
            }

            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
            Notify.Init();
        }

        /// <summary>
        /// 是否存在当前进程
        /// </summary>
        private static bool ExistsCurrentProcess()
        {
            var currentProcess = Process.GetCurrentProcess();

            var processList = Process.GetProcesses();
            foreach (Process item in processList)
            {
                if (item.ProcessName.ToLower() == currentProcess.ProcessName.ToLower() && item.Id != currentProcess.Id)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 当找不到程序集的时候，从嵌入的资源找
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        /// <returns></returns>
        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs e)
        {
            string resourceName = "SteamAccountChange." + new AssemblyName(e.Name).Name + ".dll";
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            {
                byte[] assemblyData = new byte[stream.Length];
                stream.Read(assemblyData, 0, assemblyData.Length);
                return Assembly.Load(assemblyData);
            }
        }
    }
}