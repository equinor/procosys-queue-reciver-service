﻿using QueueReceiver.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QueueReceiver.Core.Interfaces
{
    public interface IPersonService
    {
        Task<Person?> UpdateWithOidIfNotFoundAsync(string userOid);
        Task<Person> CreateIfNotExistAsync(string userOid);
        Task<Person?> FindByOidAsync(string userOid);
        IEnumerable<string> GetAllNotInDb(IEnumerable<string> oids);
        Task<Person?> FindAndUpdateAsync(AdPerson adPerson);
        IEnumerable<string> GetMembersWithAccessToPlant(string plantId);
    }
}