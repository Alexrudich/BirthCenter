using BirthCenter.Application.Interfaces;
using BirthCenter.Domain.Entities;
using BirthCenter.Infrastructure.Data;
using BirthCenter.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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

            if (criteria.IsRange && criteria.StartDate.HasValue && criteria.EndDate.HasValue)
            {
                // Диапазон для частичных дат
                switch (criteria.Prefix)
                {
                    case "eq":
                        query = query.Where(p => p.BirthDate >= criteria.StartDate &&
                                                p.BirthDate <= criteria.EndDate);
                        break;
                    case "ne":
                        query = query.Where(p => p.BirthDate < criteria.StartDate ||
                                                p.BirthDate > criteria.EndDate);
                        break;
                    case "gt":
                        query = query.Where(p => p.BirthDate > criteria.EndDate);
                        break;
                    case "lt":
                        query = query.Where(p => p.BirthDate < criteria.StartDate);
                        break;
                    case "ge":
                        query = query.Where(p => p.BirthDate >= criteria.StartDate);
                        break;
                    case "le":
                        query = query.Where(p => p.BirthDate <= criteria.EndDate);
                        break;
                    default:
                        query = query.Where(p => p.BirthDate >= criteria.StartDate &&
                                                p.BirthDate <= criteria.EndDate);
                        break;
                }
            }
            else if (criteria.ExactDate.HasValue)
            {
                // Точная дата
                var date = criteria.ExactDate.Value;

                switch (criteria.Prefix)
                {
                    case "eq":
                        query = query.Where(p => p.BirthDate.Date == date.Date);
                        break;
                    case "ne":
                        query = query.Where(p => p.BirthDate.Date != date.Date);
                        break;
                    case "gt":
                        query = query.Where(p => p.BirthDate > date);
                        break;
                    case "lt":
                        query = query.Where(p => p.BirthDate < date);
                        break;
                    case "ge":
                        query = query.Where(p => p.BirthDate >= date);
                        break;
                    case "le":
                        query = query.Where(p => p.BirthDate <= date);
                        break;
                    default:
                        query = query.Where(p => p.BirthDate.Date == date.Date);
                        break;
                }
            }

            return await query.ToListAsync();
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