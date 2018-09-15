using BookLib.Core.Search;
using MvvmCross;

namespace BookLib.Core
{
    public static class DependencyRegister
    {
        public static void Register()
        {
            //Mvx.LazyConstructAndRegisterSingleton<ISearchService, GoodReadsSearchService>();
            //Mvx.LazyConstructAndRegisterSingleton<ISearchService, AudibleSearchService>();
            //Mvx.LazyConstructAndRegisterSingleton<ISearchService, GraphicAudioSearchService>();
            //Mvx.LazyConstructAndRegisterSingleton<ISearchService, BigFinishSearchService>();
            //Mvx.LazyConstructAndRegisterSingleton<ISearchService, AudiobookstoreSearchService>();

            Mvx.RegisterSingleton<AudibleSearchService>(new AudibleSearchService());
            Mvx.RegisterSingleton<GoodReadsSearchService>(new GoodReadsSearchService());
            Mvx.RegisterSingleton<GraphicAudioSearchService>(new GraphicAudioSearchService());
            Mvx.RegisterSingleton<BigFinishSearchService>(new BigFinishSearchService());
            Mvx.RegisterSingleton<AudiobookstoreSearchService>(new AudiobookstoreSearchService());
        }
    }
}
