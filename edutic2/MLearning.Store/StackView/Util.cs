using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace StackView
{
    public enum  StackViewState
	{
		Open, Close
	};


	public enum  SelectionType
	{
		ItemType, StackType, None
	};

	public delegate void StackItemFullAnimationStartedEventHandler(object sender, int chapter, int section, int page);
	public delegate void StackItemFullAnimationCompletedEventHandler(object sender, int chapter, int section, int page);
	public delegate void StackItemThumbAnimationStartedEventHandler(object sender, int chapter, int section, int page);
	public delegate void StackItemThumbAnimationCompletedEventHandler(object sender, int chapter, int section, int page);	

    public delegate void IControlsComponentSelectedEventHandler(object sender, SelectionType t, int index);

    

    public class Util
    {
        public static double DeviceHeight = 900.0;
        public static double DeviceWidth = 1600.0;
        public static double ThumbWidth = 267.0;
        public static double ThumbHeight = 150.0;
        public static double FrameWidth = 305.0;
        public static double FrameHeight = 210.0;
        public static double ItemStackWidth = 315.0;
        public static double ItemStackHeight = 210.0;
        public static double StackWidth = 381.0;
        public static double StackHeight = 335.0;
        public static double DeltaY = (900 - 335) / 2 + 4; //translate of Y 
        public static double ThumbScale = 6.0; // 1600.0 / 267.0 ;

        public static Color GetColorbyIndex(int i)
        {
            if (i == 0) return Windows.UI.ColorHelper.FromArgb(180, 4, 178, 171);
            if (i == 1) return Windows.UI.ColorHelper.FromArgb(180, 191, 245, 65);
            if (i == 2) return Windows.UI.ColorHelper.FromArgb(180, 228, 42, 214);
            if (i == 3) return Windows.UI.ColorHelper.FromArgb(180, 255, 189, 60);
            if (i == 4) return Windows.UI.ColorHelper.FromArgb(180, 67, 202, 255);
            if (i == 5) return Windows.UI.ColorHelper.FromArgb(180, 0, 70, 205);
            else return Colors.Red;
        }
    }
}
