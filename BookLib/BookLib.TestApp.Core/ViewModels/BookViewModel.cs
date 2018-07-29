using BookLib.Core.Model;
using MvvmCross.ViewModels;

namespace BookLib.TestApp.Core.ViewModels
{
    public class BookViewModel : MvxViewModel<Book>
    {
        private Book _book;
        public Book Details
        {
            get { return _book; }
            set { SetProperty(ref _book, value); }
        }

        public override void Prepare(Book parameter)
        {
            Details = parameter;
        }
    }
}
