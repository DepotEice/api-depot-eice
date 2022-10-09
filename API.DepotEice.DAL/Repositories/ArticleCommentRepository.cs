using API.DepotEice.DAL.Entities;
using API.DepotEice.DAL.IRepositories;
using API.DepotEice.DAL.Mappers;
using API.DepotEice.Helpers.Exceptions;
using DevHopTools.DataAccess;
using DevHopTools.DataAccess.Interfaces;

namespace API.DepotEice.DAL.Repositories;

public class ArticleCommentRepository : RepositoryBase, IArticleCommentRepository
{
    public ArticleCommentRepository(IDevHopConnection connection) : base(connection) { }

    /// <summary>
    /// Retrieve all <c>ArticleComment</c> records <paramref name="articleId"/> from the 
    /// database 
    /// </summary>
    /// <param name="articleId">
    /// The ID of <see cref="ArticleEntity"/> to retrieve
    /// </param>
    /// <returns>
    /// An <see cref="IEnumerable{T}"/> of <see cref="ArticleCommentEntity"/>
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public IEnumerable<ArticleCommentEntity> GetArticleComments(int articleId)
    {
        if (articleId <= 0)
            throw new ArgumentOutOfRangeException(nameof(articleId));

        string query = "SELECT * FROM [dbo].[ArticleComments] WHERE [ArticleId] = @articleId";

        Command command = new Command(query);

        command.AddParameter("articleId", articleId);

        return _connection.ExecuteReader(command,
            articleComment => Mapper.DbToArticleComment(articleComment));
    }

    /// <inheritdoc/>
    /// <exception cref="NotImplementedException"></exception>
    public IEnumerable<ArticleCommentEntity> GetAll()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="DatabaseScalarNullException"></exception>
    public int Create(ArticleCommentEntity entity)
    {
        if (entity is null)
            throw new ArgumentNullException(nameof(entity));

        Command command = new Command("spCreateArticleComment", true);

        command.AddParameter("note", entity.Note);
        command.AddParameter("review", entity.Review);
        command.AddParameter("articleId", entity.ArticleId);
        command.AddParameter("userId", entity.UserId);

        string? scalarResult = _connection.ExecuteScalar(command).ToString();

        if (string.IsNullOrEmpty(scalarResult))
            throw new DatabaseScalarNullException(nameof(scalarResult));

        return int.Parse(scalarResult);
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public ArticleCommentEntity GetByKey(int key)
    {
        if (key <= 0)
            throw new ArgumentOutOfRangeException(nameof(key));

        string query = "SELECT * FROM [Appointments] WHERE [Appointments].[Id] = @id";

        Command command = new Command(query);

        command.AddParameter("id", key);

        return _connection
            .ExecuteReader(command,
                articleComment => Mapper.DbToArticleComment(articleComment))
            .SingleOrDefault();
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException"></exception>
    public bool Update(int key, ArticleCommentEntity entity)
    {
        if (entity is null)
            throw new ArgumentNullException(nameof(entity));

        string query = "UPDATE [dbo].[ArticleComments] SET [Note] = @note, [Review] = @review WHERE [Id] = @id";

        Command command = new Command(query);

        command.AddParameter("id", key);
        command.AddParameter("note", entity.Note);
        command.AddParameter("review", entity.Review);

        return _connection.ExecuteNonQuery(command) > 0;
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public bool Delete(int key)
    {
        if (key <= 0)
            throw new ArgumentOutOfRangeException(nameof(key));

        string query = "DELETE FROM [dbo].[ArticleComments] WHERE [Id] = @id";

        Command command = new Command(query);

        command.AddParameter("id", key);

        return _connection.ExecuteNonQuery(command) > 0;
    }
}
