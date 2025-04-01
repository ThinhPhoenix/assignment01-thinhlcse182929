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
    }   
}
