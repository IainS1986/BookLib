using Android.App;
using Android.OS;
using Android.Text;
using Android.Text.Method;
using Android.Widget;
using BookLib.TestApp.Core.ViewModels;
using MvvmCross.Binding.BindingContext;

namespace BookLib.TestApp.Droid.Views
{
    [Activity]
    public class BookView : BaseView
    {
        private TextView _synopsis;

        protected override int LayoutResource => Resource.Layout.BookView;

        public string Synopsis
        {
            get => _synopsis.Text;
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    _synopsis.TextFormatted = null;
                    return;
                }

                if (Build.VERSION.SdkInt >= BuildVersionCodes.N)
                {
                    _synopsis.TextFormatted = Html.FromHtml((value), Html.FromHtmlModeLegacy);
                }
                else
                {
                    _synopsis.TextFormatted = Html.FromHtml(value);
                }
            }
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            _synopsis = FindViewById<TextView>(Resource.Id.book_synopsis);
            _synopsis.MovementMethod = new ScrollingMovementMethod();

            //SupportActionBar.SetDisplayHomeAsUpEnabled(false);

            var set = this.CreateBindingSet<BookView, BookViewModel>();
            set.Bind(this).For(v => v.Synopsis).To(vm => vm.Details.Synopsis);
            set.Apply();
        }

    }
}
