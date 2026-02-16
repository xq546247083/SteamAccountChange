using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SteamHub.Enums;
using SteamHub.Manager;
using SteamHub.Repositories;
using System.Diagnostics;

namespace SteamHub.ViewModel
{
    /// <summary>
    /// 主界面的ViewModel
    /// </summary>
    public partial class MainViewModel
    {
        #region 绑定属性

        /// <summary>
        /// 进程列表模式
        /// </summary>
        [ObservableProperty]
        private ProcessListMode processListMode;

        /// <summary>
        /// 显示的进程列表
        /// </summary>
        [ObservableProperty]
        private List<string> displayProcessList;

        /// <summary>
        /// 选中的进程名
        /// </summary>
        [ObservableProperty]
        private string selectedProcessName;

        #endregion

        #region 属性变化引发的事件

        partial void OnProcessListModeChanged(ProcessListMode oldValue, ProcessListMode newValue)
        {
            LoadProcesses();
        }

        #endregion

        #region 绑定方法

        /// <summary>
        /// 添加游戏进程信息
        /// </summary>
        [RelayCommand]
        private void AddKillProcess()
        {
            if (ProcessListMode != ProcessListMode.System)
            {
                return;
            }

            if (string.IsNullOrEmpty(SelectedProcessName))
            {
                return;
            }

            SettingRepository.AddKillProcess(SelectedProcessName);

            ReLoad();
            Lactor.ShowToolTip("添加成功！");
        }

        /// <summary>
        /// 删除游戏进程信息
        /// </summary>
        [RelayCommand]
        private void DeleteKillProcess()
        {
            if (ProcessListMode != ProcessListMode.Kill)
            {
                return;
            }

            if (string.IsNullOrEmpty(SelectedProcessName))
            {
                return;
            }

            SettingRepository.DeleteKillProcess(SelectedProcessName);

            ReLoad();
            Lactor.ShowToolTip("删除成功!");
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 加载进程列表
        /// </summary>
        private void LoadProcesses()
        {
            if (ProcessListMode == ProcessListMode.System)
            {
                var processes = Process.GetProcesses();
                DisplayProcessList = processes.Select(p => p.ProcessName).Distinct().OrderBy(n => n).ToList();
            }
            else
            {
                DisplayProcessList = SettingRepository.GetKillProcessList();
            }

            SelectedProcessName = DisplayProcessList != null && DisplayProcessList.Count > 0 ? DisplayProcessList.FirstOrDefault() : string.Empty;
        }

        #endregion
    }
}