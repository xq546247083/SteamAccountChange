using SteamAccountChange.Common;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SteamAccountChange
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            Init();
            RegisteEvent();
        }

        /// <summary>
        /// 注册事件
        /// </summary>
        private void RegisteEvent()
        {
            this.KeyDown += MainWindow_KeyDown;
            this.Closing += MainWindow_Closing;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        private void Init()
        {
            cbAccount.Items.Clear();

            // 加载账号信息
            var saveInfo = SteamHelper.GetSaveInfo();
            cbAccount.ItemsSource = saveInfo.GameProcessList;

            // 选中第一个
            if (cbAccount.Items.Count > 0)
            {
                cbAccount.SelectedIndex = 0;
            }
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
        /// 点击确定
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void SureBtn_Click(object sender, RoutedEventArgs e)
        {
            if (cbAccount.SelectedValue != null)
                SteamHelper.OpenSteam(cbAccount.SelectedValue.ToString());
        }

        /// <summary>
        /// 复制用户名
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void CopyUserAccountBtn_Click(object sender, RoutedEventArgs e)
        {
            var saveInfo = SteamHelper.GetSaveInfo();
            if (saveInfo != null) 
            {
                var steamAccount = saveInfo.SteamAccoutInfoList.FirstOrDefault(r => r.Account == cbAccount.SelectedValue.ToString());
                if (steamAccount != null)
                {
                    Clipboard.SetDataObject(steamAccount.Account);
                    ShowToolTip("复制成功！");
                    return;
                }
            }

            ShowToolTip("复制失败！");
        }

        /// <summary>
        /// 复制密码
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void CopyPasswordBtn_Click(object sender, RoutedEventArgs e)
        {
            var saveInfo = SteamHelper.GetSaveInfo();
            if (saveInfo != null)
            {
                var steamAccount = saveInfo.SteamAccoutInfoList.FirstOrDefault(r => r.Account == cbAccount.SelectedValue.ToString());
                if (steamAccount != null)
                {
                    Clipboard.SetDataObject(steamAccount.Password);
                    ShowToolTip("复制成功！");
                    return;
                }
            }

            ShowToolTip("复制失败！");
        }

        /// <summary>
        /// 按键
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.F1:
                    MessageBox.Show("1、请在程序运行目录创建一个Steamaccount.txt \n\r2、把【Name ID】写在Steamaccount.txt里，一行一个。\n\rPS:各个账号都在电脑上点了记住密码登录过。", "帮助");
                    break;
            }
        }

        /// <summary>
        /// 正在关闭窗口
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Lactor.MainWindow.Visibility = System.Windows.Visibility.Hidden;
            Lactor.MainWindow.ShowInTaskbar = false;

            e.Cancel = true;
        }
    }
}
