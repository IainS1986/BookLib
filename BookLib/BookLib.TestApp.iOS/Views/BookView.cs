using System;
using MvvmCross.Binding.BindingContext;
using BookLib.TestApp.Core.ViewModels;
using MvvmCross.Platforms.Ios.Views;
using SDWebImage;
using UIKit;
using Foundation;
using BookLib.TestApp.iOS.Extensions;

namespace BookLib.TestApp.iOS.Views
{
    public partial class BookView : MvxViewController<BookViewModel>
    {
        private string _imageUrl;
        public string ImageUrl
        {
            get
            {
                return _imageUrl;
            }

            set
            {
                _imageUrl = value;

                if (string.IsNullOrEmpty(_imageUrl))
                {
                    CoverImage.Image = null;
                }
                else
                {
                    CoverImage.SetImage(new NSUrl(value), null, SDWebImageOptions.ProgressiveDownload, ProgressHandler, CompletedHandler);
                    CoverImage.FadeIn();
                }
            }
        }

        public BookView() : base("BookView", null)
        {
        } 

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            CoverImage.ContentMode = UIViewContentMode.ScaleAspectFit;
            CoverImage.Layer.MasksToBounds = true;
            CoverImage.Alpha = 0;//Hide to fade in

            OverviewRoot.Layer.CornerRadius = 8;
            OverviewRoot.Layer.BorderColor = UIColor.DarkGray.CGColor;
            OverviewRoot.Layer.BorderWidth = 2;

            var set = this.CreateBindingSet<BookView, BookViewModel>();
            set.Bind(this).For(v => v.ImageUrl).To(vm => vm.Details.ImageURL);
            set.Bind(TitleLabel).To(vm => vm.Details.Title);
            set.Bind(AuthorLabel).To(vm => vm.Details.Author);
            set.Bind(SynopsisLabel).To(vm => vm.Details.Synopsis);
            set.Bind(GenreLabel).To(vm => vm.Details.Genre);
            set.Bind(PublishDateLabel).To(vm => vm.Details.PublishDateFormatted);
            set.Apply();
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
