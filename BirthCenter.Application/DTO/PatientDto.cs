using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BirthCenter.Application.DTO
{
    public class PatientDto
    {
        public Guid Id { get; set; }
        public PatientNameDto Name { get; set; }
        public string Gender { get; set; }
        public DateTime BirthDate { get; set; }
        public bool Active { get; set; }
    }

    public class PatientNameDto
    {
        public string Use { get; set; } = "official";  // Значение по умолчанию
        public string Family { get; set; } = "Иванов"; // Пример
        public List<string> Given { get; set; } = new List<string> { "Иван", "Иванович" }; // Пример
    }
}
