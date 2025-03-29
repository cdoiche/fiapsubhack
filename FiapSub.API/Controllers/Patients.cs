using System.Security.Claims;
using FiapSub.API.DTOs;
using FiapSub.Core.Entities;
using FiapSub.Core.UseCases.Appointments;
using FiapSub.Core.UseCases.Patients;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FiapSub.API.Controllers;

[ApiController]
[Route("api/patient")]
public class PatientsController : ControllerBase
{
    private readonly RegisterPatientUseCase _registerPatientUseCase;
    private readonly UpdatePatientProfileUseCase _updatePatientProfileUseCase;
    private readonly ScheduleAppointmentUseCase _scheduleAppointmentUseCase;
    private readonly CancelConfirmedAppointmentUseCase _cancelConfirmedAppointmentUseCase;
    private readonly RescheduleAppointmentUseCase _rescheduleAppointmentUseCase;
    private readonly ListPatientUpcomingAppointmentsUseCase _listPatientUpcomingAppointmentsUseCase;
    private readonly ListPatientPastAppointmentsUseCase _listPatientPastAppointmentsUseCase;

    public PatientsController(
        RegisterPatientUseCase registerPatientUseCase,
        UpdatePatientProfileUseCase updatePatientProfileUseCase,
        ScheduleAppointmentUseCase scheduleAppointmentUseCase,
        CancelConfirmedAppointmentUseCase cancelConfirmedAppointmentUseCase,
        RescheduleAppointmentUseCase rescheduleAppointmentUseCase,
        ListPatientUpcomingAppointmentsUseCase listPatientUpcomingAppointmentsUseCase,
        ListPatientPastAppointmentsUseCase listPatientPastAppointmentsUseCase)
    {
        _registerPatientUseCase = registerPatientUseCase;
        _updatePatientProfileUseCase = updatePatientProfileUseCase;
        _scheduleAppointmentUseCase = scheduleAppointmentUseCase;
        _cancelConfirmedAppointmentUseCase = cancelConfirmedAppointmentUseCase;
        _rescheduleAppointmentUseCase = rescheduleAppointmentUseCase;
        _listPatientUpcomingAppointmentsUseCase = listPatientUpcomingAppointmentsUseCase;
        _listPatientPastAppointmentsUseCase = listPatientPastAppointmentsUseCase;
    }

    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> Post([FromBody] CreatePatientDTO dto)
    {
        try
        {
            var patient = new Patient(dto.Name, dto.Email, dto.Password, dto.Phone, dto.CPF);
            await _registerPatientUseCase.ExecuteAsync(patient);
            return Ok(new { message = "Patient registered successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("update-profile")]
    [Authorize]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdatePatientProfileDTO request)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userTypeClaim = User.FindFirst("userType")?.Value;
            
            if (userTypeClaim != "Patient" || string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized("Unauthorized access.");
            }

            int patientId = int.Parse(userIdClaim);

            await _updatePatientProfileUseCase.ExecuteAsync(patientId, request.Name, request.Phone);
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

    [HttpPost("schedule-appointment")]
    [Authorize]
    public async Task<IActionResult> ScheduleAppointment([FromBody] ScheduleAppointmentRequest request)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userTypeClaim = User.FindFirst("userType")?.Value;

            if (userTypeClaim != "Patient" || string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized("Unauthorized access.");
            }

            int patientId = int.Parse(userIdClaim);

            await _scheduleAppointmentUseCase.ExecuteAsync(patientId, request.AvailabilityId);
            return Ok(new { Message = "Appointment scheduled successfully." });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { Message = ex.Message });
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

            if (userTypeClaim != "Patient" || string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized("Unauthorized access.");
            }

            int patientId = int.Parse(userIdClaim);

            await _cancelConfirmedAppointmentUseCase.ExecuteAsync(patientId, request.AppointmentId, "Patient", request.CancellationReason);
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

    [HttpPost("reschedule-appointment")]
    [Authorize]
    public async Task<IActionResult> RescheduleAppointment([FromBody] RescheduleAppointmentRequest request)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userTypeClaim = User.FindFirst("userType")?.Value;

            if (userTypeClaim != "Patient" || string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized("Unauthorized access.");
            }

            await _rescheduleAppointmentUseCase.ExecuteAsync(request.AppointmentId, request.NewAvailabilityId, request.CancellationReason);
            return Ok(new { Message = "Appointment rescheduled successfully." });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { Message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { Message = ex.Message });
        }
    }

    [HttpGet("upcoming-appointments")]
    [Authorize]
    public async Task<IActionResult> GetUpcomingAppointments()
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userTypeClaim = User.FindFirst("userType")?.Value;
    
            if (userTypeClaim != "Patient" || string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized("Unauthorized access.");
            }
    
            int patientId = int.Parse(userIdClaim);
    
            var appointments = await _listPatientUpcomingAppointmentsUseCase.ExecuteAsync(patientId);
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
    
            if (userTypeClaim != "Patient" || string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized("Unauthorized access.");
            }
    
            int patientId = int.Parse(userIdClaim);
    
            var appointments = await _listPatientPastAppointmentsUseCase.ExecuteAsync(patientId);
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