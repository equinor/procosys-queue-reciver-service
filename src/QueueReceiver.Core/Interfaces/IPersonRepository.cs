using QueueReceiver.Core.Models;
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
        IEnumerable<string> GetOidsBasedOnProject(long projectId);
        Task UpdatePersonSettings();
        Task<Person> FindAsync(long personId);
        Task<IEnumerable<Person>> FindPossibleMatches(string mobileNumber, string firstName, string lastName, string userName);
    }
}