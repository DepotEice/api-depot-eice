using API.DepotEice.DAL.Entities;
using API.DepotEice.DAL.IRepositories;
using API.DepotEice.DAL.Mappers;
using API.DepotEice.Helpers.Exceptions;
using DevHopTools.DataAccess;
using DevHopTools.DataAccess.Interfaces;

namespace API.DepotEice.DAL.Repositories;

public class ScheduleRepository : RepositoryBase, IScheduleRepository
{
    public ScheduleRepository(IDevHopConnection connection) : base(connection) { }

    /// <summary>
    /// Retrieve every <see cref="ScheduleEntity"/> records from the database linked to a
    /// module <see cref="ModuleEntity"/> by its ID
    /// </summary>
    /// <param name="moduleId">
    /// The linked <see cref="ModuleEntity"/>'s ID
    /// </param>
    /// <returns>
    /// An <see cref="IEnumerable{T}"/> of <see cref="ScheduleEntity"/>. If no data is found,
    /// returns an empty <see cref="IEnumerable{T}"/>
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public IEnumerable<ScheduleEntity> GetModuleSchedules(int moduleId)
    {
        if (moduleId <= 0)
            throw new ArgumentOutOfRangeException(nameof(moduleId));

        string query = "SELECT * FROM [dbo].[Schedules] WHERE [ModuleId] = @moduleId";

        Command command = new Command(query);
        command.AddParameter("moduleId", moduleId);

        return _connection
            .ExecuteReader(command, schedule => schedule.DbToSchedule());
    }

    #region Basic CRUD

    /// <inheritdoc/>
    /// <exception cref="NotImplementedException"></exception>
    public IEnumerable<ScheduleEntity> GetAll()
    {
        string query = "SELECT * FROM [dbo].[Schedules]";

        Command command = new Command(query);

        return _connection.ExecuteReader(command, s => s.DbToSchedule());
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="DatabaseScalarNullException"></exception>
    public int Create(ScheduleEntity entity)
    {
        if (entity is null)
            throw new ArgumentNullException(nameof(entity));

        Command command = new Command("spSchedules_Create", true);
        command.AddParameter("title", entity.Title);
        command.AddParameter("details", entity.Details);
        command.AddParameter("startsAt", entity.StartAt);
        command.AddParameter("endsAt", entity.EndAt);
        command.AddParameter("moduleId", entity.ModuleId);

        string scalarResult = _connection.ExecuteScalar(command).ToString();

        if (string.IsNullOrEmpty(scalarResult))
            throw new DatabaseScalarNullException(nameof(scalarResult));

        return int.Parse(scalarResult);
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public ScheduleEntity GetByKey(int key)
    {
        if (key <= 0)
            throw new ArgumentOutOfRangeException(nameof(key));

        string query = "SELECT * FROM [dbo].[Schedules] WHERE [Id] = @id";

        Command command = new Command(query);
        command.AddParameter("id", key);

        return _connection
            .ExecuteReader(command, schedule => schedule.DbToSchedule())
            .SingleOrDefault();
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public bool Update(int key, ScheduleEntity entity)
    {
        if (key <= 0)
            throw new ArgumentOutOfRangeException(nameof(key));

        if (entity is null)
            throw new ArgumentNullException(nameof(entity));

        string query = "UPDATE [dbo].[Schedules] SET [Title] = @title, [Details] = @details, [StartsAt] = @startsAt, [EndsAt] = @endsAt WHERE [Id] = @id";

        Command command = new Command(query);
        command.AddParameter("id", key);
        command.AddParameter("title", entity.Title);
        command.AddParameter("details", entity.Details);
        command.AddParameter("startsAt", entity.StartAt);
        command.AddParameter("endsAt", entity.EndAt);

        return _connection.ExecuteNonQuery(command) > 0;
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public bool Delete(int key)
    {
        if (key <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(key));
        }

        string query = "DELETE FROM [dbo].[Schedules] WHERE [Id] = @id";

        Command command = new Command(query);
        command.AddParameter("id", key);

        return _connection.ExecuteNonQuery(command) > 0;
    }

    #endregion
}
