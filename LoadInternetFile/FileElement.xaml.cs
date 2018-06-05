using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Логика взаимодействия для FileElement.xaml
    /// </summary>
    public partial class FileElement : UserControl
    {
        public string Type { get; private set; }
        //имя файла
        public FileElement(Image img,string text,string type)
        {
            InitializeComponent();
            image.Source = img.Source;
            fileNAme.Text = text;
            this.Type = type;
        }
    }
}
