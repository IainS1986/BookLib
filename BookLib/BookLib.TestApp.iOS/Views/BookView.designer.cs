// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace BookLib.TestApp.iOS.Views
{
	[Register ("BookView")]
	partial class BookView
	{
		[Outlet]
		UIKit.UILabel AuthorLabel { get; set; }

		[Outlet]
		UIKit.UIImageView CoverImage { get; set; }

		[Outlet]
		UIKit.UILabel GenreLabel { get; set; }

		[Outlet]
		UIKit.UIActivityIndicatorView ImageLoading { get; set; }

		[Outlet]
		UIKit.UIView OverviewRoot { get; set; }

		[Outlet]
		UIKit.UILabel PublishDateLabel { get; set; }

		[Outlet]
		UIKit.UITextView SynopsisLabel { get; set; }

		[Outlet]
		UIKit.UIActivityIndicatorView SynopsisLoading { get; set; }

		[Outlet]
		UIKit.UILabel TitleLabel { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (SynopsisLoading != null) {
				SynopsisLoading.Dispose ();
				SynopsisLoading = null;
			}

			if (AuthorLabel != null) {
				AuthorLabel.Dispose ();
				AuthorLabel = null;
			}

			if (CoverImage != null) {
				CoverImage.Dispose ();
				CoverImage = null;
			}

			if (GenreLabel != null) {
				GenreLabel.Dispose ();
				GenreLabel = null;
			}

			if (ImageLoading != null) {
				ImageLoading.Dispose ();
				ImageLoading = null;
			}

			if (OverviewRoot != null) {
				OverviewRoot.Dispose ();
				OverviewRoot = null;
			}

			if (PublishDateLabel != null) {
				PublishDateLabel.Dispose ();
				PublishDateLabel = null;
			}

			if (SynopsisLabel != null) {
				SynopsisLabel.Dispose ();
				SynopsisLabel = null;
			}

			if (TitleLabel != null) {
				TitleLabel.Dispose ();
				TitleLabel = null;
			}
		}
	}
}
