using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using newsmng_bussinessobject;
using newsmng_dao;

namespace newsmng_repository
{
    public class NewsArticleRepository : INewsArticleRepository
    {
        public NewsArticle GetOne(string id)
        {
            return NewsArticleDAO.Instance.GetOne(id);
        }

        public List<NewsArticle> GetAll()
        {
            return NewsArticleDAO.Instance.GetAll();
        }

        public void Add(NewsArticle a)
        {
            NewsArticleDAO.Instance.Add(a);
        }

        public void Update(NewsArticle a)
        {
            NewsArticleDAO.Instance.Update(a);
        }


        public void Delete(string id)
        {
            NewsArticleDAO.Instance.Delete(id);
        }

        public List<NewsArticle> GetAllTrue()
        {
            return NewsArticleDAO.Instance.GetAllTrue();
        }
    }
}
