using System;
using System.Collections.Generic;
using Android.App;
using Android.OS;
using BookLib.Core.Model;
using MvvmCross.Droid.Support.V7.RecyclerView;
using MvvmCross.Droid.Support.V7.RecyclerView.ItemTemplates;

namespace BookLib.TestApp.Droid.Views
{
    [Activity]
    public class BookView : BaseView
    {
        protected override int LayoutResource => Resource.Layout.BookView;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            //SupportActionBar.SetDisplayHomeAsUpEnabled(false);
        }
    }
}
