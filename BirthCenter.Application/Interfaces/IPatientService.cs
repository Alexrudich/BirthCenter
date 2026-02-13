using BirthCenter.Application.DTO;

namespace BirthCenter.Application.Interfaces
{
    public interface IPatientService
    {
        Task<PatientDto> GetByIdAsync(Guid id);
        Task<IEnumerable<PatientDto>> GetAllAsync();
        Task<IEnumerable<PatientDto>> SearchByBirthDateAsync(string birthDate);
        Task<PatientDto> CreateAsync(CreatePatientRequest request);
        Task<PatientDto> UpdateAsync(Guid id, UpdatePatientRequest request);
        Task DeleteAsync(Guid id);
    }
}
