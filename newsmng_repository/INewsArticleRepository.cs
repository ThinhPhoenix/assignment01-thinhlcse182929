using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using newsmng_bussinessobject;
using newsmng_dao;

namespace newsmng_repository
{
    public interface INewsArticleRepository
    {
        public NewsArticle GetOne(string id);

        public List<NewsArticle> GetAll();

        public void Add(NewsArticle a);

        public void Update(NewsArticle a);

        public void Delete(string id);

        public List<NewsArticle> GetAllTrue();

    }
}
