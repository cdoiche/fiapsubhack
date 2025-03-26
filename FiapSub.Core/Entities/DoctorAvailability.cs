namespace FiapSub.Core.Entities;

public class DoctorAvailability
{
    public int Id { get; private set; }
    public int DoctorId { get; private set; }
    public DateTime StartTime { get; private set; }
    public DateTime EndTime { get; private set; }
    public bool IsAvailable { get; private set; }

    public DoctorAvailability(int doctorId, DateTime startTime, DateTime endTime)
    {
        if (startTime >= endTime)
            throw new ArgumentException("Start time must be before end time.");
        
        DoctorId = doctorId;
        StartTime = startTime;
        EndTime = endTime;
        IsAvailable = true;
    }

    public void Block()
    {
        IsAvailable = false;
    }
}