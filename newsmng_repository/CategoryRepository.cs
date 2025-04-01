using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using newsmng_dao;

namespace newsmng_repository
{
    public class CategoryRepository : ICategoryRepository
    {
        public List<object> GetSelectList()
        {
            return CategoryDAO.Instance.GetSelectList();
        }
    }
}
