using System.Threading.Tasks;
using BookLib.Core.Model;
using BookLib.Core.Search;
using MvvmCross.ViewModels;

namespace BookLib.TestApp.Core.ViewModels
{
    public class BookViewModel : MvxViewModel<Book>
    {
        private ISearchService _searchService;

        private Book _book;
        public Book Details
        {
            get { return _book; }
            set { SetProperty(ref _book, value); }
        }

        private bool _loading;
        public bool Loading
        {
            get => _loading;
            set => SetProperty(ref _loading, value);
        }

        public BookViewModel(ISearchService search)
        {
            _searchService = search;
        }

        public override void Prepare(Book parameter)
        {
            Details = parameter;
        }

        public override async Task Initialize()
        {
            Loading = true;

            await _searchService.Synopsis(Details);

            RaisePropertyChanged(() => Details);

            Loading = false;

            await base.Initialize();
        }
    }
}
