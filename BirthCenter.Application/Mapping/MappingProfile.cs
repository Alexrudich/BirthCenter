using AutoMapper;
using BirthCenter.Application.DTO;
using BirthCenter.Domain.Entities;

namespace BirthCenter.Application.Mapping
{
    /// <summary>
    /// AutoMapper profile for domain to DTO mappings
    /// </summary>
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreatePatientMapping();
        }

        /// <summary>
        /// Configures mapping from Patient entity to PatientDto
        /// </summary>
        private void CreatePatientMapping()
        {
            CreateMap<Patient, PatientDto>()
                .ForMember(
                    dest => dest.Name,
                    opt => opt.MapFrom(src => MapPatientName(src)))
                .ForMember(
                    dest => dest.Gender,
                    opt => opt.MapFrom(src => src.Gender.ToString().ToLower()));
        }

        /// <summary>
        /// Maps Patient domain entity to PatientNameDto
        /// </summary>
        /// <param name="source">Patient entity</param>
        /// <returns>PatientNameDto with flattened name structure</returns>
        private static PatientNameDto MapPatientName(Patient source)
        {
            return new PatientNameDto
            {
                Use = source.Use,
                Family = source.Family,
                Given = source.Given.ToList() // Convert IReadOnlyList to List for JSON serialization
            };
        }
    }
}