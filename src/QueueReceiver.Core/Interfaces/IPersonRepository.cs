﻿using QueueReceiver.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QueueReceiver.Core.Interfaces
{
    public interface IPersonRepository
    {
        Task<Person?> FindByUserOidAsync(string userOid);
        Task<long> FindPersonIdByUserOidAsync(string userOid);
        Task<Person> AddPersonAsync(Person person);
        Task<Person?> FindByMobileNumberAndNameAsync(string mobileNumber, string givenName, string surname);
        IEnumerable<string> GetAllNotInDb(IEnumerable<string> oids);
        Task<Person?> FindByMobileNumberAsync(string mobileNumber);
        Task<Person?> FindByFullNameAsync(string firstName, string lastName);
        Task<Person?> FindByEmailAsync(string userEmail);
        IEnumerable<string> GetOidsBasedOnProject(long projectId);
        Task<bool> SomePersonBasedOnUserNameExists(string userName);
        Task<Person> FindAsync(long personId);
    }
}