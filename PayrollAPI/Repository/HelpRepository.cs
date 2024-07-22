using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Newtonsoft.Json;
using PayrollAPI.Data;
using PayrollAPI.DataModel;
using PayrollAPI.Interfaces;
using PayrollAPI.Models;
using System.Data;

namespace PayrollAPI.Repository
{
    public class HelpRepository : IHelp
    {
        private readonly DBConnect _context;
        public HelpRepository(DBConnect context)
        {
            _context = context;
        }

        public IDbTransaction BeginTransaction()
        {
            var transaction = _context.Database.BeginTransaction();

            return transaction.GetDbTransaction();
        }

        public async Task<MsgDto> GetCategories()
        {
            MsgDto _msg = new MsgDto();
            try
            {
                var _categoryList = await _context.Category.ToListAsync();

                if (_categoryList.Count > 0)
                {
                    _msg.Data = JsonConvert.SerializeObject(_categoryList);
                    _msg.MsgCode = 'S';
                    _msg.Message = "Success";
                    return _msg;
                }
                else
                {
                    _msg.Data = string.Empty;
                    _msg.MsgCode = 'E';
                    _msg.Message = "No Data Available";
                    return _msg;
                }
            }
            catch (Exception ex)
            {
                _msg.MsgCode = 'E';
                _msg.Message = "Error : " + ex.Message;
                _msg.Description = "Inner Expection : " + ex.InnerException;
                return _msg;
            }
        }

        public async Task<MsgDto> GetArticlesByCategory(int id)
        {
            MsgDto _msg = new MsgDto();
            try
            {
                var _articleList = await _context.Article.Where(o => o.categoryId == id).ToListAsync();

                if (_articleList != null)
                {
                    _msg.Data = JsonConvert.SerializeObject(_articleList);
                    _msg.MsgCode = 'S';
                    _msg.Message = "Success";
                    return _msg;
                }
                else
                {
                    _msg.Data = string.Empty;
                    _msg.MsgCode = 'E';
                    _msg.Message = "No Data Available";
                    return _msg;
                }
            }
            catch (Exception ex)
            {
                _msg.MsgCode = 'E';
                _msg.Message = "Error : " + ex.Message;
                _msg.Description = "Inner Expection : " + ex.InnerException;
                return _msg;
            }
        }

        public async Task<MsgDto> CreateArticle(ArticleDto articleDto)
        {
            MsgDto _msg = new MsgDto();
            try
            {
                using var transaction = BeginTransaction();

                var _article = new Article
                {
                    categoryId = articleDto.categoryID,
                    title = articleDto.title,
                    content = articleDto.content,
                    createdBy = articleDto.createdBy,
                    createdDate = DateTime.Now
                };

                _context.Add(_article);

                var _category = _context.Category.FirstOrDefault(o => o.id == articleDto.categoryID);

                if (_category != null)
                {
                    _category.articleCount += 1;
                    _context.Entry(_category).State = EntityState.Modified;
                }

                await _context.SaveChangesAsync();

                transaction.Commit();

                _msg.MsgCode = 'S';
                _msg.Message = "New Article Created Successfully";
                return _msg;
            }
            catch (Exception ex)
            {
                _msg.MsgCode = 'E';
                _msg.Message = "Error : " + ex.Message;
                _msg.Description = "Inner Expection : " + ex.InnerException;
                return _msg;
            }
        }

        public async Task<MsgDto> UpdateArticle(ArticleDto articleDto)
        {
            MsgDto _msg = new MsgDto();
            try
            {
                using var transaction = BeginTransaction();

                var _article = _context.Article.FirstOrDefault(o => o.id == articleDto.id);
                if (_article != null)
                {
                    _article.title = articleDto.title ?? _article.title;
                    _article.content = articleDto.content ?? _article.content;

                    _article.lastUpdateBy = _article.lastUpdateBy;
                    _article.lastUpdateDate = DateTime.Now;

                    _msg.MsgCode = 'S';
                    _msg.Message = "Article updated Successfully";

                    _context.Entry(_article).State = EntityState.Modified;
                    await _context.SaveChangesAsync();

                    transaction.Commit();

                    return _msg;
                }
                else
                {
                    _msg.MsgCode = 'N';
                    _msg.Message = "No Tax Code Found";
                    return _msg;
                }
            }
            catch (Exception ex)
            {
                _msg.MsgCode = 'E';
                _msg.Message = "Error : " + ex.Message;
                _msg.Description = "Inner Expection : " + ex.InnerException;
                return _msg;
            }
        }
    }
}
