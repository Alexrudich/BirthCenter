using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BirthCenter.Domain.Entities;
using BirthCenter.Domain.Enums;
using BirthCenter.Infrastructure.Data;

namespace BirthCenter.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeedController : ControllerBase
    {
        private readonly AppDbContext _context;
        private const int TargetPatientCount = 100;

        public SeedController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Populates the database with test data (100 patients)
        /// </summary>
        /// <returns>Number of created patients</returns>
        /// <response code="200">Returns the number of added patients</response>
        /// <response code="400">If database already has 100 or more patients</response>
        [HttpPost("generate")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<string>> GenerateTestData()
        {
            var existingCount = await _context.Patients.CountAsync();

            if (existingCount >= TargetPatientCount)
            {
                return BadRequest($"Database already has {existingCount} patients. " +
                                 $"Target is {TargetPatientCount}. Test data not added.");
            }

            var needed = TargetPatientCount - existingCount;
            var patients = GeneratePatients(needed);

            await _context.Patients.AddRangeAsync(patients);
            await _context.SaveChangesAsync();

            return Ok($"Added {needed} test patients. Total in database: {TargetPatientCount}");
        }

        /// <summary>
        /// Removes all test data from the database
        /// </summary>
        /// <returns>Number of deleted patients</returns>
        [HttpDelete("clear")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<ActionResult<string>> ClearTestData()
        {
            var patients = await _context.Patients.ToListAsync();
            var count = patients.Count;

            _context.Patients.RemoveRange(patients);
            await _context.SaveChangesAsync();

            return Ok($"Deleted {count} patients from database");
        }

        /// <summary>
        /// Gets the current number of patients in the database
        /// </summary>
        /// <returns>Number of patients</returns>
        [HttpGet("count")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<ActionResult<int>> GetCount()
        {
            var count = await _context.Patients.CountAsync();
            return Ok(count);
        }

        private static List<Patient> GeneratePatients(int count)
        {
            var rand = new Random();

            var firstNames = new[] { "Ivan", "Petr", "Sidor", "Alexey", "Dmitry", "Andrey", "Mikhail", "Nikolay" };
            var lastNames = new[] { "Ivanov", "Petrov", "Sidorov", "Alexeev", "Dmitriev", "Andreev", "Mikhailov", "Nikolaev" };
            var patronymics = new[] { "Ivanovich", "Petrovich", "Sidorovich", "Alexeevich", "Dmitrievich", "Andreevich", "Mikhailovich", "Nikolaevich" };

            var patients = new List<Patient>();

            for (int i = 0; i < count; i++)
            {
                var firstName = firstNames[rand.Next(firstNames.Length)];
                var lastName = lastNames[rand.Next(lastNames.Length)];
                var patronymic = patronymics[rand.Next(patronymics.Length)];

                var gender = rand.Next(4) switch
                {
                    0 => Gender.Male,
                    1 => Gender.Female,
                    2 => Gender.Other,
                    _ => Gender.Unknown
                };

                var year = 2023 + rand.Next(2);
                var month = rand.Next(1, 13);
                var day = rand.Next(1, DateTime.DaysInMonth(year, month) + 1);
                var hour = rand.Next(0, 24);
                var minute = rand.Next(0, 60);
                var second = rand.Next(0, 60);

                var birthDate = new DateTime(year, month, day, hour, minute, second, DateTimeKind.Utc);

                var patient = new Patient(
                    family: lastName,
                    birthDate: birthDate,
                    gender: gender,
                    active: rand.Next(100) > 10,
                    use: "official",
                    given: new List<string> { firstName, patronymic }
                );

                patients.Add(patient);
            }

            return patients;
        }
    }
}