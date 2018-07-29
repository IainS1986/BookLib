using BookLib.Core;
using BookLib.TestApp.Core;
using MvvmCross.Platforms.Ios.Core;
using UIKit;

namespace BookLib.TestApp.iOS
{
    public class Setup : MvxIosSetup<App>
    {
        protected override void InitializeIoC()
        {
            base.InitializeIoC();

            DependencyRegister.Register();
        }
    }
}
