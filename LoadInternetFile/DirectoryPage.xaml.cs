using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// Логика взаимодействия для DirectoryPage.xaml
    /// </summary>
    public partial class DirectoryPage : Page
    {
        Window window;
        FtpWebRequest request;
        List<string> files;
        UserInfo userInfo;
        public DirectoryPage(Window win,FtpWebRequest req,UserInfo info)
        {
            InitializeComponent();
            window = win;
            request = req;
            userInfo = info;
            GetDirect();
            
        }
        public async void GetDirect()
        {
            string file = await DownloadFiles();
            files = file.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            foreach(var s in files) {
                if (s.ToList()[0] == 'd')
                {
                    //тебе сюда
                    var str = s.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Last();
                    string name = str.Substring(0, str.Length - 1);
                    Image image = new Image();
                    s.Split(' ');
                    image.Source = BitmapFrame.Create(new Uri(Directory.GetCurrentDirectory() + "/icn.png"));
                    listBox.Items.Add(new FileElement(image, s,"folder"));
                }
                else
                {
                    Image image = new Image();
                    image.Source = BitmapFrame.Create(new Uri(Directory.GetCurrentDirectory() + "/file.png"));
                    listBox.Items.Add(new FileElement(image, s,"file"));//имя файла
                }
                //image.Source = Directory.GetCurrentDirectory() + "file.png";
            }
        }

        private Task<string> DownloadFiles()
        {
            return Task.Run(() =>
            {
 
                FtpWebResponse fgtpWebResponse = (FtpWebResponse)request.GetResponse();
                using (var stream = fgtpWebResponse.GetResponseStream())
                {
                    
                    byte[] buffer = new byte[5012];
                    int bytes = stream.Read(buffer, 0, buffer.Length);
                    string data = Encoding.Default.GetString(buffer);
                    return data;
                }
            });
        }

        private async void DoubleClickFile(object sender, MouseButtonEventArgs e)
        {
            FileElement el = (FileElement)listBox.SelectedItem;
            if (el.Type == "file")
            {
                MessageBoxResult res = MessageBox.Show("Хотите загрузить себе этот файл", "Question", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (MessageBoxResult.No == res)
                {
                    //do no stuff
                }
                else
                {
                    SaveFileDialog openFileDialog = new SaveFileDialog();
                    if (openFileDialog.ShowDialog() == true)
                    {
                        string path  = openFileDialog.FileName;
                        await DownloadContent(path);
                    }
                    //do yes stuff
                }
            }
        }

        private Task DownloadContent(string path)
        {
            return Task.Run(() =>
            {
                FtpWebRequest ftpWebRequest = (FtpWebRequest)WebRequest.Create(userInfo.Site+ "/.txt");//имя файла
                ftpWebRequest.Method = WebRequestMethods.Ftp.DownloadFile;
                ftpWebRequest.Credentials = new NetworkCredential(userInfo.Login, userInfo.Password);
               
              FtpWebResponse fgtpWebResponse = (FtpWebResponse)ftpWebRequest.GetResponse();
                using (var stream = fgtpWebResponse.GetResponseStream())
                {

                    byte[] buffer = new byte[10000];
                    int bytes = stream.Read(buffer, 0, buffer.Length);
                    File.WriteAllBytes(path,buffer);
                    //string data = Encoding.Default.GetString(buffer);
                    //return ;
                }
            });
        }
    }
}
