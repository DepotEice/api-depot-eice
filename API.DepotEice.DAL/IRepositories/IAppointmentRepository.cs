using API.DepotEice.DAL.Entities;

namespace API.DepotEice.DAL.IRepositories;

public interface IAppointmentRepository : IRepositoryBase<AppointmentEntity, int>
{
    bool AcceptAppointment(int appointmentId);
}
