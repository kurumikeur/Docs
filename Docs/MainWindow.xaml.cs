using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Docs.Views;

namespace Docs
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindowView MWV { get; set; }
        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = this;
            MWV = new MainWindowView();

        }

        private void Button_AppendController(object sender, RoutedEventArgs e)
        {
            string func = ((Button)sender).Tag as string;
            switch (func)
            {
                case "AddNew":
                    MWV.AddNewAppend();
                    break;
                case "DeleteAll":
                    MWV.DeleteAllAppend();
                    break;
                case "Delete":
                    MWV.DeleteAppend(((FrameworkElement)e.Source).DataContext);
                    break;
                default:
                    return;
            }
        }
        private void Button_CalculateFile(object sender, RoutedEventArgs e)
        {
            MWV.CalculateDoc();
        }
    }
}