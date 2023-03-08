using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Entities;

namespace Server.Services.PersonServices {
    public class PersonService : IPersonService {
        private readonly ApplicationDbContext _context;

        public PersonService(ApplicationDbContext context) {
            _context = context;
        }
        public async Task<List<Person>> Get() {
            return await _context.People.ToListAsync();
        }
        public async Task<Person?> GetById(int id) {
            var response = await _context.People.FindAsync(id);

            if (response == null)
                return null;

            return response!;
        }
        public async Task<List<Person>> Post(PersonDto request) {
            Person person = new Person(){
                Name = request.Name,
                Lastname = request.Lastname,
                Years = request.Years,
                Class = request.Class,
                CreatedAt = DateTime.Now,
            };

            _context.Add(person);
            await _context.SaveChangesAsync();

            return await _context.People.ToListAsync();
        }
        public async Task<Person?> Put(int id, PersonDto request) {
            var response = await _context.People.FindAsync(id);

            if (response == null)
                return null;

            response.Name = request.Name;
            response.Lastname = request.Lastname;
            response.Years = request.Years;
            response.Class = request.Class;
            response.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return response;
        }
        public async Task<Person?> Delete(int id) {
            var response = await _context.People.FindAsync(id);

            if (response == null)
                return null;

            _context.Remove(response);
            await _context.SaveChangesAsync();

            return response;
        }
    }
}