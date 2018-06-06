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
        string currentSite;
        public DirectoryPage(Window win, FtpWebRequest req, UserInfo info)
        {
            InitializeComponent();
            window = win;
            request = req;
            userInfo = info;
            currentSite = info.Site;
            GetDirect(req);

        }
        public async void GetDirect(FtpWebRequest req)
        {
            listBox.Items.Clear();
            listBox.Items.Refresh();
            string file = await DownloadFiles(req);
            files = file.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            foreach (var s in files)
            {
                if (s.ToList()[0] == 'd')
                {
                    var str = s.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Last();
                    string name = str.Substring(0, str.Length - 1);
                    Image image = new Image();
                    s.Split(' ');
                    image.Source = BitmapFrame.Create(new Uri(Directory.GetCurrentDirectory() + "/icn.png"));
                    listBox.Items.Add(new FileElement(image, s, "folder", name));
                }
                else
                {
                    var str = s.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Last();
                    string name = str.Substring(0, str.Length - 1);
                    Image image = new Image();
                    image.Source = BitmapFrame.Create(new Uri(Directory.GetCurrentDirectory() + "/file.png"));
                    listBox.Items.Add(new FileElement(image, s, "file", name));//имя файла
                }

            }
        }

        private Task<string> DownloadFiles(FtpWebRequest req)
        {
            return Task.Run(() =>
            {
                FtpWebResponse fgtpWebResponse = (FtpWebResponse)req.GetResponse();
                using (var stream = fgtpWebResponse.GetResponseStream())
                {

                    byte[] buffer = new byte[100000];
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

                }
                else
                {
                    SaveFileDialog openFileDialog = new SaveFileDialog();
                    if (openFileDialog.ShowDialog() == true)
                    {
                        string path = openFileDialog.FileName;
                        await DownloadContent(path, el.Name);
                    }
                }
            }
            else
            {
                FtpWebRequest refreshReequest = (FtpWebRequest)WebRequest.Create(userInfo.Site + @"\" + el.Name);
                refreshReequest.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                refreshReequest.Credentials = new NetworkCredential(userInfo.Login, userInfo.Password);
                GetDirect(refreshReequest);
                currentSite = userInfo.Site + @"\" + el.Name;
            }
        }

        private Task DownloadContent(string path, string name)
        {
            return Task.Run(() =>
            {
                try
                {
                    FtpWebRequest ftpWebRequest = (FtpWebRequest)WebRequest.Create(userInfo.Site + "/" + name);//имя файла
                    ftpWebRequest.Method = WebRequestMethods.Ftp.DownloadFile;
                    ftpWebRequest.Credentials = new NetworkCredential(userInfo.Login, userInfo.Password);

                    FtpWebRequest ftpWebRequestSize = (FtpWebRequest)WebRequest.Create(userInfo.Site + name);//имя файла  //адина
                    ftpWebRequestSize.Method = WebRequestMethods.Ftp.GetFileSize;
                    ftpWebRequestSize.Credentials = new NetworkCredential(userInfo.Login, userInfo.Password);
                    long fileSize;
                    FtpWebResponse ftpFileSuzeResponse = (FtpWebResponse)ftpWebRequestSize.GetResponse();

                    fileSize = ftpFileSuzeResponse.ContentLength;

                    FtpWebResponse fgtpWebResponse = (FtpWebResponse)ftpWebRequest.GetResponse();
                    using (var stream = fgtpWebResponse.GetResponseStream())
                    {

                        byte[] buffer = new byte[fileSize];
                        int bytes = stream.Read(buffer, 0, buffer.Length);
                        File.WriteAllBytes(path, buffer);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            });
        }

        private async void UploadButtonClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();

            if (dialog.ShowDialog() == true)
            {
                await UploadContent(dialog.FileName);
            }
        }

        private Task UploadContent(string name)
        {
            return Task.Run(() =>
            {
                FileInfo fileInfo = new FileInfo(name);


                byte[] data = File.ReadAllBytes(name);

                FtpWebRequest temp = (FtpWebRequest)WebRequest.Create(new Uri($@"{currentSite}/{fileInfo.Name}"));
                temp.Method = WebRequestMethods.Ftp.UploadFile;
                temp.Credentials = new NetworkCredential(userInfo.Login, userInfo.Password);
                temp.ContentLength = data.Length;
                FtpWebResponse response = (FtpWebResponse)temp.GetResponse();

                using (var stream = temp.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
            });
        }
    }
}

