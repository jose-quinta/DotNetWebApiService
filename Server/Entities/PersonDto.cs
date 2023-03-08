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