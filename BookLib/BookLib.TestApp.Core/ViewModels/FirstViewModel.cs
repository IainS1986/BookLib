using System.Threading.Tasks;
using BookLib.Core.Model;
using BookLib.Core.Search;
using MvvmCross;
using MvvmCross.Commands;
using MvvmCross.ViewModels;

namespace BookLib.TestApp.Core.ViewModels
{
    public class FirstViewModel : MvxViewModel
    {
        private ISearchService _searchService;

        public MvxAsyncCommand SearchCommand => new MvxAsyncCommand(OnSearch);
        public MvxCommand EngineCommand => new MvxCommand(OnSelectEngine);

        public MvxAsyncCommand<Book> SelectedCommand => new MvxAsyncCommand<Book>(NavigateToBookDetails);

        private SearchType _engine = SearchType.Audible;
        public SearchType Engine
        {
            get { return _engine; }
            set { SetProperty(ref _engine, value); SetSearchEngine(); }
        }

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

        public FirstViewModel()
        {
            
        }

        public override Task Initialize()
        {
            Books = new MvxObservableCollection<Book>();
            SetSearchEngine();
            return base.Initialize();
        }

        private void OnSelectEngine()
        {
            switch (Engine)
            {
                case SearchType.Audible:
                    Engine = SearchType.Audiobookstore;
                    break;
                case SearchType.Audiobookstore:
                    Engine = SearchType.GraphicAudio;
                    break;
                case SearchType.GraphicAudio:
                    Engine = SearchType.BigFinish;
                    break;
                case SearchType.BigFinish:
                    Engine = SearchType.GoodReads;
                    break;
                case SearchType.GoodReads:
                    Engine = SearchType.Audible;
                    break;
            }
        }

        private void SetSearchEngine()
        {
            switch (Engine)
            {
                case SearchType.Audible:
                    _searchService = Mvx.GetSingleton<AudibleSearchService>();
                    break;
                case SearchType.Audiobookstore:
                    _searchService = Mvx.GetSingleton<AudiobookstoreSearchService>();
                    break;
                case SearchType.GraphicAudio:
                    _searchService = Mvx.GetSingleton<GraphicAudioSearchService>();
                    break;
                case SearchType.BigFinish:
                    _searchService = Mvx.GetSingleton<BigFinishSearchService>();
                    break;
                case SearchType.GoodReads:
                    _searchService = Mvx.GetSingleton<GoodReadsSearchService>();
                    break;
            }
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
                SearchInProgress = false;
            });
        }

        private async Task NavigateToBookDetails(Book book)
        {
            await NavigationService.Navigate<BookViewModel, Book>(book);
        }
    }
}
