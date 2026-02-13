using System;
using System.Collections.Generic;
using Bogus;
using BirthCenter.DataLoader.Models;

namespace BirthCenter.DataLoader.Services
{
    public class PatientGenerator
    {
        private readonly Faker<GeneratedPatient> _patientFaker;

        public PatientGenerator()
        {
            _patientFaker = new Faker<GeneratedPatient>("ru")
                .RuleFor(p => p.Name, f => new PatientName
                {
                    Family = f.Name.LastName(),
                    Given = new List<string> { f.Name.FirstName(), f.Name.FirstName() },
                    Use = "official"
                })
                .RuleFor(p => p.Gender, f => f.PickRandom(new[] { "male", "female", "other", "unknown" }))
                .RuleFor(p => p.BirthDate, f => f.Date.Past(2)) // дети до 2 лет
                .RuleFor(p => p.Active, f => f.Random.Bool(0.9f)); // 90% active
        }

        public List<GeneratedPatient> Generate(int count)
        {
            return _patientFaker.Generate(count);
        }
    }
}