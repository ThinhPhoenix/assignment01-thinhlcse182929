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
    public class SystemAccountRepository : ISystemAccountRepository
    {
        public SystemAccount GetAccount(string email, string password)
        {
            return SystemAccountDAO.Instance.GetAccount(email, password);
        }

        public SystemAccount GetOne(short id)
        {
            return SystemAccountDAO.Instance.GetOne(id);
        }

        public List<object> GetSelectList()
        {
            return SystemAccountDAO.Instance.GetSelectList();
        }

        public List<SystemAccount> GetAll()
        {
            return SystemAccountDAO.Instance.GetAll();
        }

        public void Add(SystemAccount a)
        {
            SystemAccountDAO.Instance.Add(a);        
        }

        public void Update(SystemAccount a)
        {
            SystemAccountDAO.Instance.Update(a);
        }

        public void Delete(short id)
        {
            SystemAccountDAO.Instance.Delete(id);
        }
    }
}
