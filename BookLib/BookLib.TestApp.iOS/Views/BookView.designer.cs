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
		UIKit.UIImageView CoverImage { get; set; }

		[Outlet]
		UIKit.UIActivityIndicatorView ImageLoading { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (CoverImage != null) {
				CoverImage.Dispose ();
				CoverImage = null;
			}

			if (ImageLoading != null) {
				ImageLoading.Dispose ();
				ImageLoading = null;
			}
		}
	}
}
