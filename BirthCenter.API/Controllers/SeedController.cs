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
        private const int MinYear = 2023;
        private const int MaxYear = 2025;
        private const int ActiveThreshold = 90; // 90% active patients
        private const int HoursInDay = 24;
        private const int MinutesInHour = 60;
        private const int SecondsInMinute = 60;

        public SeedController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Populates the database with test data (100 patients) in Russian locale
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
            var patients = GenerateRussianPatients(needed);

            await _context.Patients.AddRangeAsync(patients);
            await _context.SaveChangesAsync();

            return Ok($"Added {needed} test patients. Total in database: {TargetPatientCount}");
        }

        /// <summary>
        /// Ensures database has exactly 100 patients (generates if needed)
        /// </summary>
        /// <returns>Status message</returns>
        [HttpPost("ensure")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<ActionResult<string>> EnsureTestData()
        {
            var existingCount = await _context.Patients.CountAsync();

            if (existingCount >= TargetPatientCount)
            {
                return Ok($"Database already has {existingCount} patients. No action needed.");
            }

            var needed = TargetPatientCount - existingCount;
            var patients = GenerateRussianPatients(needed);

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

        private static List<Patient> GenerateRussianPatients(int count)
        {
            var patients = new List<Patient>();

            patients.AddRange(GenerateGuaranteedDemoPatients());
            patients.AddRange(GenerateRandomPatients(count - patients.Count));

            return patients;
        }

        private static IEnumerable<Patient> GenerateGuaranteedDemoPatients()
        {
            return new List<Patient>
            {
                new(
                    family: "Иванов",
                    birthDate: new DateTime(2024, 1, 13, 18, 25, 43, DateTimeKind.Utc),
                    gender: Gender.Male,
                    active: true,
                    use: "official",
                    given: new List<string> { "Иван", "Иванович" }
                ),
                new(
                    family: "Петров",
                    birthDate: new DateTime(2023, 5, 15, 10, 30, 0, DateTimeKind.Utc),
                    gender: Gender.Male,
                    active: true,
                    use: "official",
                    given: new List<string> { "Петр", "Петрович" }
                ),
                new(
                    family: "Сидорова",
                    birthDate: new DateTime(2024, 8, 20, 14, 45, 0, DateTimeKind.Utc),
                    gender: Gender.Female,
                    active: true,
                    use: "official",
                    given: new List<string> { "Анна", "Сидоровна" }
                ),
                new(
                    family: "Алексеев",
                    birthDate: new DateTime(2025, 3, 10, 9, 15, 0, DateTimeKind.Utc),
                    gender: Gender.Male,
                    active: false,
                    use: "official",
                    given: new List<string> { "Алексей", "Алексеевич" }
                )
            };
        }

        private static List<Patient> GenerateRandomPatients(int count)
        {
            var rand = new Random();
            var patients = new List<Patient>();

            var firstNames = new[] { "Иван", "Петр", "Сидор", "Алексей", "Дмитрий", "Андрей", "Михаил", "Николай" };
            var lastNames = new[] { "Иванов", "Петров", "Сидоров", "Алексеев", "Дмитриев", "Андреев", "Михайлов", "Николаев" };
            var patronymics = new[] { "Иванович", "Петрович", "Сидорович", "Алексеевич", "Дмитриевич", "Андреевич", "Михайлович", "Николаевич" };

            for (var i = 0; i < count; i++)
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

                var year = rand.Next(MinYear, MaxYear + 1);
                var month = rand.Next(1, 13);
                var day = rand.Next(1, DateTime.DaysInMonth(year, month) + 1);
                var hour = rand.Next(0, HoursInDay);
                var minute = rand.Next(0, MinutesInHour);
                var second = rand.Next(0, SecondsInMinute);

                var birthDate = new DateTime(year, month, day, hour, minute, second, DateTimeKind.Utc);

                var patient = new Patient(
                    family: lastName,
                    birthDate: birthDate,
                    gender: gender,
                    active: rand.Next(100) < ActiveThreshold,
                    use: "official",
                    given: new List<string> { firstName, patronymic }
                );

                patients.Add(patient);
            }

            return patients;
        }
    }
}