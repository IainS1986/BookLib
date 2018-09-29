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
	[Register ("FirstView")]
	partial class FirstView
	{
		[Outlet]
		UIKit.UIButton ModeButton { get; set; }

		[Outlet]
		UIKit.UIButton SearchButton { get; set; }

		[Outlet]
		UIKit.UIActivityIndicatorView SearchInProgress { get; set; }

		[Outlet]
		UIKit.UITextField SearchInput { get; set; }

		[Outlet]
		UIKit.UITableView SearchResults { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (SearchButton != null) {
				SearchButton.Dispose ();
				SearchButton = null;
			}

			if (SearchInProgress != null) {
				SearchInProgress.Dispose ();
				SearchInProgress = null;
			}

			if (SearchInput != null) {
				SearchInput.Dispose ();
				SearchInput = null;
			}

			if (SearchResults != null) {
				SearchResults.Dispose ();
				SearchResults = null;
			}

			if (ModeButton != null) {
				ModeButton.Dispose ();
				ModeButton = null;
			}
		}
	}
}
