using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using newsmng_bussinessobject;

namespace newsmng_dao
{
    public class TagDAO
    {

        private FunewsManagementContext _dbContext;
        private static TagDAO instance;

        public TagDAO()
        {
            _dbContext = new FunewsManagementContext();
        }

        public static TagDAO Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new TagDAO();
                }
                return instance;
            }
        }


        public Tag GetOne(int id)
        {
            return _dbContext.Tags
                .SingleOrDefault(a => a.TagId.Equals(id));
        }

        public List<Tag> GetAll()
        {
            return _dbContext.Tags
                .ToList();
        }

        public void Add(Tag a)
        {
            Tag cur = GetOne(a.TagId);
            if (cur != null)
            {
                throw new Exception();
            }
            _dbContext.Tags.Add(a);
            _dbContext.SaveChanges();
        }

        public void Update(Tag a)
        {
            Tag cur = GetOne(a.TagId);
            if (cur == null)
            {
                throw new Exception();
            }
            _dbContext.Entry(cur).CurrentValues.SetValues(a);
            _dbContext.SaveChanges();
        }

        public void Delete(int id)
        {
            Tag cur = GetOne(id);
            if (cur != null)
            {
                _dbContext.Tags.Remove(cur);
                _dbContext.SaveChanges(); // Delete the object
            }
        }

    }
}
