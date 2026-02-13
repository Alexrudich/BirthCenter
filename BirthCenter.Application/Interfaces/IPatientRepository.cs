using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BirthCenter.Domain.Entities;

namespace BirthCenter.Application.Interfaces
{
    public interface IPatientRepository
    {
        Task<Patient> GetByIdAsync(Guid id);
        Task<IEnumerable<Patient>> GetAllAsync();
        Task<IEnumerable<Patient>> SearchByBirthDateAsync(string dateSearchParam);
        Task<Patient> AddAsync(Patient patient);
        Task UpdateAsync(Patient patient);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
    }
}