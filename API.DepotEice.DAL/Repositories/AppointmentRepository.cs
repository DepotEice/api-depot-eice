using API.DepotEice.DAL.Entities;
using API.DepotEice.DAL.IRepositories;
using API.DepotEice.DAL.Mappers;
using API.DepotEice.Helpers.Exceptions;
using DevHopTools.DataAccess;
using DevHopTools.DataAccess.Interfaces;

namespace API.DepotEice.DAL.Repositories;

public class AppointmentRepository : RepositoryBase, IAppointmentRepository
{
    public AppointmentRepository(IDevHopConnection connection) : base(connection) { }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="appointmentId"></param>
    /// <param name="isAccepted"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public bool AppointmentDecision(int appointmentId, bool isAccepted = true)
    {
        if (appointmentId <= 0)
            throw new ArgumentOutOfRangeException(nameof(appointmentId));

        string query = "UPDATE [dbo].[Appointments] SET [IsAccepted] = @isAccepted WHERE [Id] = @id";

        Command command = new Command(query);
        command.AddParameter("id", appointmentId);
        command.AddParameter("isAccepted", isAccepted);

        return _connection.ExecuteNonQuery(command) > 0;
    }

    /// <inheritdoc/>
    public IEnumerable<AppointmentEntity> GetAll()
    {
        Command command = new Command("SELECT * FROM [dbo].[Appointments]");

        return _connection.ExecuteReader(command,
            appointment => Mapper.DbToAppointmentEntity(appointment));
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="DatabaseScalarNullException"></exception>
    public int Create(AppointmentEntity entity)
    {
        if (entity is null)
            throw new ArgumentNullException(nameof(entity));

        Command command = new Command("spAppointmentCreate", true);

        command.AddParameter("startAt", entity.StartAt);
        command.AddParameter("endAt", entity.EndAt);
        command.AddParameter("userId", entity.UserId);

        string? scalarResult = _connection.ExecuteScalar(command).ToString();

        if (string.IsNullOrEmpty(scalarResult))
        {
            throw new DatabaseScalarNullException(nameof(scalarResult));
        }
            
        return int.Parse(scalarResult);
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public AppointmentEntity? GetByKey(int key)
    {
        if (key <= 0)
            throw new ArgumentOutOfRangeException(nameof(key));

        string query = "SELECT * FROM [dbo].[Appointments] WHERE [Id] = @id";

        Command command = new Command(query);

        command.AddParameter("id", key);

        return _connection
            .ExecuteReader(command, appointment => appointment.DbToAppointmentEntity())
            .SingleOrDefault();
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public bool Update(int key, AppointmentEntity entity)
    {
        if (key <= 0)
            throw new ArgumentOutOfRangeException(nameof(key));

        if (entity is null)
            throw new ArgumentNullException(nameof(entity));

        string query = "UPDATE [dbo].[Appointments] SET [StartAt] = @startAt, [EndAt] = @endAt WHERE [Id] = @id";

        Command command = new Command(query);
        command.AddParameter("id", key);
        command.AddParameter("startAt", entity.StartAt);
        command.AddParameter("endAt", entity.EndAt);
        return _connection.ExecuteNonQuery(command) > 0;
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public bool Delete(int key)
    {
        if (key <= 0)
            throw new ArgumentOutOfRangeException(nameof(key));

        string query = "DELETE FROM [dbo].[Appointments] WHERE [Id] = @id";

        Command command = new Command(query);

        command.AddParameter("id", key);

        return _connection.ExecuteNonQuery(command) > 0;
    }
}
