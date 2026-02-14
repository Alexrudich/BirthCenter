using BirthCenter.Domain.Enums;

namespace BirthCenter.Domain.Entities
{
    public class Patient
    {
        public Guid Id { get; private set; }
        public string Family { get; private set; }
        public IReadOnlyList<string> Given { get; private set; }
        public string Use { get; private set; }
        public Gender Gender { get; private set; }
        public DateTime BirthDate { get; private set; }
        public bool Active { get; private set; }

        // For EF Core
        private Patient() { }

        public Patient(
            string family,
            DateTime birthDate,
            Gender gender = Gender.Unknown,
            bool active = true,
            string use = "official",
            List<string>? given = null)
        {
            if (string.IsNullOrWhiteSpace(family))
                throw new ArgumentException("Family is required", nameof(family));

            Id = Guid.NewGuid();
            Family = family;
            BirthDate = birthDate;
            Gender = gender;
            Active = active;
            Use = use;
            Given = (given ?? new List<string>()).AsReadOnly();
        }

        public void Update(
            string family = null,
            DateTime? birthDate = null,
            Gender? gender = null,
            bool? active = null,
            string use = null,
            List<string>? given = null)
        {
            Family = family ?? Family;
            BirthDate = birthDate ?? BirthDate;
            Gender = gender ?? Gender;
            Active = active ?? Active;
            Use = use ?? Use;
            if (given != null)
                Given = given.AsReadOnly();
        }
    }
}