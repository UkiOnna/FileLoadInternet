using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LoadInternetFile
{
    /// <summary>
    /// Логика взаимодействия для AuthPage.xaml
    /// </summary>
    public partial class AuthPage : Page
    {
        Window window;
        bool isProtected;
        public AuthPage(Window win)
        {
            InitializeComponent();
            window = win;
        }

        private void ConnectClick(object sender, RoutedEventArgs e)
        {
            try
            {
                isProtected = (bool)checkSafe.IsChecked;
                FtpWebRequest ftpWebRequest = (FtpWebRequest)WebRequest.Create(Site.Text);
                ftpWebRequest.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                ftpWebRequest.Credentials = new NetworkCredential(login.Text, password.Text);
                ftpWebRequest.UsePassive = isProtected;
                FtpWebResponse fgtpWebResponse = (FtpWebResponse)ftpWebRequest.GetResponse();
                UserInfo info = new UserInfo();
                info.Login = login.Text;
                info.Password = password.Text;
                info.Site = Site.Text;
               
                window.Content = new DirectoryPage(window, ftpWebRequest,info);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


    }
}
