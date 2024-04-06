namespace PayrollAPI.DataModel
{
    public class ArticleDto
    {
        public int? id { get; set; }
        public int categoryID { get; set; }
        public string? title { get; set; }
        public string? content { get; set; }
        public string? createdBy { get; set; }
        public string? lastUpdateBy { get; set; }
    }
}
