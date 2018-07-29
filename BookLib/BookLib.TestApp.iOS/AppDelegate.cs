using BookLib.TestApp.Core;
using Foundation;
using MvvmCross.Platforms.Ios.Core;
using UIKit;

namespace BookLib.TestApp.iOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : MvxApplicationDelegate<Setup, App>
    {
        public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
            return base.FinishedLaunching(application, launchOptions);
        }
    }
}
