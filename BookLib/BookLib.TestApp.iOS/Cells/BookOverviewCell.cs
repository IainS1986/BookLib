using System;
using Foundation;
using MvvmCross.Platforms.Ios.Binding.Views;
using MvvmCross.Binding.BindingContext;
using UIKit;
using BookLib.Core.Model;
using SDWebImage;

namespace BookLib.TestApp.iOS.Cells
{
    public partial class BookOverviewCell : MvxTableViewCell
    {
        public static readonly NSString Key = new NSString("BookOverviewCell");
        public static readonly UINib Nib;

        private string _thumbnailUrl;
        public string ThumbnailUrl
        {
            get
            {
                return _thumbnailUrl;
            }

            set
            {
                _thumbnailUrl = value;

                if (string.IsNullOrEmpty(_thumbnailUrl))
                {
                    CoverImage.Image = null;
                }
                else
                {
                    CoverImage.SetImage(new NSUrl(value), null, SDWebImageOptions.ProgressiveDownload, ProgressHandler, CompletedHandler);
                }
            }
        }

        static BookOverviewCell()
        {
            Nib = UINib.FromName("BookOverviewCell", NSBundle.MainBundle);
        }

        protected BookOverviewCell(IntPtr handle) : base(handle)
        {
            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<BookOverviewCell, Book>();
                set.Bind(this).For(v => v.ThumbnailUrl).To(vm => vm.ThumbnailURL);
                set.Bind(TitleLabel).To(vm => vm.Title);
                set.Bind(AuthorLabel).To(vm => vm.Author);
                set.Apply();
            });
        }

        void ProgressHandler(nint receivedSize, nint expectedSize)
        {
            ImageLoading.Hidden = false;
            ImageLoading.StartAnimating();
        }

        void CompletedHandler(UIImage image, NSError error, SDImageCacheType cacheType, NSUrl url)
        {
            ImageLoading.StopAnimating();
            ImageLoading.Hidden = true;
        }
    }
}
