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
    public interface ITagRepository
    {
        public Tag GetOne(int id);

        public List<Tag> GetAll();

        public void Add(Tag a);

        public void Update(Tag a);

        public void Delete(int id);
    }
}
