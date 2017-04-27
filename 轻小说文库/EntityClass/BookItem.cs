using System.Collections.ObjectModel;

namespace 轻小说文库
{
    public class BookItem
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public string Classification { get; set; }
        public string State { get; set; }
        public string CoverUri { get; set; }
        public string LastUpdate { get; set; }
        public string Interlinkage { get; set; }
        public string Summary { get; set; }
        public string ReadLinkage { get; set; }
        public ObservableCollection<BookIndex> Indexes { get; set; }
    }
}
