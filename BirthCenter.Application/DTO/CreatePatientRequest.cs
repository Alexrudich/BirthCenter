using System.ComponentModel.DataAnnotations;

namespace BirthCenter.Application.DTO
{
    public class CreatePatientRequest
    {
        [Required]
        public PatientNameDto Name { get; set; }

        [Required]
        public DateTime BirthDate { get; set; }

        public string Gender { get; set; } = "unknown";

        public bool Active { get; set; } = true;
    }
}