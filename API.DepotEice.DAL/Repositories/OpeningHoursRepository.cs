using API.DepotEice.DAL.Entities;
using API.DepotEice.DAL.IRepositories;
using API.DepotEice.DAL.Mappers;
using API.DepotEice.Helpers.Exceptions;
using DevHopTools.DataAccess;
using DevHopTools.DataAccess.Interfaces;

namespace API.DepotEice.DAL.Repositories;

/// <summary>
/// Repository for <see cref="OpeningHoursEntity"/>
/// </summary>
public class OpeningHoursRepository : RepositoryBase, IOpeningHoursRepository
{
    public OpeningHoursRepository(IDevHopConnection connection) : base(connection) { }

    /// <inheritdoc/>
    public IEnumerable<OpeningHoursEntity> GetAll()
    {
        string query = "SELECT * FROM [dbo].[OpeningHours]";

        Command command = new Command(query);

        return _connection
            .ExecuteReader(command, openingHours => Mapper.DbToOpeningHours(openingHours));
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="DatabaseScalarNullException"></exception>
    public int Create(OpeningHoursEntity entity)
    {
        if (entity is null)
        {
            throw new ArgumentNullException(nameof(entity));
        }

        Command command = new Command("spCreateOpeningHours", true);

        command.AddParameter("openAt", entity.OpenAt);
        command.AddParameter("closeAt", entity.CloseAt);

        string? scalarResult = _connection.ExecuteScalar(command).ToString();

        if (string.IsNullOrEmpty(scalarResult))
        {
            throw new DatabaseScalarNullException(nameof(scalarResult));
        }

        return int.Parse(scalarResult);
    }

    /// <inheritdoc/>
    public OpeningHoursEntity GetByKey(int key)
    {
        if (key <= 0)
            throw new ArgumentOutOfRangeException(nameof(key));

        string query = "SELECT * FROM [dbo].[OpeningHours] WHERE [Id] = @id";

        Command command = new Command(query);
        command.AddParameter("id", key);

        return _connection
            .ExecuteReader(command, record => record.DbToOpeningHours())
            .SingleOrDefault();
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public bool Update(int key, OpeningHoursEntity entity)
    {
        if (key <= 0)
            throw new ArgumentOutOfRangeException(nameof(key));

        if (entity is null)
            throw new ArgumentNullException(nameof(entity));

        string query = "UPDATE [dbo].[OpeningHours] SET [OpenAt] = @openAt, [CloseAt] = @closeAt WHERE [Id] = @id";

        Command command = new Command(query);

        command.AddParameter("id", key);
        command.AddParameter("openAt", entity.OpenAt);
        command.AddParameter("closeAt", entity.CloseAt);

        return _connection.ExecuteNonQuery(command) > 0;
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public bool Delete(int key)
    {
        if (key <= 0)
            throw new ArgumentOutOfRangeException(nameof(key));

        string query = "DELETE FROM [dbo].[OpeningHours] WHERE [Id] = @id";

        Command command = new Command(query);

        command.AddParameter("id", key);

        return _connection.ExecuteNonQuery(command) > 0;
    }
}
