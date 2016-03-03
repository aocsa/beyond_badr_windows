using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;


namespace MLReader
{
    public class LOSlideSource : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private int _type;

        public int Type
        {
            get { return _type; }
            set
            {
                _type = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Type"));
            }
        }


        private LOSlideStyle _style;

        public LOSlideStyle Style
        {
            get { return _style; }
            set { _style = value; }
        }
        


        private string _title;

        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Title"));
            }
        }

        private string _author;

        public string Author
        {
            get { return _author; }
            set
            {
                _author = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Author"));
            }
        }


        private string _paragraph;

        public string Paragraph
        {
            get { return _paragraph; }
            set
            {
                _paragraph = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Paragraph"));
            }
        }

        private BitmapImage _image;

        public BitmapImage Image
        {
            get { return _image; }
            set
            {
                _image = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Image"));
            }
        }


        private byte[] _imagebytes;

        public byte[] ImageBytes
        {
            get { return _imagebytes; }
            set
            {
                _imagebytes = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("ImageBytes"));
            }
        }
        
        

        private string _imageurl;

        public string ImageUrl
        {
            get { return _imageurl; }
            set { _imageurl = value;
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs("ImageUrl"));
            }
        }
        

        private string _videourl;

        public string VideoUrl
        {
            get { return _videourl; }
            set
            {
                _videourl = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("VideoUrl"));
            }
        }


        private ObservableCollection<LOItemSource> _itemize;

        public ObservableCollection<LOItemSource> Itemize
        {
            get { return _itemize; }
            set
            {
                _itemize = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Itemize"));
            }
        }


    }
}
