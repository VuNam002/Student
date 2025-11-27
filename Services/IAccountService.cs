﻿using Student_management.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Student_management.Services
{
    public interface IAccountService
    {
        Task<List<AccountDto>> GetAllAccountsAsync();
    }
}