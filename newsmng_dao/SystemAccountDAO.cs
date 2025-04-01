using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using newsmng_bussinessobject;

namespace newsmng_dao
{
    public class SystemAccountDAO
    {
        private FunewsManagementContext _dbContext;
        private static SystemAccountDAO instance;

        public SystemAccountDAO()
        {
            _dbContext = new FunewsManagementContext();
        }

        public static SystemAccountDAO Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SystemAccountDAO();
                }
                return instance;
            }
        }

        public SystemAccount? GetAccount(string email, string password)
        {
            // Load configuration from appsettings.json
            IConfiguration config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            // Read admin credentials from appsettings.json
            string? adminEmail = config["Administrator:Email"];
            string? adminPassword = config["Administrator:Password"];
            int adminRole = 0;
            int.TryParse(config["Administrator:Role"], out adminRole);

            // Validate admin login
            if (!string.IsNullOrEmpty(adminEmail) && !string.IsNullOrEmpty(adminPassword) &&
                email.Equals(adminEmail, StringComparison.OrdinalIgnoreCase) &&
                password.Equals(adminPassword, StringComparison.Ordinal))
            {
                return new SystemAccount
                {
                    AccountName = "Administrator",
                    AccountEmail = adminEmail,
                    AccountRole = adminRole
                };
            }

            // Check for normal user in database
            return _dbContext.SystemAccounts
                .SingleOrDefault(m => m.AccountEmail == email && m.AccountPassword == password);
        }




        public SystemAccount GetOne(short id)
        {
            return _dbContext.SystemAccounts
                .SingleOrDefault(a => a.AccountId.Equals(id));
        }

        public List<SystemAccount> GetAll()
        {
            return _dbContext.SystemAccounts
                .ToList();
        }

        public void Add(SystemAccount a)
        {
            SystemAccount cur = GetOne(a.AccountId);
            if (cur != null)
            {
                throw new Exception();
            }
            _dbContext.SystemAccounts.Add(a);
            _dbContext.SaveChanges();
        }

        public void Update(SystemAccount a)
        {
            SystemAccount cur = GetOne(a.AccountId);
            if (cur == null)
            {
                throw new Exception();
            }
            _dbContext.Entry(cur).CurrentValues.SetValues(a);
            _dbContext.SaveChanges();
        }

        public void Delete(short id)
        {
            SystemAccount cur = GetOne(id);
            if (cur != null)
            {
                _dbContext.SystemAccounts.Remove(cur);
                _dbContext.SaveChanges(); // Delete the object
            }
        }

        public List<object> GetSelectList()
        {
            return _dbContext.SystemAccounts
                .Select(c => new
                {
                    Value = c.AccountId.ToString(),
                    Text = c.AccountId.ToString()
                })
                .ToList<object>();
        }
    }
}
