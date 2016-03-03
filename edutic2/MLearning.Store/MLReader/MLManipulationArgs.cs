using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLReader
{
    public class MLManipulationArgs
    {
        public MLManipulationArgs()
        {
            X = 0;
            Y = 0;
            Scale = 1.0;
            Rotate = 0;
        }

        public double X { get; set; }
        public double Y { get; set; }
        public double Scale { get; set; }
        public double Rotate { get; set; }
    }
}
