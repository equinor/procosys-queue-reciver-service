﻿using QueueReceiver.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QueueReceiver.Core.Interfaces
{
    public interface IPersonRepository
    {
        Task<Person?> FindByUserOid(string userOid);
        //Task<Person> FindByUserEmail(string userEmail);
        Task<Person> FindByUsername(string userName);
        Task<Person> AddPerson(Person person);
        void Update(Person person);
        Task<int> SaveChangesAsync();
        Task<Person?> FindByNameAndMobileNumber(string mobileNumber, string givenName, string surname);
        IEnumerable<string> GetAllNotInDb(IEnumerable<string> oids);
    }
}