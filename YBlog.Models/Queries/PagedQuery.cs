namespace YBlog.Models.Queries
{
    public class PagedQuery
    {
        private int? _pageSize;
        private int? _page;
        public int? Page
        {
            get
            {
                if (!_page.HasValue)
                {
                    _page = 1;
                }
                return _page;
            }
            set
            {
                _page = value;
            }
        }
        public int PageSize
        {
            get
            {
                if (!_pageSize.HasValue)
                {
                    _pageSize = 20;
                }
                return _pageSize.Value;
            }
        }
        public int PageCount { get; set; }
        public int TotalCount { get; set; }
        public void SetPageSize(int pageSize)
        {
            _pageSize = pageSize;
        }
    }
}
