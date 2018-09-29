using System;
using MvvmCross.Binding.BindingContext;
using BookLib.TestApp.Core.ViewModels;
using MvvmCross.Platforms.Ios.Views;
using BookLib.Core.Converters;
using BookLib.TestApp.iOS.Cells;
using MvvmCross.Platforms.Ios.Binding.Views;
using UIKit;
using Foundation;
using MvvmCross.ViewModels;
using System.Collections;

namespace BookLib.TestApp.iOS.Views
{
    public partial class FirstView : MvxViewController<FirstViewModel>
    {
        SearchResultsTableSource _source;

        public FirstView() : base("FirstView", null)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            _source = new SearchResultsTableSource(SearchResults, BookOverviewCell.Key);

            SearchResults.Source = _source;
            SearchResults.RowHeight = 60;

            var set = this.CreateBindingSet<FirstView, FirstViewModel>();
            set.Bind(SearchInput).To(vm => vm.Search);
            set.Bind(SearchButton).To(vm => vm.SearchCommand);
            set.Bind(SearchInProgress).For(v => v.Hidden).To(vm => vm.SearchInProgress).WithConversion(new IsFalseConverter());
            set.Bind(SearchResults).For(v => v.Hidden).To(vm => vm.SearchInProgress);
            set.Bind(_source).To(vm => vm.Books);
            set.Bind(_source).For(v => v.SelectionChangedCommand).To(vm => vm.SelectedCommand);
            set.Bind(ModeButton).For("Title").To(vm => vm.Engine);
            set.Bind(ModeButton).To(vm => vm.EngineCommand);
            set.Apply();
        }

        public class SearchResultsTableSource : MvxStandardTableViewSource
        {
            public SearchResultsTableSource(UITableView tableView, NSString key) : base(tableView, key)
            {
                TableView.RegisterNibForCellReuse(BookOverviewCell.Nib, BookOverviewCell.Key);
            }
        }
    }
}
