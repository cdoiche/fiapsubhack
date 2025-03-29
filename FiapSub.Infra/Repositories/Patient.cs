using FiapSub.Core.Entities;
using FiapSub.Core.Interfaces;
using FiapSub.Infra.Data;
using Microsoft.EntityFrameworkCore;

namespace FiapSub.Infra.Repositories;

public class PatientRepository : IPatientRepository
{
    private readonly ApplicationDbContext _context;

    public PatientRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Patient> GetByIdAsync(int id)
    {
        var patient =  await _context.Patients.FindAsync(id);

        if (patient == null)
        {
            throw new KeyNotFoundException($"Patient with id {id} not found");
        }

        return patient;
    }

    public async Task<IEnumerable<Patient>> GetAllAsync()
    {
        return await _context.Patients.ToListAsync();        
    }

    public async Task AddAsync(Patient user)
    {
        await _context.Patients.AddAsync(user);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Patient user)
    {
        _context.Patients.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var patient = await _context.Patients.FindAsync(id);
        if (patient != null)
        {
            _context.Patients.Remove(patient);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<Patient> GetByEmailAsync(string email)
    {
        var patient = await _context.Patients.FirstOrDefaultAsync(p=> p.Email == email);

        return patient;
    }

    public async Task<bool> ExistsWithEmailAsync(string email)
    {
        return await _context.Patients.AnyAsync(p => p.Email == email);
    }
}