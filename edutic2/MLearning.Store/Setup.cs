using Cirrious.CrossCore;
using Cirrious.CrossCore.Platform;
using Cirrious.MvvmCross.ViewModels;
using Cirrious.MvvmCross.WindowsCommon.Platform;
using MLearning.Core.File;
using MLearning.Store.File;
using Windows.UI.Xaml.Controls;

namespace MLearning.Store
{
    public class Setup : MvxWindowsSetup
    {
        public Setup(Frame rootFrame) : base(rootFrame)
        {
        }

        protected override void InitializeLastChance()
        {
            Mvx.RegisterSingleton<IAsyncStorageService>(new AsyncStorageStoreService());
            base.InitializeLastChance();
        }
        protected override IMvxApplication CreateApp()
        {
            return new Core.App();
        }
		
        protected override IMvxTrace CreateDebugTrace()
        {
            return new DebugTrace();
        }
    }
}