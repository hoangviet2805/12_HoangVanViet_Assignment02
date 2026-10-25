using System.Collections.Generic;
using _12_HoangVanViet_A02_BE.Models;

namespace _12_HoangVanViet_A02_BE.Repositories
{
    public interface ISystemAccountRepository
    {
        IEnumerable<SystemAccount> GetAccounts();
        SystemAccount? GetAccountById(short id);
        SystemAccount? GetAccountByEmail(string email);
        void AddAccount(SystemAccount account);
        void UpdateAccount(SystemAccount account);
        void DeleteAccount(SystemAccount account);
    }
}
