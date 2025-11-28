using System.Collections.Generic;
using System.Threading.Tasks;
using Student_management.DTOs.Account;

namespace Student_management.Services
{
    public interface IAccountService
    {
        Task<List<AccountDto>> GetAll();
        Task<string?> LoginAsync(LoginRequestDto loginRequest);
        Task<AccountDto?> Detail(int id);
        Task<AccountDto> CreateAccount(CreateAccount dto);
        Task<AccountDto> EditAccount(int id, CreateAccount dto);
        Task<bool> DeleteAccount(int id);
    }
}