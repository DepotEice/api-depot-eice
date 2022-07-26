using API.DepotEice.DAL.Entities;
using API.DepotEice.DAL.IRepositories;
using API.DepotEice.DAL.Mappers;
using API.DepotEice.Helpers.Exceptions;
using DevHopTools.Connection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DepotEice.DAL.Repositories
{
    public class ArticleRepository : IArticleRepository
    {
        private readonly Connection _connection;

        public ArticleRepository(Connection connection)
        {
            if (connection is null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            _connection = connection;
        }

        /// <summary>
        /// Create a new <c>Article</c> record in the database
        /// </summary>
        /// <param name="entity">
        /// The entity to create
        /// </param>
        /// <returns>
        /// The ID of the newly created record. <c>0</c> Otherwise
        /// </returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="DatabaseScalarNullException"></exception>
        public int Create(ArticleEntity entity)
        {
            if (entity is null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            Command command = new Command("spCreateArticle", true);

            command.AddParameter("title", entity.Title);
            command.AddParameter("body", entity.Body);
            command.AddParameter("pinned", entity.Pinned);
            command.AddParameter("userId", entity.UserId);

            string? scalarResult = _connection.ExecuteScalar(command).ToString();

            if (string.IsNullOrEmpty(scalarResult))
            {
                throw new DatabaseScalarNullException(nameof(scalarResult));
            }

            return int.Parse(scalarResult);
        }

        /// <summary>
        /// Delete the <paramref name="entity"/> record from the database
        /// </summary>
        /// <param name="entity">
        /// The <c>Article</c> record to delete
        /// </param>
        /// <returns>
        /// <c>true</c> If one or more row was affected by the query. <c>false</c> Otherwise
        /// </returns>
        /// <exception cref="ArgumentNullException"></exception>
        public bool Delete(ArticleEntity entity)
        {
            if (entity is null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            Command command = new Command("spDeleteArticle", true);

            command.AddParameter("id", entity.Id);

            return _connection.ExecuteNonQuery(command) > 0;
        }

        /// <summary>
        /// Retrieve all <c>Article</c> records from the database
        /// </summary>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> of <see cref="ArticleEntity"/>
        /// </returns>
        public IEnumerable<ArticleEntity> GetAll()
        {
            Command command = new Command("SELECT * FROM [dbo].[Articles]");

            return _connection
                .ExecuteReader(command, article => Mapper.DbToArticle(article));
        }

        /// <summary>
        /// Retrieve an <c>Article</c> record from the database
        /// </summary>
        /// <param name="key">
        /// <c>Article</c>'s ID
        /// </param>
        /// <returns>
        /// <c>null</c> If more than one or no record at all. <see cref="ArticleEntity"/> otherwise
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public ArticleEntity? GetByKey(int key)
        {
            if (key <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(key));
            }

            Command command = new Command("spGetArticle", true);

            command.AddParameter("id", key);

            return _connection
                .ExecuteReader(command, article => Mapper.DbToArticle(article))
                .SingleOrDefault();
        }

        // TODO : Implement PinArticle
        public bool PinArticle(int id, bool isPinned)
        {
            throw new NotImplementedException();
        }

        public bool Update(ArticleEntity entity)
        {
            throw new NotImplementedException();
        }
    }
}
