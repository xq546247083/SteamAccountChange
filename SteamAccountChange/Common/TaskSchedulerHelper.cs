using TaskScheduler;

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
            var ts = new TaskSchedulerClass();
            ts.Connect(null, null, null, null);

            var folder = ts.GetFolder("\\");
            folder.DeleteTask(taskName, 0);
        }

        /// <summary>
        /// 获取任务
        /// </summary>
        /// <param name="taskName"></param>
        /// <returns></returns>
        public static IRegisteredTask Get(string taskName)
        {
            var ts = new TaskSchedulerClass();
            ts.Connect(null, null, null, null);

            var folder = ts.GetFolder("\\");
            var tasks_exists = folder.GetTasks(1);
            for (int i = 1; i <= tasks_exists.Count; i++)
            {
                IRegisteredTask t = tasks_exists[i];
                if (t.Name.Equals(taskName))
                {
                    return t;
                }
            }

            return null;
        }

        /// <summary>
        /// 添加启动任务
        /// </summary>
        /// <param name="taskName"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool AddLuanchTask(string taskName, string path)
        {
            if (Get(taskName) != null)
            {
                Del(taskName);
            }

            // 计划对象
            var scheduler = new TaskSchedulerClass();
            scheduler.Connect(null, null, null, null);

            // 新建任务
            var task = scheduler.NewTask(0);
            task.RegistrationInfo.Author = Local.AppName;
            task.RegistrationInfo.Description = Local.AppName;
            task.Principal.RunLevel = _TASK_RUNLEVEL.TASK_RUNLEVEL_HIGHEST;

            // 触发类型
            task.Triggers.Create(_TASK_TRIGGER_TYPE2.TASK_TRIGGER_LOGON);

            // 动作
            IExecAction action = (IExecAction)task.Actions.Create(_TASK_ACTION_TYPE.TASK_ACTION_EXEC);
            action.Path = path;

            // 运行任务时间超时停止任务吗? PTOS 不开启超时
            task.Settings.ExecutionTimeLimit = "PT0S";
            // 只有在交流电源下才执行
            task.Settings.DisallowStartIfOnBatteries = false;
            // 仅当计算机空闲下才执行
            task.Settings.RunOnlyIfIdle = false;

            var folder = scheduler.GetFolder("\\");
            IRegisteredTask regTask = folder.RegisterTaskDefinition(taskName, task, (int)_TASK_CREATION.TASK_CREATE, null, null, _TASK_LOGON_TYPE.TASK_LOGON_INTERACTIVE_TOKEN, "");
            IRunningTask runTask = regTask.Run(null);

            return runTask != null;
        }
    }
}