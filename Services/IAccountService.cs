﻿using Student_management.DTOs;
using Student_management.DTOs.Account;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Student_management.Services
{
    public interface IAccountService
    {
        Task<List<AccountDto>> GetAll();
    }
}