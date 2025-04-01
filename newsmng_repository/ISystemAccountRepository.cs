using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using newsmng_bussinessobject;
using newsmng_dao;

namespace newsmng_repository
{
    public interface ISystemAccountRepository
    {
        public SystemAccount GetAccount(string email, string password);
        public SystemAccount GetOne(short id);
        public List<object> GetSelectList();

        public List<SystemAccount> GetAll();

        public void Add(SystemAccount a);

        public void Update(SystemAccount a);

        public void Delete(short id);
    }
}
