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
    /// <summary>
    /// Repository for <see cref="MessageEntity"/>
    /// </summary>
    public class MessageRepository : IMessageRepository
    {
        private readonly Connection _connection;

        public MessageRepository(Connection connection)
        {
            if (connection is null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            _connection = connection;
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="DatabaseScalarNullException"></exception>
        public int Create(MessageEntity entity)
        {
            if (entity is null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            Command command = new Command("spCreateMessage", true);

            command.AddParameter("body", entity.Content);
            command.AddParameter("userFromId", entity.SenderId);
            command.AddParameter("userToId", entity.ReceiverId);

            string? scalarResult = _connection.ExecuteScalar(command).ToString();

            if (string.IsNullOrEmpty(scalarResult))
            {
                throw new DatabaseScalarNullException(nameof(scalarResult));
            }

            return int.Parse(scalarResult);
        }

        /// <summary>
        /// Not implemented
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public bool Delete(MessageEntity entity)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Not implemented
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public IEnumerable<MessageEntity> GetAll()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Not implemented
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public MessageEntity? GetByKey(int key)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Retrieve user's messages sent and received
        /// </summary>
        /// <param name="userId">
        /// <see cref="UserEntity.Id"/>'s value
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> of <see cref="MessageEntity"/>
        /// </returns>
        /// <exception cref="ArgumentNullException"></exception>
        public IEnumerable<MessageEntity> GetUserMessages(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException(nameof(userId));
            }

            Command command = new Command("spGetUserMessages", true);

            command.AddParameter("userId", userId);

            return _connection
                .ExecuteReader(command, message => Mapper.DbToMessage(message));
        }

        /// <summary>
        /// Not implemented
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public bool Update(MessageEntity entity)
        {
            throw new NotImplementedException();
        }
    }
}
