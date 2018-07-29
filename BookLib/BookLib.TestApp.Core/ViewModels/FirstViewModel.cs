using System.Threading.Tasks;
using BookLib.Core.Model;
using BookLib.Core.Search;
using MvvmCross.Commands;
using MvvmCross.ViewModels;

namespace BookLib.TestApp.Core.ViewModels
{
    public class FirstViewModel : MvxViewModel
    {
        private readonly ISearchService _searchService;

        public MvxAsyncCommand SearchCommand => new MvxAsyncCommand(OnSearch);

        private string _searchString;
        public string Search
        {
            get { return _searchString; }
            set { SetProperty(ref _searchString, value); }
        }

        private bool _searchInProgress;
        public bool SearchInProgress
        {
            get { return _searchInProgress; }
            set { SetProperty(ref _searchInProgress, value); }
        }

        private MvxObservableCollection<Book> _books;
        public MvxObservableCollection<Book> Books
        {
            get { return _books; }
            set { SetProperty(ref _books, value); }
        }

        public FirstViewModel(ISearchService searchService)
        {
            _searchService = searchService;
        }

        public override Task Initialize()
        {
            Books = new MvxObservableCollection<Book>();
            return base.Initialize();
        }

        private async Task OnSearch()
        {
            if (string.IsNullOrEmpty(Search) ||
                Search.Length < 3)
            {
                return;
            }

            await Task.Run(async () =>
            {
                SearchInProgress = true;
                Books.Clear();
                Books.AddRange(await _searchService.Search(Search));
                RaisePropertyChanged(() => Books);
                SearchInProgress = false;
            });
        }
    }
}
