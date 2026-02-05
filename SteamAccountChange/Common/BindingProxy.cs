using System.Windows;

namespace SteamAccountChange.Common
{
    /// <summary>
    /// 绑定代理
    /// </summary>
    public class BindingProxy : Freezable
    {
        /// <summary>
        /// 重构
        /// </summary>
        /// <returns></returns>
        protected override Freezable CreateInstanceCore()
        {
            return new BindingProxy();
        }

        /// <summary>
        /// 数据
        /// </summary>
        public object Data
        {
            get
            {
                return (object)GetValue(DataProperty);
            }
            set
            {
                SetValue(DataProperty, value);
            }
        }

        /// <summary>
        /// 数据
        /// </summary>
        public static readonly DependencyProperty DataProperty = DependencyProperty.Register("Data", typeof(object), typeof(BindingProxy), new UIPropertyMetadata(null));
    }
}