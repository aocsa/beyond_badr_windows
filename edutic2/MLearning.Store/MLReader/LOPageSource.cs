using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace MLReader
{
    public class LOPageSource : INotifyPropertyChanged
    {
        public LOPageSource()
        { 
        }


        public event PropertyChangedEventHandler PropertyChanged;

        private BitmapImage _cover;

        public BitmapImage Cover
        {
            get { return _cover; }
            set { _cover = value;
                 
            }
        }


        private byte[] _coverbytes;

        public byte[] CoverBytes
        {
            get { return _coverbytes; }
            set
            {
                _coverbytes = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("CoverBytes"));
            }
        }
        


        private bool _isloaded;

        public bool IsLoaded
        {
            get { return _isloaded; }
            set { _isloaded = value;
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs("IsLoaded"));
            }
        }
        


        private string  _pagetitle;

        public string  PageTitle
        {
            get { return _pagetitle; }
            set { _pagetitle = value; }
        }


        private string _pagedescription;

        public string PageDescription
        {
            get { return _pagedescription; }
            set { _pagedescription = value; }
        }
        


        private List<LOSlideSource> _slides;

        public List<LOSlideSource> Slides
        {
            get { return _slides; }
            set { _slides = value; }
        }


        public int Index { get; set; }

        public int PageIndex { get; set; }

        //section
        public int StackIndex { get; set; }

        //chapter
        public int LOIndex { get; set; }

    }
}
