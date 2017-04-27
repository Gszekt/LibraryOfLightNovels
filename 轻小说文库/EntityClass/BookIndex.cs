using System.Collections.Generic;

namespace 轻小说文库
{
    public class BookIndex
    {
        public string VolumnTitle { get; set; }
        public List<TitleAndLinkage> ChapterTitles { get; set; }
        public BookIndex()
        {
            ChapterTitles = new List<TitleAndLinkage>();
        }
    }
    public class TitleAndLinkage
    {
        public string ChapterTitle { get; set; }
        public string InterLinkage { get; set; }
    }
}
