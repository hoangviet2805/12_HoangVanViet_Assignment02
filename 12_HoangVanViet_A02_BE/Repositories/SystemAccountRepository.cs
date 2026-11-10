using System;
using System.Collections.Generic;
using System.Linq;
using _12_HoangVanViet_A02_BE.Models;

namespace _12_HoangVanViet_A02_BE.Repositories
{
    public class SystemAccountRepository : ISystemAccountRepository
    {
        private readonly FUNewsManagementContext _context;

        public SystemAccountRepository(FUNewsManagementContext context)
        {
            _context = context;
        }

        public IEnumerable<SystemAccount> GetAccounts() => _context.SystemAccounts.ToList();

        public SystemAccount? GetAccountById(short id) => _context.SystemAccounts.FirstOrDefault(c => c.AccountID == id);

        public SystemAccount? GetAccountByEmail(string email) => _context.SystemAccounts.FirstOrDefault(c => c.AccountEmail == email);

        public void AddAccount(SystemAccount account)
        {
            _context.SystemAccounts.Add(account);
            _context.SaveChanges();
        }

        public void UpdateAccount(SystemAccount account)
        {
            _context.SystemAccounts.Update(account);
            _context.SaveChanges();
        }

        public void DeleteAccount(SystemAccount account)
        {
            bool hasArticles = _context.NewsArticles.Any(n => n.CreatedByID == account.AccountID);
            if (hasArticles)
            {
                throw new InvalidOperationException("Cannot delete account because it has created news articles.");
            }
            _context.SystemAccounts.Remove(account);
            _context.SaveChanges();
        }
    }
}
