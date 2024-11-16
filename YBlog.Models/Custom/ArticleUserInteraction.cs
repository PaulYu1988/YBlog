namespace YBlog.Models.Custom
{
    public class ArticleUserInteraction
    {
        public int LikeCount { get; set; }
        public int FavoriteCount { get; set; }
        public bool LikeChecked { get; set; }
        public bool FavoriteChecked { get; set; }
    }
}
