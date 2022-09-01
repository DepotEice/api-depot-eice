using API.DepotEice.DAL.Entities;
using API.DepotEice.DAL.IRepositories;
using API.DepotEice.DAL.Mappers;
using API.DepotEice.Helpers.Exceptions;
using DevHopTools.DataAccess;
using DevHopTools.DataAccess.Interfaces;

namespace API.DepotEice.DAL.Repositories;

public class ScheduleFileRepository : RepositoryBase, IScheduleFileRepository
{
    public ScheduleFileRepository(IDevHopConnection connection) : base(connection) { }

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
            throw new ArgumentOutOfRangeException(nameof(scheduleId));

        string query = "SELECT * FROM [dbo].[ScheduleFiles] WHERE [ScheduleId] = @scheduleId";

        Command command = new Command(query);
        command.AddParameter("scheduleId", scheduleId);

        return _connection
            .ExecuteReader(command, scheduleFile => scheduleFile.DbToScheduleFile());
    }

    #region BASIC CRUD

    /// <inheritdoc/>
    /// <exception cref="NotImplementedException"></exception>
    public IEnumerable<ScheduleFileEntity> GetAll()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="DatabaseScalarNullException"></exception>
    public int Create(ScheduleFileEntity entity)
    {
        if (entity is null)
            throw new ArgumentNullException(nameof(entity));

        Command command = new Command("spCreateScheduleFile", true);
        command.AddParameter("filePath", entity.FilePath);
        command.AddParameter("scheduleId", entity.ScheduleId);

        string scalarResult = _connection.ExecuteScalar(command).ToString();

        if (string.IsNullOrEmpty(scalarResult))
            throw new DatabaseScalarNullException(nameof(scalarResult));

        return int.Parse(scalarResult);
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public ScheduleFileEntity GetByKey(int key)
    {
        if (key <= 0)
            throw new ArgumentOutOfRangeException(nameof(key));

        string query = "SELECT * FROM [dbo].[ScheduleFiles] WHERE [Id] = @id";

        Command command = new Command(query);
        command.AddParameter("id", key);

        return _connection
            .ExecuteReader(command, scheduleFile => scheduleFile.DbToScheduleFile())
            .SingleOrDefault();
    }

    /// <inheritdoc/>
    /// <exception cref="NotImplementedException"></exception>
    public bool Update(int key, ScheduleFileEntity entity)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException"></exception>
    public bool Delete(int key)
    {
        if (key <= 0)
            throw new ArgumentOutOfRangeException(nameof(key));

        string query = "DELETE FROM [dbo].[ScheduleFiles] WHERE [Id] = @id";

        Command command = new Command(query);
        command.AddParameter("id", key);

        return _connection.ExecuteNonQuery(command) > 0;
    }

    #endregion
}
