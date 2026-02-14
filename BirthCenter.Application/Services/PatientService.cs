using AutoMapper;
using BirthCenter.Application.DTO;
using BirthCenter.Application.Interfaces;
using BirthCenter.Domain.Entities;
using BirthCenter.Domain.Enums;
using BirthCenter.Domain.Exceptions;

namespace BirthCenter.Application.Services
{
    /// <summary>
    /// Service for managing Patient domain operations
    /// </summary>
    public class PatientService : IPatientService
    {
        private readonly IPatientRepository _repository;
        private readonly IMapper _mapper;

        public PatientService(IPatientRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves a patient by their unique identifier
        /// </summary>
        /// <param name="id">Patient's GUID</param>
        /// <returns>Patient DTO if found</returns>
        /// <exception cref="NotFoundException">Thrown when patient doesn't exist</exception>
        public async Task<PatientDto> GetByIdAsync(Guid id)
        {
            var patient = await _repository.GetByIdAsync(id);
            if (patient == null)
                throw new NotFoundException($"Patient with id {id} not found");

            return _mapper.Map<PatientDto>(patient);
        }

        /// <summary>
        /// Retrieves all patients from the system
        /// </summary>
        /// <returns>Collection of patient DTOs</returns>
        public async Task<IEnumerable<PatientDto>> GetAllAsync()
        {
            var patients = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<PatientDto>>(patients);
        }

        /// <summary>
        /// Searches for patients by birth date using FHIR-compliant syntax
        /// </summary>
        /// <param name="birthDate">FHIR date parameter (e.g., "2024", "2024-01", "gt2024-01-01")</param>
        /// <returns>Collection of patients matching the date criteria</returns>
        public async Task<IEnumerable<PatientDto>> SearchByBirthDateAsync(string birthDate)
        {
            var patients = await _repository.SearchByBirthDateAsync(birthDate);
            return _mapper.Map<IEnumerable<PatientDto>>(patients);
        }

        /// <summary>
        /// Creates a new patient from the request data
        /// </summary>
        /// <param name="request">Patient creation request</param>
        /// <returns>Created patient DTO</returns>
        /// <exception cref="ValidationException">Thrown when required fields are missing</exception>
        /// <remarks>
        /// - Family name is required
        /// - Converts gender string to Gender enum (case-insensitive)
        /// - Ensures DateTime is in UTC for PostgreSQL compatibility
        /// </remarks>
        public async Task<PatientDto> CreateAsync(CreatePatientRequest request)
        {
            // Family name is required by domain rules
            if (request.Name?.Family == null)
                throw new ValidationException("Family name is required");

            // Parse gender string to enum (case-insensitive, defaults to Unknown)
            var gender = Enum.TryParse<Gender>(request.Gender, true, out var g)
                ? g : Gender.Unknown;

            // PostgreSQL requires DateTime with Kind = Utc
            // See: https://www.npgsql.org/doc/types/datetime.html
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

        /// <summary>
        /// Updates an existing patient with partial data
        /// </summary>
        /// <param name="id">Patient ID to update</param>
        /// <param name="request">Update request with optional fields</param>
        /// <returns>Updated patient DTO</returns>
        /// <exception cref="NotFoundException">Thrown when patient doesn't exist</exception>
        /// <remarks>
        /// Only fields that are provided in the request will be updated.
        /// Gender string is converted to enum if provided.
        /// </remarks>
        public async Task<PatientDto> UpdateAsync(Guid id, UpdatePatientRequest request)
        {
            var patient = await _repository.GetByIdAsync(id);
            if (patient == null)
                throw new NotFoundException($"Patient with id {id} not found");

            // Extract values only if they are provided in the request
            var family = request.Name?.Family;

            DateTime? birthDate = null;
            if (request.BirthDate.HasValue)
            {
                birthDate = DateTime.SpecifyKind(request.BirthDate.Value, DateTimeKind.Utc);
            }

            // Parse gender only if it was provided
            Gender? gender = request.Gender != null
                ? Enum.Parse<Gender>(request.Gender, true)
                : null;

            var active = request.Active;
            var use = request.Name?.Use;
            var given = request.Name?.Given;

            patient.Update(family, birthDate, gender, active, use, given);

            await _repository.UpdateAsync(patient);
            return _mapper.Map<PatientDto>(patient);
        }

        /// <summary>
        /// Deletes a patient from the system
        /// </summary>
        /// <param name="id">Patient ID to delete</param>
        /// <exception cref="NotFoundException">Thrown when patient doesn't exist</exception>
        public async Task DeleteAsync(Guid id)
        {
            if (!await _repository.ExistsAsync(id))
                throw new NotFoundException($"Patient with id {id} not found");

            await _repository.DeleteAsync(id);
        }
    }
}