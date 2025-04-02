using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using newsmng_bussinessobject;
using newsmng_dao;

namespace newsmng_repository
{
    public class CategoryRepository : ICategoryRepository
    {
        public List<object> GetSelectList()
        {
            return CategoryDAO.Instance.GetSelectList();
        }

        public Category GetOne(short id)
        {
            return CategoryDAO.Instance.GetOne(id);
        }

        public List<Category> GetAll()
        {
            return CategoryDAO.Instance.GetAll();
        }

        public void Add(Category a)
        {
            CategoryDAO.Instance.Add(a);
        }

        public void Update(Category a)
        {
            CategoryDAO.Instance.Update(a);
        }

        public void Delete(short id)
        {
            CategoryDAO.Instance.Delete(id);
        }
    }
}
