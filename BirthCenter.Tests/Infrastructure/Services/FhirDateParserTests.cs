using BirthCenter.Domain.Enums;
using BirthCenter.Domain.Specifications;
using BirthCenter.Infrastructure.Services;
using FluentAssertions;
using Xunit;

namespace BirthCenter.Tests.Infrastructure.Services
{
    public class FhirDateParserTests
    {
        [Fact]
        public void Parse_WithYearOnly_ShouldReturnYearRange()
        {
            // Arrange
            var input = "2024";

            // Act
            var result = FhirDateParser.Parse(input);

            // Assert
            result.IsPartial.Should().BeTrue();
            result.IsRange.Should().BeTrue();
            result.StartDate.Should().Be(new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc));
            result.EndDate.Should().Be(new DateTime(2024, 12, 31, 23, 59, 59, DateTimeKind.Utc));
            result.Prefix.Should().Be(DatePrefix.Eq);
        }

        [Fact]
        public void Parse_WithYearMonth_ShouldReturnMonthRange()
        {
            // Arrange
            var input = "2024-01";

            // Act
            var result = FhirDateParser.Parse(input);

            // Assert
            result.IsPartial.Should().BeTrue();
            result.IsRange.Should().BeTrue();
            result.StartDate.Should().Be(new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc));
            result.EndDate.Should().Be(new DateTime(2024, 1, 31, 23, 59, 59, DateTimeKind.Utc));
        }

        [Fact]
        public void Parse_WithPrefix_ShouldExtractPrefix()
        {
            // Arrange
            var input = "gt2024-01-01";

            // Act
            var result = FhirDateParser.Parse(input);

            // Assert
            result.Prefix.Should().Be(DatePrefix.Gt);
            result.ExactDate.Should().Be(new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc));
        }

        [Fact]
        public void Parse_WithInvalidFormat_ShouldThrowException()
        {
            // Arrange
            var input = "invalid";

            // Act
            Action act = () => FhirDateParser.Parse(input);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("Invalid date format: invalid");
        }
    }
}