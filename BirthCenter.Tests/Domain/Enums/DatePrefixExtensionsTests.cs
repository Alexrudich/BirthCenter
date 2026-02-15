using BirthCenter.Domain.Enums;
using FluentAssertions;
using Xunit;

namespace BirthCenter.Tests.Domain.Enums
{
    public class DatePrefixExtensionsTests
    {
        [Theory]
        [InlineData(DatePrefix.Eq, "eq")]
        [InlineData(DatePrefix.Ne, "ne")]
        [InlineData(DatePrefix.Gt, "gt")]
        [InlineData(DatePrefix.Lt, "lt")]
        [InlineData(DatePrefix.Ge, "ge")]
        [InlineData(DatePrefix.Le, "le")]
        [InlineData(DatePrefix.Sa, "sa")]
        [InlineData(DatePrefix.Eb, "eb")]
        [InlineData(DatePrefix.Ap, "ap")]
        public void ToPrefixString_ShouldReturnCorrectString(DatePrefix prefix, string expected)
        {
            // Act
            var result = prefix.ToPrefixString();

            // Assert
            result.Should().Be(expected);
        }

        [Theory]
        [InlineData("eq", DatePrefix.Eq)]
        [InlineData("ne", DatePrefix.Ne)]
        [InlineData("gt", DatePrefix.Gt)]
        [InlineData("lt", DatePrefix.Lt)]
        [InlineData("ge", DatePrefix.Ge)]
        [InlineData("le", DatePrefix.Le)]
        [InlineData("sa", DatePrefix.Sa)]
        [InlineData("eb", DatePrefix.Eb)]
        [InlineData("ap", DatePrefix.Ap)]
        [InlineData("EQ", DatePrefix.Eq)]
        [InlineData("Gt", DatePrefix.Gt)]
        public void ParsePrefix_WithValidString_ShouldReturnCorrectEnum(string input, DatePrefix expected)
        {
            // Act
            var result = input.ParsePrefix();

            // Assert
            result.Should().Be(expected);
        }

        [Fact]
        public void ParsePrefix_WithInvalidString_ShouldThrowException()
        {
            // Arrange
            var invalid = "invalid";

            // Act
            Action act = () => invalid.ParsePrefix();

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("Invalid prefix: invalid");
        }

        [Fact]
        public void AllPrefixStrings_ShouldReturnAllPrefixes()
        {
            // Act
            var result = DatePrefixExtensions.AllPrefixStrings;

            // Assert
            result.Should().HaveCount(9);
            result.Should().Contain(new[] { "eq", "ne", "gt", "lt", "ge", "le", "sa", "eb", "ap" });
        }
    }
}