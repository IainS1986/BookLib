using System.Threading.Tasks;
using BookLib.Core.Model;
using BookLib.Core.Search;
using MvvmCross;
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

        public override void Prepare(Book parameter)
        {
            Details = parameter;

            switch (Details.Engine)
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

        public override async Task Initialize()
        {
            Loading = true;

            await _searchService.GetExtraDetails(Details);

            RaisePropertyChanged(() => Details);

            Loading = false;

            await base.Initialize();
        }
    }
}
