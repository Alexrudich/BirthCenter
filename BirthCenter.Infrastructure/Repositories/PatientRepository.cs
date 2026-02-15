using BirthCenter.Application.Interfaces;
using BirthCenter.Domain.Entities;
using BirthCenter.Domain.Enums;
using BirthCenter.Domain.Specifications;
using BirthCenter.Infrastructure.Data;
using BirthCenter.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace BirthCenter.Infrastructure.Repositories
{
    public class PatientRepository : IPatientRepository
    {
        private readonly AppDbContext _context;

        public PatientRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Patient> GetByIdAsync(Guid id)
        {
            return await _context.Patients.FindAsync(id);
        }

        public async Task<IEnumerable<Patient>> GetAllAsync()
        {
            return await _context.Patients.ToListAsync();
        }

        public async Task<IEnumerable<Patient>> SearchByBirthDateAsync(string dateSearchParam)
        {
            var criteria = FhirDateParser.Parse(dateSearchParam);
            var query = _context.Patients.AsQueryable();

            query = criteria.IsRange
                ? ApplyRangeFilter(query, criteria)
                : ApplyExactDateFilter(query, criteria);

            return await query.ToListAsync();
        }

        private static IQueryable<Patient> ApplyRangeFilter(IQueryable<Patient> query, DateSearchCriteria criteria)
        {
            if (!criteria.StartDate.HasValue || !criteria.EndDate.HasValue)
                return query;

            return criteria.Prefix switch
            {
                DatePrefix.Eq =>
                    query.Where(p => p.BirthDate >= criteria.StartDate && p.BirthDate <= criteria.EndDate),

                DatePrefix.Ne =>
                    query.Where(p => p.BirthDate < criteria.StartDate || p.BirthDate > criteria.EndDate),

                DatePrefix.Gt =>
                    query.Where(p => p.BirthDate > criteria.EndDate),

                DatePrefix.Lt =>
                    query.Where(p => p.BirthDate < criteria.StartDate),

                DatePrefix.Ge =>
                    query.Where(p => p.BirthDate >= criteria.StartDate),

                DatePrefix.Le =>
                    query.Where(p => p.BirthDate <= criteria.EndDate),

                _ => query.Where(p => p.BirthDate >= criteria.StartDate && p.BirthDate <= criteria.EndDate)
            };
        }

        private static IQueryable<Patient> ApplyExactDateFilter(IQueryable<Patient> query, DateSearchCriteria criteria)
        {
            if (!criteria.ExactDate.HasValue)
                return query;

            var date = criteria.ExactDate.Value;

            return criteria.Prefix switch
            {
                DatePrefix.Eq =>
                    query.Where(p => p.BirthDate.Date == date.Date),

                DatePrefix.Ne =>
                    query.Where(p => p.BirthDate.Date != date.Date),

                DatePrefix.Gt =>
                    query.Where(p => p.BirthDate > date),

                DatePrefix.Lt =>
                    query.Where(p => p.BirthDate < date),

                DatePrefix.Ge =>
                    query.Where(p => p.BirthDate >= date),

                DatePrefix.Le =>
                    query.Where(p => p.BirthDate <= date),

                _ => query.Where(p => p.BirthDate.Date == date.Date)
            };
        }

        public async Task<Patient> AddAsync(Patient patient)
        {
            await _context.Patients.AddAsync(patient);
            await _context.SaveChangesAsync();
            return patient;
        }

        public async Task UpdateAsync(Patient patient)
        {
            _context.Patients.Update(patient);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var patient = await GetByIdAsync(id);
            if (patient != null)
            {
                _context.Patients.Remove(patient);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Patients.AnyAsync(p => p.Id == id);
        }
    }
}