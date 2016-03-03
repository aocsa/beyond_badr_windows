using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLReader
{
    public delegate void ISlideElementSizeChangedEventHandler(object sender) ;
    public interface ISlideElement
    { 
        double GetSize();
        double Position { get; set; }
        event ISlideElementSizeChangedEventHandler ISlideElementSizeChanged;
    }
}
