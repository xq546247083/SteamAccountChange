using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SteamAccountChange.Common;
using SteamAccountChange.Helper;
using SteamAccountChange.Manager;
using SteamAccountChange.Model;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SteamAccountChange.ViewModel
{
    /// <summary>
    /// 主界面的ViewModel
    /// </summary>
    public class MainWindowViewModel : ObservableObject
    {
        /// <summary>
        /// 当前的Steam账号
        /// </summary>
        private string currentSteamAccount;

        /// <summary>
        /// 构造方法
        /// </summary>
        public MainWindowViewModel()
        {
            Init();
        }

        #region 绑定属性

        /// <summary>
        /// 账号列表
        /// </summary>
        private List<SteamAccoutInfo> steamAccoutInfoList;

        /// <summary>
        /// 账号列表
        /// </summary>
        public List<SteamAccoutInfo> SteamAccoutInfoList
        {
            get
            {
                return steamAccoutInfoList;
            }
            set
            {
                steamAccoutInfoList = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 选中账号
        /// </summary>
        private SteamAccoutInfo selectedSteamAccoutInfo;

        /// <summary>
        /// 选中账号
        /// </summary>
        public SteamAccoutInfo SelectedSteamAccoutInfo
        {
            get
            {
                return selectedSteamAccoutInfo;
            }
            set
            {
                selectedSteamAccoutInfo = value;

                if (value != null)
                {
                    SteamAccountAccountText = selectedSteamAccoutInfo.Account;
                    SteamAccountNameText = selectedSteamAccoutInfo.Name;
                    SteamAccountPasswordText = selectedSteamAccoutInfo.Password;
                    SteamAccountOrderText = selectedSteamAccoutInfo.Order;
                }

                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 进程列表模式
        /// </summary>
        private ProcessListMode processListMode;

        /// <summary>
        /// 进程列表模式
        /// </summary>
        public ProcessListMode ProcessListMode
        {
            get
            {
                return processListMode;
            }
            set
            {
                processListMode = value;
                OnPropertyChanged();
                LoadProcessList();
            }
        }

        /// <summary>
        /// 显示的进程列表
        /// </summary>
        private List<ProcessInfo> displayProcessList;

        /// <summary>
        /// 显示的进程列表
        /// </summary>
        public List<ProcessInfo> DisplayProcessList
        {
            get
            {
                return displayProcessList;
            }
            set
            {
                displayProcessList = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 选中的进程名
        /// </summary>
        private string selectedProcessName;

        /// <summary>
        /// 选中的进程名
        /// </summary>
        public string SelectedProcessName
        {
            get
            {
                return selectedProcessName;
            }
            set
            {
                selectedProcessName = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// steamAccount的Account文本内容
        /// </summary>
        private string steamAccountAccountText;

        /// <summary>
        /// steamAccount的Account文本内容
        /// </summary>
        public string SteamAccountAccountText
        {
            get
            {
                return steamAccountAccountText;
            }
            set
            {
                steamAccountAccountText = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// steamAccount的Name文本内容
        /// </summary>
        private string steamAccountNameText;

        /// <summary>
        /// steamAccount的Name文本内容
        /// </summary>
        public string SteamAccountNameText
        {
            get
            {
                return steamAccountNameText;
            }
            set
            {
                steamAccountNameText = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// steamAccount的Password文本内容
        /// </summary>
        private string steamAccountPasswordText;

        /// <summary>
        /// steamAccount的Password文本内容
        /// </summary>
        public string SteamAccountPasswordText
        {
            get
            {
                return steamAccountPasswordText;
            }
            set
            {
                steamAccountPasswordText = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// steamAccount的Order文本内容
        /// </summary>
        private string steamAccountOrderText;

        /// <summary>
        /// steamAccount的Order文本内容
        /// </summary>
        public string SteamAccountOrderText
        {
            get
            {
                return steamAccountOrderText;
            }
            set
            {
                steamAccountOrderText = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region 绑定方法

        /// <summary>
        /// 按键
        /// </summary>
        public RelayCommand<CommandParameterEx> MainWindowKeyDownCommand => new RelayCommand<CommandParameterEx>(DoMainWindowKeyDown);

        /// <summary>
        /// 按键
        /// </summary>
        /// <param name="commandParameterEx">参数</param>
        private void DoMainWindowKeyDown(CommandParameterEx commandParameterEx)
        {
            // 转换参数
            var keyEventArgs = commandParameterEx.EventArgs as KeyEventArgs;
            if (keyEventArgs == null)
            {
                return;
            }

            switch (keyEventArgs.Key)
            {
                case Key.F1:
                    MessageBox.Show("1、点击【新建】添加游戏账号\r\n2、点击【修改】编辑\r\n3、自行摸索，很简单的", "帮助");
                    break;
            }
        }

        /// <summary>
        /// 正在关闭窗口
        /// </summary>
        public RelayCommand<CommandParameterEx> MainWindowClosingCommand => new RelayCommand<CommandParameterEx>(DoMainWindowClosing);

        /// <summary>
        /// 正在关闭窗口
        /// </summary>
        /// <param name="commandParameterEx">参数</param>
        private void DoMainWindowClosing(CommandParameterEx commandParameterEx)
        {
            // 转换参数
            var cancelEventArgs = commandParameterEx.EventArgs as CancelEventArgs;
            if (cancelEventArgs == null)
            {
                return;
            }

            Lactor.MainWindow.Visibility = System.Windows.Visibility.Hidden;
            Lactor.MainWindow.ShowInTaskbar = false;
            cancelEventArgs.Cancel = true;
        }

        /// <summary>
        /// 点击新建
        /// </summary>
        public RelayCommand NewBtnClickCommand => new RelayCommand(DoNewBtnClickCommand);

        /// <summary>
        /// 点击新建
        /// </summary>
        private void DoNewBtnClickCommand()
        {
            var (success, autoLoginUserObj) = RegistryHelper.Get(@"Software\Valve\Steam", "AutoLoginUser");
            if (success == false || autoLoginUserObj == null)
            {
                ShowToolTip("请先登陆Steam！");
                return;
            }
            var autoLoginUser = autoLoginUserObj.ToString();
            if (string.IsNullOrEmpty(autoLoginUser))
            {
                ShowToolTip("请先登陆Steam！");
                return;
            }

            // 构建账号信息
            var steamAccount = new SteamAccoutInfo();
            steamAccount.Account = autoLoginUser;
            steamAccount.Name = autoLoginUser;
            steamAccount.Password = "";
            steamAccount.Order = "0";

            // 添加账号
            var localData = LocalDataHelper.GetLocalData();
            if (localData.SteamAccoutInfoList.Any(r => r.Account == steamAccount.Account))
            {
                ShowToolTip("添加失败！账号已存在！");
                return;
            }

            localData.SteamAccoutInfoList.Add(steamAccount);
            LocalDataHelper.Save(localData);

            currentSteamAccount = steamAccount.Account;
            ReLoad();
            ShowToolTip("添加成功！");
        }

        /// <summary>
        /// 复制用户名
        /// </summary>
        public RelayCommand CopyUserAccountBtnClickCommand => new RelayCommand(DoCopyUserAccountBtnClick);

        /// <summary>
        /// 复制用户名
        /// </summary>
        private void DoCopyUserAccountBtnClick()
        {
            if (SelectedSteamAccoutInfo == null || SelectedSteamAccoutInfo.Account == null)
            {
                ShowToolTip("复制失败！");
                return;
            }

            Clipboard.SetDataObject(SelectedSteamAccoutInfo.Account);
            ShowToolTip("复制成功！");
        }

        /// <summary>
        /// 复制密码
        /// </summary>
        public RelayCommand CopyPasswordBtnClickCommand => new RelayCommand(DoCopyPasswordBtnClick);

        /// <summary>
        /// 复制密码
        /// </summary>
        private void DoCopyPasswordBtnClick()
        {
            if (SelectedSteamAccoutInfo == null || SelectedSteamAccoutInfo.Password == null)
            {
                ShowToolTip("复制失败！");
                return;
            }

            Clipboard.SetDataObject(SelectedSteamAccoutInfo.Password);
            ShowToolTip("复制成功！");
        }

        /// <summary>
        /// 点击更多
        /// </summary>
        public RelayCommand<ContextMenu> OperateBtnClickCommand => new RelayCommand<ContextMenu>(DoOperateBtnClick);

        /// <summary>
        /// 点击更多
        /// </summary>
        /// <param name="window">窗体</param>
        private void DoOperateBtnClick(ContextMenu cm)
        {
            cm.IsOpen = true;
        }

        /// <summary>
        /// 保存steam账号信息
        /// </summary>
        public RelayCommand SaveSteamAccoutInfoBtnClickCommand => new RelayCommand(DoSaveSteamAccoutInfoBtnClick);

        /// <summary>
        /// 保存steam账号信息
        /// </summary>
        private void DoSaveSteamAccoutInfoBtnClick()
        {
            if (SelectedSteamAccoutInfo == null)
            {
                ShowToolTip("保存失败！请选择要修改的账号！");
                return;
            }

            SelectedSteamAccoutInfo.Name = SteamAccountNameText;
            SelectedSteamAccoutInfo.Password = SteamAccountPasswordText;
            SelectedSteamAccoutInfo.Order = SteamAccountOrderText;

            // 删除账号
            var localData = LocalDataHelper.GetLocalData();
            localData.SteamAccoutInfoList.RemoveAll(r => r.Account == SelectedSteamAccoutInfo.Account);

            // 再新增账号
            localData.SteamAccoutInfoList.Add(SelectedSteamAccoutInfo);
            LocalDataHelper.Save(localData);

            currentSteamAccount = SelectedSteamAccoutInfo.Account;
            ReLoad();
            ShowToolTip("保存成功！");
        }

        /// <summary>
        /// 删除steam账号信息
        /// </summary>
        public RelayCommand DelSteamAccoutInfoBtnClickCommand => new RelayCommand(DoDelSteamAccoutInfoBtnClick);

        /// <summary>
        /// 删除steam账号信息
        /// </summary>
        private void DoDelSteamAccoutInfoBtnClick()
        {
            if (SelectedSteamAccoutInfo == null)
            {
                ShowToolTip("删除失败！请选择要修改的账号！");
                return;
            }

            // 删除账号
            var localData = LocalDataHelper.GetLocalData();
            localData.SteamAccoutInfoList = localData.SteamAccoutInfoList.Where(r => r.Account != SelectedSteamAccoutInfo.Account).ToList();
            LocalDataHelper.Save(localData);

            // 清理Steam本地数据
            SteamHelper.DeleteSteamAccount(SelectedSteamAccoutInfo.Account);

            ReLoad();
            ShowToolTip("删除成功！");
        }

        /// <summary>
        /// 添加游戏进程信息
        /// </summary>
        public RelayCommand SaveGameProcessInfoBtnClickCommand => new RelayCommand(DoSaveGameProcessInfoBtnClick);

        /// <summary>
        /// 添加游戏进程信息
        /// </summary>
        private void DoSaveGameProcessInfoBtnClick()
        {
            if (ProcessListMode != ProcessListMode.System)
            {
                ShowToolTip("添加失败！必须是系统进程模式！");
                return;
            }

            if (string.IsNullOrEmpty(SelectedProcessName))
            {
                ShowToolTip("添加失败！游戏进程必填！");
                return;
            }

            // 构建游戏进程信息
            var processInfo = new ProcessInfo();
            processInfo.Name = SelectedProcessName;

            // 添加游戏进程
            var localData = LocalDataHelper.GetLocalData();
            if (localData.KillProcessList.Any(r => r.Name == processInfo.Name))
            {
                ShowToolTip("添加失败！游戏进程已存在！");
                return;
            }

            localData.KillProcessList.Insert(0, processInfo);
            LocalDataHelper.Save(localData);

            ReLoad();
            ShowToolTip("添加成功！");
        }

        /// <summary>
        /// 删除游戏进程信息
        /// </summary>
        public RelayCommand DelGameProcessInfoBtnClickCommand => new RelayCommand(DoDelGameProcessInfoBtnClick);

        /// <summary>
        /// 删除游戏进程信息
        /// </summary>
        private void DoDelGameProcessInfoBtnClick()
        {
            if (ProcessListMode != ProcessListMode.Saved)
            {
                ShowToolTip("删除失败！必须是已保存配置模式！");
                return;
            }

            if (string.IsNullOrEmpty(SelectedProcessName))
            {
                ShowToolTip("删除失败！游戏进程必填！");
                return;
            }

            // 删除游戏进程
            var localData = LocalDataHelper.GetLocalData();
            localData.KillProcessList = localData.KillProcessList.Where(r => r.Name != SelectedProcessName).ToList();
            LocalDataHelper.Save(localData);

            ReLoad();
            ShowToolTip("删除成功！");
        }

        /// <summary>
        /// 其它
        /// </summary>
        public RelayCommand<ContextMenu> OtherBtnClickCommand => new RelayCommand<ContextMenu>(DoOtherBtnClick);

        /// <summary>
        /// 其它
        /// </summary>
        /// <param name="window">窗体</param>
        private void DoOtherBtnClick(ContextMenu cm)
        {
            cm.IsOpen = true;
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 初始化
        /// </summary>
        private void Init()
        {
            LoadSaveInfo();
        }

        /// <summary>
        /// 显示提示框
        /// </summary>
        /// <param name="text">文本</param>
        private void ShowToolTip(string text)
        {
            var tempToolTip = new ToolTip();
            tempToolTip.Content = text;
            tempToolTip.StaysOpen = false;
            tempToolTip.IsOpen = true;
        }

        /// <summary>
        /// 重新加载
        /// </summary>
        private void ReLoad()
        {
            NotifyIconManager.LoadMenu();
            LoadSaveInfo();
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 加载保存的信息
        /// </summary>
        public void LoadSaveInfo()
        {
            // 加载账号信息
            var localData = LocalDataHelper.GetLocalData();
            SteamAccoutInfoList = localData.SteamAccoutInfoList.OrderBy(r => r.Order).ThenBy(r => r.Account).ToList();

            // 选中第一个
            if (selectedSteamAccoutInfo == null && SteamAccoutInfoList != null && SteamAccoutInfoList.Count > 0)
            {
                // 选回之前的账号
                SteamAccoutInfo currentSteamAccountInfo = null;
                if (!string.IsNullOrEmpty(currentSteamAccount))
                {
                    currentSteamAccountInfo = SteamAccoutInfoList.FirstOrDefault(r => r.Account == currentSteamAccount);
                }

                // 选中第一个
                if (currentSteamAccountInfo == null)
                {
                    currentSteamAccountInfo = SteamAccoutInfoList.FirstOrDefault();
                }

                SelectedSteamAccoutInfo = currentSteamAccountInfo;
            }

            LoadProcessList();
        }

        /// <summary>
        /// 加载进程列表
        /// </summary>
        private void LoadProcessList()
        {
            if (ProcessListMode == ProcessListMode.System)
            {
                var processes = Process.GetProcesses();
                DisplayProcessList = processes.Select(p => p.ProcessName).Distinct().OrderBy(n => n).Select(n => new ProcessInfo { Name = n }).ToList();
            }
            else
            {
                var localData = LocalDataHelper.GetLocalData();
                DisplayProcessList = localData.KillProcessList.ToList();
            }

            if (DisplayProcessList != null && DisplayProcessList.Count > 0)
            {
                SelectedProcessName = DisplayProcessList.FirstOrDefault()?.Name;
            }
            else
            {
                SelectedProcessName = string.Empty;
            }
        }

        #endregion
    }
}