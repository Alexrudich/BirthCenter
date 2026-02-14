using Microsoft.EntityFrameworkCore;
using BirthCenter.Domain.Entities;
using BirthCenter.Domain.Enums;
using BirthCenter.Infrastructure.Data;
using Npgsql;

namespace BirthCenter.DataLoader
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Starting DataLoader...");

            // Строка подключения с отключённым SSL
            var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
                                   ?? "Host=localhost;Port=5432;Database=birthcenter;Username=postgres;Password=postgres;SSL Mode=Disable;";

            Console.WriteLine($"Connection string: {connectionString}");

            try
            {
                var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
                optionsBuilder.UseNpgsql(connectionString);

                using var context = new AppDbContext(optionsBuilder.Options);

                // Пробуем открыть соединение вручную для диагностики
                try
                {
                    using var conn = new NpgsqlConnection(connectionString);
                    await conn.OpenAsync();
                    Console.WriteLine("✅ Raw Npgsql connection SUCCESS!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Raw Npgsql connection FAILED: {ex.Message}");
                    return;
                }

                // Проверяем EF Core
                var canConnect = await context.Database.CanConnectAsync();
                Console.WriteLine($"EF Core CanConnect: {canConnect}");

                if (!canConnect)
                {
                    Console.WriteLine("❌ EF Core failed to connect!");
                    return;
                }

                // Считаем пациентов
                var existingCount = await context.Patients.CountAsync();
                Console.WriteLine($"Found {existingCount} existing patients.");

                // Определяем, сколько нужно добавить
                var targetTotal = 100;
                var needed = targetTotal - existingCount;

                if (needed <= 0)
                {
                    Console.WriteLine($"✅ Database already has {existingCount} patients. No new patients needed.");
                    Console.WriteLine($"Target: {targetTotal}, Existing: {existingCount}");
                    return;
                }

                Console.WriteLine($"Need to add {needed} patients to reach target of {targetTotal}.");

                // Спрашиваем подтверждение
                Console.Write($"Generate and save {needed} new patients? (y/n): ");
                var response = Console.ReadLine()?.ToLower();

                if (response != "y" && response != "yes")
                {
                    Console.WriteLine("Operation cancelled.");
                    return;
                }

                Console.WriteLine($"Generating {needed} patients...");
                var patients = GeneratePatients(needed);

                await context.Patients.AddRangeAsync(patients);
                var saved = await context.SaveChangesAsync();

                Console.WriteLine($"✅ Successfully saved {saved} new patients to database!");
                Console.WriteLine($"Total patients now: {existingCount + saved}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Connection error: {ex.Message}");
                if (ex.InnerException != null)
                    Console.WriteLine($"Inner: {ex.InnerException.Message}");
            }
        }

        static List<Patient> GeneratePatients(int count)
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

                if ((i + 1) % 10 == 0)
                    Console.WriteLine($"Generated {i + 1}/{count} patients...");
            }

            return patients;
        }
    }
}