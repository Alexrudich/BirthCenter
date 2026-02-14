
namespace BirthCenter.Application.DTO
{
    public class PatientDto
    {
        public Guid Id { get; set; }
        public PatientNameDto Name { get; set; }
        public string? Gender { get; set; }
        public DateTime BirthDate { get; set; }
        public bool Active { get; set; }
    }

    public class PatientNameDto
    {
        public string? Use { get; set; }
        public string? Family { get; set; }
        public List<string>? Given { get; set; }
    }
}
