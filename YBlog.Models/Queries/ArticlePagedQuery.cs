using YBlog.Models.Enums;

namespace YBlog.Models.Queries
{
    public class ArticlePagedQuery : PagedQuery
    {
        private string orderBy = "Id";
        private EnumOrderDirections? orderDirection;
        public string OrderBy
        {
            get { return orderBy; }
        }
        public EnumOrderDirections OrderDirection
        {
            get
            {
                if (orderDirection == null)
                    orderDirection = EnumOrderDirections.Desc;
                return orderDirection.Value;
            }
        }
        public void SetOrderBy(string value)
        {
            orderBy = value;
        }
        public void SetOrderDirection(EnumOrderDirections value)
        {
            orderDirection = value;
        }
        public int? CategoryId { get; set; }
        public string? Title { get; set; }
        public int? Status { get; set; }
    }
}
