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
            Mvx.LazyConstructAndRegisterSingleton<ISearchService, BigFinishSearchService>();
        }
    }
}
