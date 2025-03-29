using Microsoft.Extensions.DependencyInjection;
using FiapSub.Core.UseCases.Auth;
using FiapSub.Core.UseCases.Doctors;
using FiapSub.Core.UseCases.Patients;
using FiapSub.Core.UseCases.Appointments;
using FiapSub.Core.UseCases.Availability;

namespace FiapSub.Core.Extensions;

public static class ServiceCollectionExtenions
{
    public static IServiceCollection AddUseCases(this IServiceCollection services)
    {
        // Auth use cases
        services.AddScoped<AuthenticateUserUseCase>();

        // Appointment use cases
        services.AddScoped<ScheduleAppointmentUseCase>();
        services.AddScoped<RescheduleAppointmentUseCase>();
        services.AddScoped<ConfirmPendingAppointmentUseCase>();
        services.AddScoped<RejectPendingAppointmentUseCase>();
        services.AddScoped<CancelConfirmedAppointmentUseCase>();
        services.AddScoped<ListDoctorConfirmedAppointmentsUseCase>();
        services.AddScoped<ListDoctorPendingAppointmentsUseCase>();
        services.AddScoped<ListDoctorPastAppointmentsUseCase>();
        services.AddScoped<ListDoctorCancelledAppointmentsUseCase>();
        services.AddScoped<ListPatientUpcomingAppointmentsUseCase>();
        services.AddScoped<ListPatientPastAppointmentsUseCase>();

        // Availability use cases
        services.AddScoped<ListDoctorAvailabilityUseCase>();
        services.AddScoped<AddDoctorAvailabilityUseCase>();

        // Patients use cases
        services.AddScoped<RegisterPatientUseCase>();
        services.AddScoped<UpdatePatientProfileUseCase>();

        // Doctors use cases
        services.AddScoped<RegisterDoctorUseCase>();
        services.AddScoped<UpdateDoctorProfileUseCase>();
        services.AddScoped<SearchDoctorsBySpecialtyUseCase>();

        return services;
    }
}