using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Micro.Future.ViewModel
{
    public class ImageVM : ViewModelBase
    {
        ImageSource _imageSource;
        public ImageSource ImageSource
        {
            get
            {
                return _imageSource;
            }
            set
            {
                _imageSource = value;
                OnPropertyChanged("ImageSource");
            }
        }

        private string _sourceUri;
        public string SourceUri
        {
            get
            {
                return _sourceUri;
            }
            set
            {
                _sourceUri = value;    
            }
        }

        public void SetImage(string uri)
        {
            if (uri != null)
            {
                BitmapImage bitMap = new BitmapImage(new Uri(uri, UriKind.RelativeOrAbsolute));
                ImageSource = bitMap;
            }
        }
    }
}
