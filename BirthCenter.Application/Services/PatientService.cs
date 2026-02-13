using AutoMapper;
using BirthCenter.Application.DTO;
using BirthCenter.Application.Interfaces;
using BirthCenter.Domain.Entities;
using BirthCenter.Domain.Enums;
using BirthCenter.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BirthCenter.Application.Services
{
    public class PatientService : IPatientService
    {
        private readonly IPatientRepository _repository;
        private readonly IMapper _mapper;

        public PatientService(IPatientRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<PatientDto> GetByIdAsync(Guid id)
        {
            var patient = await _repository.GetByIdAsync(id);
            if (patient == null)
                throw new NotFoundException($"Patient with id {id} not found");

            return _mapper.Map<PatientDto>(patient);
        }

        public async Task<IEnumerable<PatientDto>> GetAllAsync()
        {
            var patients = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<PatientDto>>(patients);
        }

        public async Task<IEnumerable<PatientDto>> SearchByBirthDateAsync(string birthDate)
        {
            var patients = await _repository.SearchByBirthDateAsync(birthDate);
            return _mapper.Map<IEnumerable<PatientDto>>(patients);
        }

        public async Task<PatientDto> CreateAsync(CreatePatientRequest request)
        {
            // Валидация
            if (request.Name?.Family == null)
                throw new ValidationException("Family name is required");

            // Маппинг запроса в доменную модель
            var gender = Enum.TryParse<Gender>(request.Gender, true, out var g)
                ? g : Gender.Unknown;

            // Любые DateTime без принудительного UTC
            var utcBirthDate = DateTime.SpecifyKind(request.BirthDate, DateTimeKind.Utc);

            var patient = new Patient(
                family: request.Name.Family,
                birthDate: utcBirthDate,
                gender: gender,
                active: request.Active,
                use: request.Name?.Use ?? "official",
                given: request.Name?.Given ?? new List<string>()
            );

            var created = await _repository.AddAsync(patient);
            return _mapper.Map<PatientDto>(created);
        }

        public async Task<PatientDto> UpdateAsync(Guid id, UpdatePatientRequest request)
        {
            var patient = await _repository.GetByIdAsync(id);
            if (patient == null)
                throw new NotFoundException($"Patient with id {id} not found");

            // Обновляем только переданные поля
            string family = request.Name?.Family;
            DateTime? birthDate = request.BirthDate;
            Gender? gender = request.Gender != null
                ? Enum.Parse<Gender>(request.Gender, true)
                : null;
            bool? active = request.Active;
            string use = request.Name?.Use;
            List<string> given = request.Name?.Given;

            patient.Update(family, birthDate, gender, active, use, given);

            await _repository.UpdateAsync(patient);
            return _mapper.Map<PatientDto>(patient);
        }

        public async Task DeleteAsync(Guid id)
        {
            if (!await _repository.ExistsAsync(id))
                throw new NotFoundException($"Patient with id {id} not found");

            await _repository.DeleteAsync(id);
        }
    }
}