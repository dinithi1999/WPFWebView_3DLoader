using System.IO;
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

namespace WPFWebView2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Ensure WebView2 is initialized asynchronously
            InitializeAsync();
        }

        /// <summary>
        /// Initialize WebView2 with local HTML content
        /// WebView2 is a Chromium-based control that renders web content in a WPF application
        /// Why WebView2 need to setup as async? 
        /// </summary>
        private async void InitializeAsync()
        {
            // Initialize WebView2 control
            await webView.EnsureCoreWebView2Async(null);

            // Path to your local HTML file
            //string htmlFilePath = @"C:\Users\DinithiAthukoralage\source\repos\WPFWebView2\WPFWebView2\wwwroot\index.html";

            // Load the local HTML file into WebView2
            webView.Source = new Uri("http://localhost:8080/index.html");
        }
    }
}