using System;
using System.Collections.Generic;
using Android.App;
using Android.OS;
using BookLib.Core.Model;
using MvvmCross.Droid.Support.V7.RecyclerView;
using MvvmCross.Droid.Support.V7.RecyclerView.ItemTemplates;

namespace BookLib.TestApp.Droid.Views
{
    [Activity(Label = "View for FirstViewModel")]
    public class FirstView : BaseView
    {
        protected override int LayoutResource => Resource.Layout.FirstView;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SupportActionBar.SetDisplayHomeAsUpEnabled(false);

            var recycler = FindViewById<MvxRecyclerView>(Resource.Id.search);
            recycler.ItemTemplateSelector = new TypeTemplateSelector();
        }

        public class TypeTemplateSelector : IMvxTemplateSelector
        {
            private readonly Dictionary<Type, int> _typeMapping;

            public TypeTemplateSelector()
            {
                _typeMapping = new Dictionary<Type, int>
                {
                    { typeof(Book), Resource.Layout.item_book_overview },
                };
            }

            public int ItemTemplateId { get; set; }

            public int GetItemLayoutId(int fromViewType) => fromViewType;

            public int GetItemViewType(object forItemObject) => _typeMapping[forItemObject.GetType()];
        }
    }
}
