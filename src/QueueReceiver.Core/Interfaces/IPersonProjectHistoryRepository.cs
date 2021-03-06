﻿using QueueReceiver.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QueueReceiver.Core.Interfaces
{
    public interface IPersonProjectHistoryRepository
    {
        Task<List<PersonProjectHistory>> GetPersonProjectHistoryByIdAsync(int id);
        Task AddAsync(PersonProjectHistory personProjectHistory);
    }
}
