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
    public class TagRepository : ITagRepository
    {
        public Tag GetOne(int id)
        {
            return TagDAO.Instance.GetOne(id);
        }

        public List<Tag> GetAll()
        {
            return TagDAO.Instance.GetAll();
        }

        public void Add(Tag a)
        {
            TagDAO.Instance.Add(a);
        }

        public void Update(Tag a)
        {
            TagDAO.Instance.Update(a);
        }

        public void Delete(int id)
        {
            TagDAO.Instance.Delete(id);
        }
    }
}
