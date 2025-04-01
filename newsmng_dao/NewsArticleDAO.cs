using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using newsmng_bussinessobject;

namespace newsmng_dao
{
    public class NewsArticleDAO
    {

        private FunewsManagementContext _dbContext;
        private static NewsArticleDAO instance;

        public NewsArticleDAO()
        {
            _dbContext = new FunewsManagementContext();
        }

        public static NewsArticleDAO Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new NewsArticleDAO();
                }
                return instance;
            }
        }

        public NewsArticle GetOne(string id)
        {
            return _dbContext.NewsArticles
                .Include(a => a.Tags)
                .SingleOrDefault(a => a.NewsArticleId.Equals(id));
        }

        public List<NewsArticle> GetAll()
        {
            return _dbContext.NewsArticles
                .Include(m => m.Category)
                .Include(m => m.CreatedBy)
                .ToList();
        }

        public List<NewsArticle> GetAllTrue()
        {
            return _dbContext.NewsArticles
                .Where(m => m.NewsStatus.Equals(true))
                .Include(m => m.Category)
                .Include(m => m.CreatedBy)
                .ToList();
        }

        public void Add(NewsArticle a)
        {
            NewsArticle cur = GetOne(a.NewsArticleId);
            if (cur != null)
            {
                throw new Exception();
            }
            _dbContext.NewsArticles.Add(a);
            _dbContext.SaveChanges();
        }

        public void Update(NewsArticle a)
        {
            NewsArticle cur = GetOne(a.NewsArticleId);
            if (cur == null)
            {
                throw new Exception();
            }
            _dbContext.Entry(cur).CurrentValues.SetValues(a);
            _dbContext.SaveChanges();
        }


        public void Delete(string id)
        {
            var article = _dbContext.NewsArticles
                .Include(a => a.Tags) // Load related tags
                .FirstOrDefault(a => a.NewsArticleId.Equals(id));

            if (article != null)
            {
                // Clear the relationship between article and tags
                // This will remove entries from the join table without deleting the tags
                if (article.Tags != null)
                {
                    article.Tags.Clear();
                    _dbContext.SaveChanges(); // Save changes to remove relationships
                }

                // Now remove the article
                _dbContext.NewsArticles.Remove(article);
                _dbContext.SaveChanges(); // Delete the article
            }
        }

    }
}
