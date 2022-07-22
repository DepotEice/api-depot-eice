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
    public class ArticleCommentRepository : IArticleCommentRepository
    {
        private readonly Connection _connection;

        public ArticleCommentRepository(Connection connection)
        {
            if (connection is null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            _connection = connection;
        }

        /// <summary>
        /// Create a new <c>ArticleComment</c> record in the database. If the scalar result is 
        /// <c>null</c>, an exception <see cref="DatabaseScalarNullException"/> is thrown
        /// </summary>
        /// <param name="entity">
        /// The new record to create. Throws an <see cref="ArgumentNullException"/> if it is null
        /// </param>
        /// <returns>
        /// The ID of the newly created record. If the creation failed, return <c>0</c>
        /// </returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="DatabaseScalarNullException"></exception>
        public int Create(ArticleCommentEntity entity)
        {
            if (entity is null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            Command command = new Command("spCreateArticleComment", true);

            command.AddParameter("note", entity.Note);
            command.AddParameter("review", entity.Review);
            command.AddParameter("articleId", entity.ArticleId);
            command.AddParameter("userId", entity.UserId);

            string? scalarResult = _connection.ExecuteScalar(command).ToString();

            if (string.IsNullOrEmpty(scalarResult))
            {
                throw new DatabaseScalarNullException(nameof(scalarResult));
            }

            return int.Parse(scalarResult);
        }

        /// <summary>
        /// Delete <paramref name="entity"/> from the database
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>
        /// <c>true</c> If one or more row has been affected by the query. <c>false</c> Otherwise
        /// </returns>
        /// <exception cref="ArgumentNullException"></exception>
        public bool Delete(ArticleCommentEntity entity)
        {
            if (entity is null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            Command command = new Command("spDeleteArticleComment", true);

            command.AddParameter("id", entity.Id);

            return _connection.ExecuteNonQuery(command) > 0;
        }

        /// <summary>
        /// Not implemented
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public IEnumerable<ArticleCommentEntity> GetAll()
        {
            throw new NotImplementedException();
        }

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
            {
                throw new ArgumentOutOfRangeException(nameof(articleId));
            }

            Command command = new Command("spGetArticleComments", true);

            command.AddParameter("articleId", articleId);

            return _connection.ExecuteReader(command,
                articleComment => Mapper.DbToArticleComment(articleComment));
        }

        /// <summary>
        /// Retrieve a record <c>ArticleComment</c> with the given ID <paramref name="key"/> from
        /// the database
        /// </summary>
        /// <param name="key">
        /// The ID of the researched record
        /// </param>
        /// <returns>
        /// <c>null</c> If there is no return value or if the returned value is not unique. 
        /// <see cref="ArticleCommentEntity"/> Otherwise
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public ArticleCommentEntity? GetByKey(int key)
        {
            if (key <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(key));
            }

            Command command = new Command("spGetArticleComment", true);

            command.AddParameter("id", key);

            return _connection
                .ExecuteReader(command,
                    articleComment => Mapper.DbToArticleComment(articleComment))
                .SingleOrDefault();
        }

        /// <summary>
        /// Update the given <paramref name="entity"/> in the database
        /// </summary>
        /// <param name="entity">
        /// The entity to update
        /// </param>
        /// <returns>
        /// <c>true</c> If one or more record were modified. <c>false</c> Otherwise
        /// </returns>
        /// <exception cref="ArgumentNullException"></exception>
        public bool Update(ArticleCommentEntity entity)
        {
            if (entity is null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            Command command = new Command("spUpdateArticleComment", true);

            command.AddParameter("id", entity.Id);
            command.AddParameter("note", entity.Note);
            command.AddParameter("review", entity.Review);

            return _connection.ExecuteNonQuery(command) > 0;
        }
    }
}
