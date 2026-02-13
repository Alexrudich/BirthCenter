using AutoMapper;
using BirthCenter.Application.DTO;
using BirthCenter.Domain.Entities;

namespace BirthCenter.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Patient -> PatientDto
            CreateMap<Patient, PatientDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => new PatientNameDto
                {
                    Use = src.Use,
                    Family = src.Family,
                    Given = src.Given
                }))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender.ToString().ToLower()));

            // CreatePatientRequest -> не маппим напрямую, используем конструктор Patient
            // UpdatePatientRequest -> не маппим напрямую, используем метод Update
        }
    }
}