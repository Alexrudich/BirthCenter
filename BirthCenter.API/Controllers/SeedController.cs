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

        public SeedController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Наполняет БД тестовыми данными (100 пациентов)
        /// </summary>
        /// <returns>Количество созданных пациентов</returns>
        [HttpPost("generate")]
        public async Task<ActionResult<int>> GenerateTestData()
        {
            var existingCount = await _context.Patients.CountAsync();

            if (existingCount >= 100)
            {
                return Ok($"В БД уже {existingCount} пациентов. Тестовые данные не добавлены.");
            }

            var needed = 100 - existingCount;
            var patients = GeneratePatients(needed);

            await _context.Patients.AddRangeAsync(patients);
            await _context.SaveChangesAsync();

            return Ok($"Добавлено {needed} тестовых пациентов. Всего в БД: 100");
        }

        /// <summary>
        /// Удаляет все тестовые данные
        /// </summary>
        [HttpDelete("clear")]
        public async Task<IActionResult> ClearTestData()
        {
            var patients = await _context.Patients.ToListAsync();
            _context.Patients.RemoveRange(patients);
            await _context.SaveChangesAsync();

            return Ok($"Удалено {patients.Count} пациентов");
        }

        /// <summary>
        /// Проверяет текущее количество пациентов
        /// </summary>
        [HttpGet("count")]
        public async Task<ActionResult<int>> GetCount()
        {
            var count = await _context.Patients.CountAsync();
            return Ok(count);
        }

        private List<Patient> GeneratePatients(int count)
        {
            var rand = new Random();

            var firstNames = new[] { "Иван", "Петр", "Сидор", "Алексей", "Дмитрий", "Андрей", "Михаил", "Николай" };
            var lastNames = new[] { "Иванов", "Петров", "Сидоров", "Алексеев", "Дмитриев", "Андреев", "Михайлов", "Николаев" };
            var patronymics = new[] { "Иванович", "Петрович", "Сидорович", "Алексеевич", "Дмитриевич", "Андреевич", "Михайлович", "Николаевич" };

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