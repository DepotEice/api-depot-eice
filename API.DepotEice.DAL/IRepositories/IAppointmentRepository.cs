using API.DepotEice.DAL.Entities;

namespace API.DepotEice.DAL.IRepositories;

public interface IAppointmentRepository : IRepositoryBase<int, AppointmentEntity>
{
    bool AppointmentDecision(int appointmentId, bool isAccepted = true);
}
