using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace DataSource
{
    public class SectionDataSource : INotifyPropertyChanged
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

        private ObservableCollection<PageDataSource> _pages = new ObservableCollection<PageDataSource>();

        public ObservableCollection<PageDataSource> Pages
        {
            get { return _pages; }
            set
            {
                _pages = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Pages"));
            }
        }


        private Color _temporalcolor;

        public Color TemporalColor
        {
            get
            {
                return _temporalcolor;
            }
            set
            {
                _temporalcolor = value;
                foreach (var item in _pages)
                    item.BorderColor = _temporalcolor;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("TemporalColor"));
            }
        }

    }
}
