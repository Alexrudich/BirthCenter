using Microsoft.AspNetCore.Mvc;
using BirthCenter.Application.DTO;
using BirthCenter.Application.Interfaces;

namespace BirthCenter.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class PatientsController : ControllerBase
    {
        private readonly IPatientService _patientService;

        public PatientsController(IPatientService patientService)
        {
            _patientService = patientService;
        }

        /// <summary>
        /// Get patient by ID
        /// </summary>
        /// <param name="id">Patient unique identifier</param>
        /// <returns>Patient information</returns>
        /// <response code="200">Returns the patient</response>
        /// <response code="404"> Not found</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(PatientDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PatientDto>> GetById(Guid id)
        {
            var patient = await _patientService.GetByIdAsync(id);
            return Ok(patient);
        }

        /// <summary>
        /// Get all patients
        /// </summary>
        /// <returns>List of patients</returns>
        /// <response code="200">Returns list of patients</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<PatientDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<PatientDto>>> GetAll()
        {
            var patients = await _patientService.GetAllAsync();
            return Ok(patients);
        }

        /// <summary>
        /// Search patients by birthdate (FHIR format)
        /// </summary>
        /// <param name="birthDate">Date parameter with FHIR prefixes (eq, gt, lt, ge, le) and partial dates (2024, 2024-01)</param>
        /// <returns>List of patients matching the date criteria</returns>
        /// <response code="200">Returns matching patients</response>
        /// <response code="400">Invalid date format</response>
        /// <example>?birthDate=gt2024-01-01</example>
        /// <example>?birthDate=2024</example>
        [HttpGet("search")]
        [ProducesResponseType(typeof(IEnumerable<PatientDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<PatientDto>>> SearchByBirthDate([FromQuery] string birthDate)
        {
            if (string.IsNullOrWhiteSpace(birthDate))
                return BadRequest("birthDate parameter is required");

            var patients = await _patientService.SearchByBirthDateAsync(birthDate);
            return Ok(patients);
        }

        /// <summary>
        /// Create new patient
        /// </summary>
        /// <param name="request">Patient data</param>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /api/Patients
        ///     {
        ///         "name": {
        ///             "use": "official",
        ///             "family": "Иванов",
        ///             "given": ["Иван", "Иванович"]
        ///         },
        ///         "gender": "male",
        ///         "birthDate": "2024-01-13T18:25:43",
        ///         "active": true
        ///     }
        /// </remarks>
        /// <returns>Created patient</returns>
        /// <response code="201">Patient created successfully</response>
        /// <response code="400">Invalid request data</response>
        [HttpPost]
        [ProducesResponseType(typeof(PatientDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PatientDto>> Create([FromBody] CreatePatientRequest request)
        {
            var patient = await _patientService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = patient.Id }, patient);
        }

        /// <summary>
        /// Update existing patient
        /// </summary>
        /// <param name="id">Patient unique identifier</param>
        /// <param name="request">Updated patient data (all fields are optional)</param>
        /// <remarks>
        /// Sample request - all fields are optional. You can send only the fields you want to update:
        /// 
        ///     PUT /api/Patients/3fa85f64-5717-4562-b3fc-2c963f66afa6
        ///     {
        ///         "gender": "female"
        ///     }
        ///     
        /// Or update multiple fields:
        /// 
        ///     PUT /api/Patients/3fa85f64-5717-4562-b3fc-2c963f66afa6
        ///     {
        ///         "name": {
        ///             "family": "Петров",
        ///             "given": ["Петр", "Петрович"]
        ///         },
        ///         "active": false
        ///     }
        /// </remarks>
        /// <returns>Updated patient</returns>
        /// <response code="200">Patient updated successfully</response>
        /// <response code="400">Invalid request data</response>
        /// <response code="404">Patient not found</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(PatientDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PatientDto>> Update(Guid id, [FromBody] UpdatePatientRequest request)
        {
            var patient = await _patientService.UpdateAsync(id, request);
            return Ok(patient);
        }

        /// <summary>
        /// Delete patient
        /// </summary>
        /// <param name="id">Patient unique identifier</param>
        /// <returns>No content</returns>
        /// <response code="204">Patient deleted successfully</response>
        /// <response code="404">Patient not found</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _patientService.DeleteAsync(id);
            return NoContent();
        }
    }
}