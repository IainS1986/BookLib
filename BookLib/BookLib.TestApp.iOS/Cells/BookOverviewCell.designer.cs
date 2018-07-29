// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace BookLib.TestApp.iOS.Cells
{
	[Register ("BookOverviewCell")]
	partial class BookOverviewCell
	{
		[Outlet]
		UIKit.UILabel AuthorLabel { get; set; }

		[Outlet]
		UIKit.UIImageView CoverImage { get; set; }

		[Outlet]
		UIKit.UIActivityIndicatorView ImageLoading { get; set; }

		[Outlet]
		UIKit.UILabel TitleLabel { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (ImageLoading != null) {
				ImageLoading.Dispose ();
				ImageLoading = null;
			}

			if (AuthorLabel != null) {
				AuthorLabel.Dispose ();
				AuthorLabel = null;
			}

			if (TitleLabel != null) {
				TitleLabel.Dispose ();
				TitleLabel = null;
			}

			if (CoverImage != null) {
				CoverImage.Dispose ();
				CoverImage = null;
			}
		}
	}
}
