using Android.Content;
using BookLib.Core;
using BookLib.TestApp.Core;
using MvvmCross.Binding.Bindings.Target.Construction;
using MvvmCross.Droid.Support.V7.AppCompat;
using MvvmCross.Platforms.Android.Core;
using MvvmCross.Platforms.Android.Presenters;

namespace BookLib.TestApp.Droid
{
    public class Setup : MvxAndroidSetup<App>
    {
        protected override void InitializeIoC()
        {
            base.InitializeIoC();

            DependencyRegister.Register();
        }

        protected override void FillTargetFactories(IMvxTargetBindingFactoryRegistry registry)
        {
            base.FillTargetFactories(registry);
            MvxAppCompatSetupHelper.FillTargetFactories(registry);
        }

        protected override void InitializeFirstChance()
        {
            base.InitializeFirstChance();
        }

        protected override IMvxAndroidViewPresenter CreateViewPresenter() => new MvxAppCompatViewPresenter(AndroidViewAssemblies);

    }
}
