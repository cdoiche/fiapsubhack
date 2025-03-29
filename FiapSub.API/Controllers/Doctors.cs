using System.Security.Claims;
using FiapSub.API.DTOs;
using FiapSub.Core.Entities;
using FiapSub.Core.Enums;
using FiapSub.Core.UseCases.Appointments;
using FiapSub.Core.UseCases.Availability;
using FiapSub.Core.UseCases.Doctors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FiapSub.API.Controllers;

[ApiController]
[Route("api/doctor")]
public class DoctorsController : ControllerBase
{
    private readonly RegisterDoctorUseCase _registerDoctorUseCase;
    private readonly UpdateDoctorProfileUseCase _updateDoctorProfileUseCase;
    private readonly SearchDoctorsBySpecialtyUseCase _searchDoctorBySpecialtyUseCase;
    private readonly ListDoctorAvailabilityUseCase _listDoctorAvailabilityUseCase;
    private readonly AddDoctorAvailabilityUseCase _addDoctorAvailabilityUseCase;
    private readonly ConfirmPendingAppointmentUseCase _confirmPendingAppointmentUseCase;
    private readonly CancelConfirmedAppointmentUseCase _cancelConfirmedAppointmentUseCase;
    private readonly ListDoctorConfirmedAppointmentsUseCase _listDoctorConfirmedAppointmentsUseCase;
    private readonly ListDoctorPendingAppointmentsUseCase _listDoctorPendingAppointmentsUseCase;
    private readonly ListDoctorPastAppointmentsUseCase _listDoctorPastAppointmentsUseCase;
    private readonly ListDoctorCancelledAppointmentsUseCase _listDoctorCancelledAppointmentsUseCase;
    private readonly RejectPendingAppointmentUseCase _rejectPendingAppointmentUseCase;

    public DoctorsController(
        RegisterDoctorUseCase registerDoctorUseCase,
        UpdateDoctorProfileUseCase updateDoctorProfileUseCase,
        SearchDoctorsBySpecialtyUseCase searchDoctorBySpecialtyUseCase,
        ListDoctorAvailabilityUseCase listDoctorAvailabilityUseCase,
        AddDoctorAvailabilityUseCase addDoctorAvailabilityUseCase,
        ConfirmPendingAppointmentUseCase confirmPendingAppointmentUseCase,
        RejectPendingAppointmentUseCase rejectPendingAppointmentUseCase,
        CancelConfirmedAppointmentUseCase cancelConfirmedAppointmentUseCase,
        ListDoctorConfirmedAppointmentsUseCase listDoctorConfirmedAppointmentsUseCase,
        ListDoctorPendingAppointmentsUseCase listDoctorPendingAppointmentsUseCase,
        ListDoctorPastAppointmentsUseCase listDoctorPastAppointmentsUseCase,
        ListDoctorCancelledAppointmentsUseCase listDoctorCancelledAppointmentsUseCase)
    {
        _registerDoctorUseCase = registerDoctorUseCase;
        _updateDoctorProfileUseCase = updateDoctorProfileUseCase;
        _searchDoctorBySpecialtyUseCase = searchDoctorBySpecialtyUseCase;
        _listDoctorAvailabilityUseCase = listDoctorAvailabilityUseCase;
        _addDoctorAvailabilityUseCase = addDoctorAvailabilityUseCase;
        _confirmPendingAppointmentUseCase = confirmPendingAppointmentUseCase;
        _rejectPendingAppointmentUseCase = rejectPendingAppointmentUseCase;
        _cancelConfirmedAppointmentUseCase = cancelConfirmedAppointmentUseCase;
        _listDoctorConfirmedAppointmentsUseCase = listDoctorConfirmedAppointmentsUseCase;
        _listDoctorPendingAppointmentsUseCase = listDoctorPendingAppointmentsUseCase;
        _listDoctorPastAppointmentsUseCase = listDoctorPastAppointmentsUseCase;
        _listDoctorCancelledAppointmentsUseCase = listDoctorCancelledAppointmentsUseCase;
    }

    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> Post([FromBody] CreateDoctorDTO dto)
    {
        try
        {
            var Doctor = new Doctor(dto.Name, dto.Email, dto.Password, dto.Phone, dto.CRM, dto.Specialty);
            await _registerDoctorUseCase.ExecuteAsync(Doctor);
            return Ok(new { message = "Doctor registered successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("update-profile")]
    [Authorize]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateDoctorProfileDTO request)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userTypeClaim = User.FindFirst("userType")?.Value;
            if (userTypeClaim != "Doctor")
            {
                return Unauthorized("Unauthorized access.");
            }
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized("Unauthorized access.");
            }
            int doctorId = int.Parse(userIdClaim);

            await _updateDoctorProfileUseCase.ExecuteAsync(doctorId, request.Name, request.Phone, request.Specialty);
            return Ok(new { Message = "Profile updated successfully." });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { Message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    [HttpGet("search-by-specialty")]
    public async Task<IActionResult> SearchBySpecialty([FromQuery] Specialty specialty)
    {
        try
        {
            var doctors = await _searchDoctorBySpecialtyUseCase.ExecuteAsync(specialty);
            return Ok(doctors);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{doctorId}/availability")]
    public async Task<IActionResult> GetAvailability(int doctorId)
    {
        try
        {
            var availabilities = await _listDoctorAvailabilityUseCase.ExecuteAsync(doctorId);
            return Ok(availabilities);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { Message = ex.Message });
        }
    }

    [HttpPost("availability")]
    [Authorize]
    public async Task<IActionResult> AddAvailability([FromBody] AddAvailabilityRequest request)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userTypeClaim = User.FindFirst("userType")?.Value;

            if (userTypeClaim != "Doctor" || string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized("Unauthorized access.");
            }

            int doctorId = int.Parse(userIdClaim);

            await _addDoctorAvailabilityUseCase.ExecuteAsync(doctorId, request.Start, request.End);

            return Ok(new { Message = "Availability added successfully." });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    [HttpPost("confirm-appointment")]
    [Authorize]
    public async Task<IActionResult> ConfirmAppointment(int appointmentId)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userTypeClaim = User.FindFirst("userType")?.Value;

            if (userTypeClaim != "Doctor" || string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized("Unauthorized access.");
            }

            int doctorId = int.Parse(userIdClaim);

            await _confirmPendingAppointmentUseCase.ExecuteAsync(doctorId, appointmentId);

            return Ok(new { Message = "Appointment confirmed successfully." });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { Message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    [HttpPost("reject-appointment")]
    [Authorize]
    public async Task<IActionResult> RejectAppointment([FromBody] RejectAppointmentRequest request)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userTypeClaim = User.FindFirst("userType")?.Value;
    
            if (userTypeClaim != "Doctor" || string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized("Unauthorized access.");
            }
    
            int doctorId = int.Parse(userIdClaim);
    
            await _rejectPendingAppointmentUseCase.ExecuteAsync(doctorId, request.AppointmentId, request.RejectionReason);
            return Ok(new { Message = "Appointment rejected successfully." });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { Message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { Message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { Message = ex.Message });
        }
    }

    [HttpPost("cancel-appointment")]
    [Authorize]
    public async Task<IActionResult> CancelAppointment([FromBody] CancelAppointmentRequest request)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userTypeClaim = User.FindFirst("userType")?.Value;
    
            if (userTypeClaim != "Doctor" || string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized("Unauthorized access.");
            }
    
            int doctorId = int.Parse(userIdClaim);
    
            await _cancelConfirmedAppointmentUseCase.ExecuteAsync(doctorId, request.AppointmentId, "Doctor", request.CancellationReason);
            return Ok(new { Message = "Appointment cancelled successfully." });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { Message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { Message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

        [HttpGet("confirmed-appointments")]
        [Authorize]
        public async Task<IActionResult> GetConfirmedAppointments()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var userTypeClaim = User.FindFirst("userType")?.Value;
        
                if (userTypeClaim != "Doctor" || string.IsNullOrEmpty(userIdClaim))
                {
                    return Unauthorized("Unauthorized access.");
                }
        
                int doctorId = int.Parse(userIdClaim);

                var appointments = await _listDoctorConfirmedAppointmentsUseCase.ExecuteAsync(doctorId);
                return Ok(appointments);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }

        [HttpGet("pending-appointments")]
        [Authorize]
        public async Task<IActionResult> GetPendingAppointments()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var userTypeClaim = User.FindFirst("userType")?.Value;

                if (userTypeClaim != "Doctor" || string.IsNullOrEmpty(userIdClaim))
                {
                    return Unauthorized("Unauthorized access.");
                }
        
                int doctorId = int.Parse(userIdClaim);
                var appointments = await _listDoctorPendingAppointmentsUseCase.ExecuteAsync(doctorId);
                return Ok(appointments);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }

        [HttpGet("past-appointments")]
        [Authorize]
        public async Task<IActionResult> GetPastAppointments()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var userTypeClaim = User.FindFirst("userType")?.Value;

                if (userTypeClaim != "Doctor" || string.IsNullOrEmpty(userIdClaim))
                {
                    return Unauthorized("Unauthorized access.");
                }
        
                int doctorId = int.Parse(userIdClaim);
                var appointments = await _listDoctorPastAppointmentsUseCase.ExecuteAsync(doctorId);
                return Ok(appointments);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }

        [HttpGet("cancelled-appointments")]
        [Authorize]
        public async Task<IActionResult> GetCancelledAppointments()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var userTypeClaim = User.FindFirst("userType")?.Value;
        
                if (userTypeClaim != "Doctor" || string.IsNullOrEmpty(userIdClaim))
                {
                    return Unauthorized("Unauthorized access.");
                }
        
                int doctorId = int.Parse(userIdClaim);
        
                var appointments = await _listDoctorCancelledAppointmentsUseCase.ExecuteAsync(doctorId);
                return Ok(appointments);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }
}