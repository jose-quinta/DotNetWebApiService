using System;
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