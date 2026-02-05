using Microsoft.Win32.TaskScheduler;
using System;

namespace SteamAccountChange.Common
{
    /// <summary>
    /// 计划任务帮助类
    /// </summary>
    public static class TaskSchedulerHelper
    {
        /// <summary>
        /// 删除任务
        /// </summary>
        /// <param name="taskName"></param>
        public static void Del(string taskName)
        {
            try
            {
                using (var ts = new TaskService())
                {
                    ts.RootFolder.DeleteTask(taskName, exceptionOnNotExists: false);
                }
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// 获取任务
        /// </summary>
        /// <param name="taskName"></param>
        /// <returns></returns>
        public static Task Get(string taskName)
        {
            using (var ts = new TaskService())
            {
                return ts.FindTask(taskName);
            }
        }

        /// <summary>
        /// 添加启动任务
        /// </summary>
        /// <param name="taskName"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool AddLuanchTask(string taskName, string path)
        {
            try
            {
                using (var ts = new TaskService())
                {
                    var existingTask = ts.FindTask(taskName);
                    if (existingTask != null)
                    {
                        ts.RootFolder.DeleteTask(taskName);
                    }

                    var td = ts.NewTask();
                    td.RegistrationInfo.Author = Local.AppName;
                    td.RegistrationInfo.Description = Local.AppName;
                    td.Principal.RunLevel = TaskRunLevel.Highest;

                    // 触发类型: 登录时
                    td.Triggers.Add(new LogonTrigger());

                    // 动作: 执行程序
                    td.Actions.Add(new ExecAction(path));

                    // 设置
                    td.Settings.ExecutionTimeLimit = TimeSpan.Zero;
                    td.Settings.DisallowStartIfOnBatteries = false;
                    td.Settings.RunOnlyIfIdle = false;

                    ts.RootFolder.RegisterTaskDefinition(taskName, td, TaskCreation.CreateOrUpdate, null, null, TaskLogonType.InteractiveToken);
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}