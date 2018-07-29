using BookLib.TestApp.Core.ViewModels;
using MvvmCross.ViewModels;

namespace BookLib.TestApp.Core
{
    public class App : MvxApplication
    {
        public override void Initialize()
        {
            Setup.Register();

            RegisterAppStart<FirstViewModel>();
        }
    }
}
