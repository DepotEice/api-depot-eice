using API.DepotEice.DAL.Entities;
using API.DepotEice.DAL.IRepositories;
using API.DepotEice.DAL.Mappers;
using API.DepotEice.Helpers.Exceptions;
using DevHopTools.DataAccess;
using DevHopTools.DataAccess.Interfaces;

namespace API.DepotEice.DAL.Repositories;

public class ArticleRepository : RepositoryBase, IArticleRepository
{
    public ArticleRepository(IDevHopConnection connection) : base(connection) { }

    public bool ArticlePinDecision(int id, bool isPinned = true)
    {
        if (id <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(id));
        }

        string query = "UPDATE [dbo].[Articles] SET [IsPinned] = @isPinned WHERE [Id] = @id";

        Command command = new Command(query);
        command.AddParameter("id", id);
        command.AddParameter("isPinned", isPinned);

        return _connection.ExecuteNonQuery(command) > 0;
    }

    /// <inheritdoc/>
    /// <returns></returns>
    public IEnumerable<ArticleEntity> GetAll()
    {
        Command command = new Command("SELECT * FROM [dbo].[Articles]");

        return _connection.ExecuteReader(command, article => article.DbToArticle());
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="DatabaseScalarNullException"></exception>
    public int Create(ArticleEntity entity)
    {
        if (entity is null)
            throw new ArgumentNullException(nameof(entity));

        Command command = new Command("spArticleCreate", true);

        command.AddParameter("mainImageId", entity.MainImageId);
        command.AddParameter("title", entity.Title);
        command.AddParameter("body", entity.Body);
        command.AddParameter("pinned", entity.IsPinned);
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
    public ArticleEntity? GetByKey(int key)
    {
        if (key <= 0)
            throw new ArgumentOutOfRangeException(nameof(key));

        string query = "SELECT * FROM [Articles] WHERE [Articles].[Id] = @id";

        Command command = new Command(query);

        command.AddParameter("id", key);

        return _connection
            .ExecuteReader(command, article => article.DbToArticle())
            .SingleOrDefault();
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public bool Update(int key, ArticleEntity entity)
    {
        if (key <= 0)
            throw new ArgumentOutOfRangeException(nameof(key));

        string query = "UPDATE [dbo].[Articles] SET [Title] = @title, [Body] = @body WHERE [Id] = @id";

        Command command = new Command(query);

        command.AddParameter("mainImageId", entity.MainImageId);
        command.AddParameter("id", key);
        command.AddParameter("title", entity.Title);
        command.AddParameter("body", entity.Body);

        return _connection.ExecuteNonQuery(command) > 0;
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public bool Delete(int key)
    {
        if (key <= 0)
            throw new ArgumentOutOfRangeException(nameof(key));

        string query = "DELETE FROM [dbo].[Articles] WHERE [Id] = @id";

        Command command = new Command(query);

        command.AddParameter("id", key);

        return _connection.ExecuteNonQuery(command) > 0;
    }

    public bool Restore(int key)
    {
        if (key <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(key));
        }

        string query = "UPDATE [dbo].[Articles] SET [DeletedAt] = NULL, [UpdatedAt] = GETDATE() WHERE [Id] = @id";

        Command command = new Command(query);

        command.AddParameter("id", key);

        return _connection.ExecuteNonQuery(command) > 0;
    }
}
