# Servicios en .Net Core

Primero creamos la carpeta del proyecto de prueba o lo que quieran hacer.
Para este ejemplo le puse el nombre de DotNetWebApiService.

en mi caso como esto desde visual studio code, lo hare desde la consola con dotnet ef.
Si no tiene dotnet ef instalado usa:
    dotnet tool install --global dotnet-ef
Si tiene instalado dotnet ef actualizarlo por prevención:
    dotnet tool update --global dotnet-ef
Luego se abre con el editor de código de su preferencia, donde se crear el api (Server).
    dotnet new webapi -o Server


Una vez creada el api, procedemos a crear las carpetas de entidades (person, personDto, TypePersonClass (enum) ), data (ApplicationDbContext) y services (PersonServices).
Crearemos los modelos o entidades que vamos a usar en la carpeta de entidades.
TypePersonClass
~~~
    sing System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.Json.Serialization;
    using System.Threading.Tasks;

    namespace Server.Entities
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum TypePersonClass
        {
            Student = 1,
            Teacher = 2,
            Policeman = 3
        }
    }
~~~

Person
~~~
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Threading.Tasks;

    namespace Server.Entities {
        public class Person {
            [Key]
            public int Id { get; set; }
            [Required]
            public string Name { get; set; } = string.Empty;
            [Required]
            public string Lastname { get; set; } = string.Empty;
            [Required]
            public int Years { get; set; }
            public TypePersonClass Class { get; set; } = TypePersonClass.Student;
            public DateTime CreatedAt { get; set; } = new DateTime();
            public DateTime UpdatedAt { get; set; }
        }
    }
~~~

PersonDto
~~~
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Threading.Tasks;

    namespace Server.Entities {
        public class PersonDto {
            [Required]
            public string Name { get; set; } = string.Empty;
            [Required]
            public string Lastname { get; set; } = string.Empty;
            [Required]
            public int Years { get; set; }
            public TypePersonClass Class { get; set; } = TypePersonClass.Student;
        }
    }
~~~

Luego procedemos a instalar los paquetes para conectar a la base de datos, en mi caso se utilizará
EntityFrameworkCore
    dotnet package add Microsoft.EntityFrameworkCore
    dotnet package add Microsoft.EntityFrameworkCore.Design
    dotnet package add Microsoft.EntityFrameworkCore.SqlServer
    dotnet package add Microsoft.EntityFrameworkCore.Tools


Una vez instalados los paquetes procedemos a configurar el appsettings.json
~~~
    {
        "ConnectionStrings": {
            "DefaultConnection": "Server=localhost;Database=DotNetCoreService;Trusted_Connection=true"
        },
        "Logging": {
            "LogLevel": {
            "Default": "Information",
            "Microsoft.AspNetCore": "Warning"
            }
        },
        "AllowedHosts": "*"
    }
~~~

Luego seguimos con el ApplicationDbContext para poder hacer uso de los DbSet.
~~~
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Server.Entities;

    namespace Server.Data {
        public class ApplicationDbContext : DbContext {
            public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options) { }
            public DbSet<Person> People { get; set; } = default!;
        }
    }
~~~

Luego de hacer la configuración del archivo .json y el ApplicationDbContext, procedemos a configurar el
archivo de Program.cs
~~~
    builder.Services.AddDbContext<ApplicationDbContext>(
        options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
    );
~~~

Una vez de haber terminado las configuraciones se comienza con las migraciones.
    dotnet ef migrations add Initial

Una vez hecha la migración nos crear la carpeta de migraciones donde estarán las migraciones que
luego con el comando para actualizar la base de datos no creara la base de datos con la tabla.
    dotnet ef database update

Ya una vez hecha la migración a base de datos, continuaremos con la creación de la interfaz del servicio
de person que luego usaremos en el controlador ya que los servicios son para aumentar la seguridad de
los datos que pasamos por los controladores (Realmente eso es lo que entiendo y si quiere una
explicación razonable esta San Google).

En la carpeta de Services > PersonServices
IPersonService
~~~
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
~~~

PersonService
~~~
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
~~~

Luego de haber hecho el servicio de persona podemos configurar el Program.cs para que se pueda usar el servicio por los controladores.
~~~
    builder.Services.AddScoped<IPersonService, PersonService>();
~~~

Una vez terminado el servicio de persona continuamos con la creación del controlador de Persona
~~~
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Server.Entities;
    using Server.Services.PersonServices;

    namespace Server.Controllers {
        [ApiController]
        [Route("api/[controller]")]
        public class PersonController : ControllerBase {
            private readonly IPersonService _service;

            public PersonController(IPersonService service) {
                _service = service;
            }
            [HttpGet]
            public async Task<ActionResult<List<Person>>> Get() {
                return Ok(await _service.Get());
            }
            [HttpGet("{id}")]
            public async Task<ActionResult<Person>> GetById(int id) {
                return Ok(await _service.GetById(id));
            }
            [HttpPost]
            public async Task<ActionResult<List<Person>>> Post(PersonDto request) {
                return Ok(await _service.Post(request));
            }
            [HttpPut("{id}")]
            public async Task<ActionResult<Person>> Put(int id, PersonDto request) {
                return Ok(await _service.Put(id, request));
            }
            [HttpDelete("{id}")]
            public async Task<ActionResult<Person>> Delete(int id) {
                return Ok(await _service.Delete(id));
            }
        }
    }
~~~

### Gracias por su atención!!!
Espero que le sirva de guía para los que son los servicios, hay mejores forma de hacerlos pero esta fue la primera que se me vino a la mente (Bye bye).

[Markdown](https://markdown.es/sintaxis-markdown/ "Fuente que es de mucha ayuda")