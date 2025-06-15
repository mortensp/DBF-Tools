using DBF.ViewModels;
using Caliburn.Micro;
using System.Windows;
using System.Windows.Media;


namespace DBF.Views
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class ShellView : Window
    {
        public ShellView()
        {
            //System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("da-DK");

            if (this.WindowState == System.Windows.WindowState.Maximized)
                this.WindowState = System.Windows.WindowState.Normal;

        }
    }
}
