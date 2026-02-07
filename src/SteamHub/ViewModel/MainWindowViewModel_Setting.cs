using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SteamHub.Manager;
using SteamHub.Model;
using SteamHub.Repositories;

namespace SteamHub.ViewModel
{
    /// <summary>
    /// 主界面的ViewModel
    /// </summary>
    public partial class MainWindowViewModel : ObservableObject
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
            LoadProcessList();
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
            if (ProcessListMode != ProcessListMode.Saved)
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
    }
}