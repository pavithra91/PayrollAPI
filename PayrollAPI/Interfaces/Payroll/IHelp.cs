using PayrollAPI.DataModel;

namespace PayrollAPI.Interfaces.Payroll
{
    public interface IHelp
    {
        public Task<MsgDto> GetCategories();
        public Task<MsgDto> GetArticlesByCategory(int id);
        public Task<MsgDto> CreateArticle(ArticleDto articleDto);
        public Task<MsgDto> UpdateArticle(ArticleDto articleDto);
    }
}
