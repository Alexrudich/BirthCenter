using BirthCenter.Domain.Entities;
using BirthCenter.Domain.Enums;
using FluentAssertions;
using Xunit;

namespace BirthCenter.Tests.Domain
{
    public class PatientTests
    {
        [Fact]
        public void CreatePatient_WithValidData_ShouldSetPropertiesCorrectly()
        {
            // Arrange
            var family = "Иванов";
            var birthDate = new DateTime(2024, 1, 13, 18, 25, 43, DateTimeKind.Utc);
            var gender = Gender.Male;
            var given = new List<string> { "Иван", "Иванович" };

            // Act
            var patient = new Patient(family, birthDate, gender, true, "official", given);

            // Assert
            patient.Family.Should().Be(family);
            patient.BirthDate.Should().Be(birthDate);
            patient.Gender.Should().Be(gender);
            patient.Active.Should().BeTrue();
            patient.Use.Should().Be("official");
            patient.Given.Should().BeEquivalentTo(given);
            patient.Id.Should().NotBeEmpty();
        }

        [Fact]
        public void CreatePatient_WithoutFamily_ShouldThrowArgumentException()
        {
            // Arrange
            var birthDate = new DateTime(2024, 1, 13).ToUniversalTime();

            // Act
            Action act = () => new Patient(null!, birthDate);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("Family is required*");
        }

        [Fact]
        public void UpdatePatient_WithNewFamily_ShouldChangeFamily()
        {
            // Arrange
            var patient = new Patient("Иванов", DateTime.UtcNow);
            var newFamily = "Петров";

            // Act
            patient.Update(family: newFamily);

            // Assert
            patient.Family.Should().Be(newFamily);
        }

        [Fact]
        public void UpdatePatient_WithEmptyFamily_ShouldThrowException()
        {
            // Arrange
            var patient = new Patient("Иванов", DateTime.UtcNow);

            // Act
            var act = () => patient.Update(family: "");

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("Family cannot be empty*");
        }

        [Fact]
        public void Given_WhenCreatedAsNull_ShouldInitializeEmptyList()
        {
            // Arrange & Act
            var patient = new Patient("Иванов", DateTime.UtcNow);

            // Assert
            patient.Given.Should().NotBeNull();
            patient.Given.Should().BeEmpty();
        }

        [Fact]
        public void Given_WhenUpdated_ShouldReplaceList()
        {
            // Arrange
            var patient = new Patient("Иванов", DateTime.UtcNow);
            var newGiven = new List<string> { "Петр", "Петрович" };

            // Act
            patient.Update(given: newGiven);

            // Assert
            patient.Given.Should().BeEquivalentTo(newGiven);
        }
    }
}