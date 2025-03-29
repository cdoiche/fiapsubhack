using FiapSub.Core.Entities;
using FiapSub.Core.Enums;
using FiapSub.Core.Interfaces;
using FiapSub.Infra.Data;
using Microsoft.EntityFrameworkCore;

namespace FiapSub.Infra.Repositories;

public class DoctorRepository : IDoctorRepository
{
    private readonly ApplicationDbContext _context;

    public DoctorRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Doctor> GetByIdAsync(int id)
    {
        var doctor =  await _context.Doctors.FindAsync(id) 
            ?? throw new KeyNotFoundException($"Doctor with id {id} not found");

        return doctor;
    }

    public async Task<IEnumerable<Doctor>> GetAllAsync()
    {
        return await _context.Doctors.ToListAsync();        
    }

    public async Task AddAsync(Doctor user)
    {
        await _context.Doctors.AddAsync(user);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Doctor user)
    {
        _context.Doctors.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var doctor = await _context.Doctors.FindAsync(id);
        if (doctor != null)
        {
            _context.Doctors.Remove(doctor);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<Doctor> GetByEmailAsync(string email)
    {
        var doctor = await _context.Doctors.FirstOrDefaultAsync(p=> p.Email == email);

        return doctor;
    }

    public async Task<bool> ExistsWithEmailAsync(string email)
    {
        return await _context.Doctors.AnyAsync(p => p.Email == email);
    }

    public async Task<IEnumerable<Doctor>> SearchBySpecialtyAsync(Specialty specialty)
    {
        var doctors = _context.Doctors.Where(d => d.Specialty == specialty)
            ?? throw new KeyNotFoundException($"No doctors with specialty {specialty} found");
        
        return await doctors.ToListAsync();
    }
}