using BirthCenter.API.Controllers;
using BirthCenter.Application.DTO;
using BirthCenter.Application.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace BirthCenter.Tests.API
{
    public class PatientsControllerTests
    {
        private readonly Mock<IPatientService> _serviceMock;
        private readonly PatientsController _controller;

        public PatientsControllerTests()
        {
            _serviceMock = new Mock<IPatientService>();
            _controller = new PatientsController(_serviceMock.Object);
        }

        [Fact]
        public async Task GetById_WithValidId_ShouldReturnOk()
        {
            // Arrange
            var id = Guid.NewGuid();
            var patientDto = new PatientDto { Id = id };
            _serviceMock.Setup(x => x.GetByIdAsync(id))
                .ReturnsAsync(patientDto);

            // Act
            var result = await _controller.GetById(id);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().Be(patientDto);
        }

        [Fact]
        public async Task Create_WithValidRequest_ShouldReturnCreatedAtAction()
        {
            // Arrange
            var request = new CreatePatientRequest();
            var createdDto = new PatientDto { Id = Guid.NewGuid() };
            _serviceMock.Setup(x => x.CreateAsync(request))
                .ReturnsAsync(createdDto);

            // Act
            var result = await _controller.Create(request);

            // Assert
            var createdResult = result.Result as CreatedAtActionResult;
            createdResult.Should().NotBeNull();

            var routeValues = createdResult!.RouteValues;
            routeValues.Should().NotBeNull();
            routeValues!["id"].Should().Be(createdDto.Id);
            createdResult.StatusCode.Should().Be(201);
            createdResult.ActionName.Should().Be(nameof(_controller.GetById));
        }
    }
}