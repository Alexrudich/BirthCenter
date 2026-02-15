using AutoMapper;
using BirthCenter.Application.DTO;
using BirthCenter.Application.Interfaces;
using BirthCenter.Application.Mapping;
using BirthCenter.Application.Services;
using BirthCenter.Domain.Entities;
using BirthCenter.Domain.Enums;
using BirthCenter.Domain.Exceptions;
using FluentAssertions;
using Moq;
using Xunit;

namespace BirthCenter.Tests.Application
{
    public class PatientServiceTests
    {
        private readonly Mock<IPatientRepository> _repositoryMock;
        private readonly IMapper _mapper;
        private readonly PatientService _service;

        public PatientServiceTests()
        {
            _repositoryMock = new Mock<IPatientRepository>();

            var config = new MapperConfiguration(cfg =>
                cfg.AddProfile<MappingProfile>());
            _mapper = config.CreateMapper();

            _service = new PatientService(_repositoryMock.Object, _mapper);
        }

        [Fact]
        public async Task GetByIdAsync_WithExistingId_ShouldReturnPatient()
        {
            // Arrange
            var id = Guid.NewGuid();
            var patient = new Patient("Иванов", DateTime.UtcNow);
            _repositoryMock.Setup(x => x.GetByIdAsync(id))
                .ReturnsAsync(patient);

            // Act
            var result = await _service.GetByIdAsync(id);

            // Assert
            result.Should().NotBeNull();
            result.Name.Family.Should().Be("Иванов");
        }

        [Fact]
        public async Task GetByIdAsync_WithNonExistingId_ShouldThrowNotFoundException()
        {
            // Arrange
            var id = Guid.NewGuid();
            _repositoryMock.Setup(x => x.GetByIdAsync(id))!
                .ReturnsAsync((Patient?)null);

            // Act
            Func<Task> act = async () => await _service.GetByIdAsync(id);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>()
                .WithMessage($"*{id}*");
        }

        [Fact]
        public async Task CreateAsync_WithValidRequest_ShouldCreatePatient()
        {
            // Arrange
            var request = new CreatePatientRequest
            {
                Name = new PatientNameDto
                {
                    Family = "Иванов",
                    Given = new List<string> { "Иван", "Иванович" },
                    Use = "official"
                },
                Gender = "male",
                BirthDate = new DateTime(2024, 1, 13, 18, 25, 43),
                Active = true
            };

            _repositoryMock.Setup(x => x.AddAsync(It.IsAny<Patient>()))
                .ReturnsAsync((Patient p) => p);

            // Act
            var result = await _service.CreateAsync(request);

            // Assert
            result.Should().NotBeNull();
            result.Name.Family.Should().Be("Иванов");
            result.Gender.Should().Be("male");
            _repositoryMock.Verify(x => x.AddAsync(It.IsAny<Patient>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_WithoutFamily_ShouldThrowValidationException()
        {
            // Arrange
            var request = new CreatePatientRequest
            {
                Name = null!,
                BirthDate = DateTime.UtcNow
            };

            // Act
            Func<Task> act = async () => await _service.CreateAsync(request);

            // Assert
            await act.Should().ThrowAsync<ValidationException>()
                .WithMessage("Family name is required");
        }
    }
}