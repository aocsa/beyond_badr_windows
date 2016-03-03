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
    public class BookDataSource : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

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

        private ObservableCollection<ChapterDataSource> _chapters = new ObservableCollection<ChapterDataSource>();

        public ObservableCollection<ChapterDataSource> Chapters
        {
            get { return _chapters; }
            set
            {
                _chapters = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Chapters"));
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
                foreach (var item in _chapters)
                    item.TemporalColor = _temporalcolor;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("TemporalColor"));
            }
        }

    }
}
