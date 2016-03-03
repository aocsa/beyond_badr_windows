using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI;
using MLearning.Store.Components;

namespace DataSource
{
    public class PageDataSource : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _name;

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Name"));
            }
        }


        private string _description;

        public string Description
        {
            get { return _description; }
            set
            {
                _description = value; 
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Description"));
            }
        }


        private BitmapImage _imagecontent;

        public BitmapImage ImageContent
        {
            get { return _imagecontent; }
            set
            {
                _imagecontent = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("ImageSource"));
                   // PropertyChanged(this, new PropertyChangedEventArgs("ImageContent"));
            }
        }

        private Color _bordercolor;

        public Color BorderColor
        {
            get { return _bordercolor; }
            set
            {
                _bordercolor = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("BorderColor"));
            }
        }


    }
}
