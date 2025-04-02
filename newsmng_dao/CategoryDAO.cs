using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using newsmng_bussinessobject;

namespace newsmng_dao
{
    public class CategoryDAO
    {

        private FunewsManagementContext _dbContext;
        private static CategoryDAO instance;

        public CategoryDAO()
        {
            _dbContext = new FunewsManagementContext();
        }

        public static CategoryDAO Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new CategoryDAO();
                }
                return instance;
            }
        }

        public List<object> GetSelectList()
        {
            return _dbContext.Categories
                .Select(c => new
                {
                    Value = c.CategoryId.ToString(),
                    Text = c.CategoryDesciption
                })
                .ToList<object>();
        }


        public Category GetOne(short id)
        {
            return _dbContext.Categories
                .SingleOrDefault(a => a.CategoryId.Equals(id));
        }

        public List<Category> GetAll()
        {
            return _dbContext.Categories
                .ToList();
        }

        public void Add(Category a)
        {
            Category cur = GetOne(a.CategoryId);
            if (cur != null)
            {
                throw new Exception();
            }
            _dbContext.Categories.Add(a);
            _dbContext.SaveChanges();
        }

        public void Update(Category a)
        {
            Category cur = GetOne(a.CategoryId);
            if (cur == null)
            {
                throw new Exception();
            }
            _dbContext.Entry(cur).CurrentValues.SetValues(a);
            _dbContext.SaveChanges();
        }

        public void Delete(short id)
        {
            Category cur = GetOne(id);
            if (cur != null)
            {
                _dbContext.Categories.Remove(cur);
                _dbContext.SaveChanges(); // Delete the object
            }
        }

    }   
}
