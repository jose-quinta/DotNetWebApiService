using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Server.Entities;

namespace Server.Services.PersonServices
{
    public interface IPersonService
    {
        Task<List<Person>> Get();
        Task<Person?> GetById(int id);
        Task<List<Person>> Post(PersonDto request);
        Task<Person?> Put(int id, PersonDto request);
        Task<Person?> Delete(int id);
    }
}