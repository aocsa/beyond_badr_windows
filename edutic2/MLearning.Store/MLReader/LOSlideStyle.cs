using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace MLReader
{
    public class LOSlideStyle
    {
        public LOSlideStyle()
        { }

        /// <summary>
        /// style number
        /// </summary>
        private int _id = 0 ;

        public int ID
        {
            get { return _id; }
            set { _id = value; }
        }

        /// <summary>
        /// main = 0 , second = 1
        /// </summary>
        private int _colornumber;

        public int ColorNumber
        {
            get { return _colornumber; }
            set { _colornumber = value; }
        }
        

        private Color _titlecolor;

        public Color TitleColor
        {
            get { return _titlecolor; }
            set { _titlecolor = value; }
        }


        private Color _contentcolor;

        public Color ContentColor
        {
            get { return _contentcolor; }
            set { _contentcolor = value; }
        }

        private Color _backgroundcolor;

        public Color BackgroundColor
        {
            get { return _backgroundcolor; }
            set { _backgroundcolor = value; }
        }

        private Color _bordercolor;

        public Color BorderColor
        {
            get { return _bordercolor; }
            set { _bordercolor = value; }
        }
        
        
    }
}
