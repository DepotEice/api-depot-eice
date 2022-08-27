using API.DepotEice.DAL.Entities;
using API.DepotEice.DAL.IRepositories;
using API.DepotEice.DAL.Mappers;
using API.DepotEice.Helpers.Exceptions;
using DevHopTools.DataAccess;
using DevHopTools.DataAccess.Interfaces;

namespace API.DepotEice.DAL.Repositories;

public class ScheduleFileRepository : IScheduleFileRepository
{
    private readonly IDevHopConnection _connection;

    public ScheduleFileRepository(IDevHopConnection connection)
    {
        if (connection is null)
        {
            throw new ArgumentNullException(nameof(connection));
        }

        _connection = connection;
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="DatabaseScalarNullException"></exception>
    public int Create(ScheduleFileEntity entity)
    {
        if (entity is null)
        {
            throw new ArgumentNullException(nameof(entity));
        }

        Command command = new Command("spCreateScheduleFile", true);

        command.AddParameter("filePath", entity.FilePath);
        command.AddParameter("scheduleId", entity.ScheduleId);

        string? scalarResult = _connection.ExecuteScalar(command).ToString();

        if (string.IsNullOrEmpty(scalarResult))
        {
            throw new DatabaseScalarNullException(nameof(scalarResult));
        }

        return int.Parse(scalarResult);
    }


    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException"></exception>
    public bool Delete(ScheduleFileEntity entity)
    {
        if (entity is null)
        {
            throw new ArgumentNullException(nameof(entity));
        }

        Command command = new Command("spDeleteScheduleFile", true);

        command.AddParameter("id", entity.Id);

        return _connection.ExecuteNonQuery(command) > 0;
    }

    public IEnumerable<ScheduleFileEntity> GetAll()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public ScheduleFileEntity? GetByKey(int key)
    {
        if (key <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(key));
        }

        Command command = new Command("spGetScheduleFile", true);

        command.AddParameter("id", key);

        return _connection
            .ExecuteReader(command, scheduleFile => Mapper.DbToScheduleFile(scheduleFile))
            .SingleOrDefault();
    }

    /// <summary>
    /// Retrieve all <see cref="ScheduleFileEntity"/> linked to a <see cref="ScheduleEntity"/>
    /// by its ID
    /// </summary>
    /// <param name="scheduleId">
    /// The <see cref="ScheduleEntity"/>'s ID
    /// </param>
    /// <returns>
    /// An <see cref="IEnumerable{T}"/> of <see cref="ScheduleFileEntity"/>. If no data is 
    /// found, return an empty <see cref="IEnumerable{T}"/>
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public IEnumerable<ScheduleFileEntity> GetScheduleFiles(int scheduleId)
    {
        if (scheduleId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(scheduleId));
        }

        Command command = new Command("spGetScheduleFiles", true);

        command.AddParameter("scheduleId", scheduleId);

        return _connection
            .ExecuteReader(command, scheduleFile => Mapper.DbToScheduleFile(scheduleFile));
    }

    public bool Update(ScheduleFileEntity entity)
    {
        throw new NotImplementedException();
    }
}
