using FiapSub.Infra.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using FiapSub.Core.Interfaces;
using FiapSub.Infra.Repositories;
using FiapSub.Infra.Services;

namespace FiapSub.Infra.Extensions;

public static class ServiceCollectionExtenions
{
    public static IServiceCollection AddInfra(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlite(connectionString));

        using (var serviceProvider = services.BuildServiceProvider())
        {
            var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();
            dbContext.Database.Migrate();
        }

        services.AddScoped<IPatientRepository, PatientRepository>();
        services.AddScoped<IAppointmentRepository, AppointmentRepository>();
        services.AddScoped<IDoctorAvailabilityRepository, DoctorAvailabilityRepository>();
        services.AddScoped<IDoctorRepository, DoctorRepository>();

        services.AddScoped<INotificationService, MockNotificationService>();

        return services;
    }
}