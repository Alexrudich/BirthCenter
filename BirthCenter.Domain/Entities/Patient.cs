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
            string? family = null,
            DateTime? birthDate = null,
            Gender? gender = null,
            bool? active = null,
            string? use = null,
            List<string>? given = null)
        {
            UpdateFamily(family);
            UpdateUse(use);
            UpdateBirthDate(birthDate);
            UpdateGender(gender);
            UpdateActive(active);
            UpdateGiven(given);
        }

        private void UpdateFamily(string? newFamily)
        {
            if (newFamily == null) return;

            ValidateFamily(newFamily);
            Family = newFamily;
        }

        private void UpdateUse(string? newUse)
        {
            if (newUse == null) return;

            ValidateUse(newUse);
            Use = newUse;
        }

        private void UpdateBirthDate(DateTime? newBirthDate)
        {
            if (newBirthDate.HasValue)
                BirthDate = newBirthDate.Value;
        }

        private void UpdateGender(Gender? newGender)
        {
            if (newGender.HasValue)
                Gender = newGender.Value;
        }

        private void UpdateActive(bool? newActive)
        {
            if (newActive.HasValue)
                Active = newActive.Value;
        }

        private void UpdateGiven(List<string>? newGiven)
        {
            if (newGiven != null)
                Given = newGiven.AsReadOnly();
        }

        private static void ValidateFamily(string family)
        {
            if (string.IsNullOrWhiteSpace(family))
                throw new ArgumentException("Family cannot be empty", nameof(family));
        }

        private static void ValidateUse(string use)
        {
            if (string.IsNullOrWhiteSpace(use))
                throw new ArgumentException("Use cannot be empty", nameof(use));
        }
    }
}