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
public class OpeningHoursRepository : IOpeningHoursRepository
{
    private readonly IDevHopConnection _connection;

    public OpeningHoursRepository(IDevHopConnection connection)
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
    /// <exception cref="ArgumentNullException"></exception>
    public bool Delete(OpeningHoursEntity entity)
    {
        if (entity is null)
        {
            throw new ArgumentNullException(nameof(entity));
        }

        Command command = new Command("spDeleteOpeningHours", true);

        command.AddParameter("id", entity.Id);

        return _connection.ExecuteNonQuery(command) > 0;
    }

    public IEnumerable<OpeningHoursEntity> GetAll()
    {
        string query = "SELECT * FROM [dbo].[OpeningHours]";

        Command command = new Command(query);

        return _connection
            .ExecuteReader(command, openingHours => Mapper.DbToOpeningHours(openingHours));
    }

    /// <summary>
    /// Not implemented
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public OpeningHoursEntity? GetByKey(int key)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException"></exception>
    public bool Update(OpeningHoursEntity entity)
    {
        if (entity is null)
        {
            throw new ArgumentNullException(nameof(entity));
        }

        Command command = new Command("spUpdateOpeningHours", true);

        command.AddParameter("id", entity.Id);
        command.AddParameter("openAt", entity.OpenAt);
        command.AddParameter("closeAt", entity.CloseAt);

        return _connection.ExecuteNonQuery(command) > 0;
    }
}
