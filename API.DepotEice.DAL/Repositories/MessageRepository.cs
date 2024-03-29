﻿using API.DepotEice.DAL.Entities;
using API.DepotEice.DAL.IRepositories;
using API.DepotEice.DAL.Mappers;
using API.DepotEice.Helpers.Exceptions;
using DevHopTools.DataAccess;
using DevHopTools.DataAccess.Interfaces;

namespace API.DepotEice.DAL.Repositories;

/// <summary>
/// Repository for <see cref="MessageEntity"/>
/// </summary>
public class MessageRepository : RepositoryBase, IMessageRepository
{
    public MessageRepository(IDevHopConnection connection) : base(connection) { }

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

        string query = "SELECT * FROM [Messages] WHERE ([ReceiverId] = @userId) OR ([SenderId] = @userId)";

        Command command = new Command(query);

        command.AddParameter("userId", userId);

        return _connection.ExecuteReader(command, message => message.DbToMessage());
    }

    /// <summary>
    /// Set all the message between sender and receiver as read
    /// </summary>
    /// <param name="senderId">The id of the sender</param>
    /// <param name="receiverId">The id of the receiver</param>
    /// <returns>
    /// true If the messages have been marked as read, false otherwise
    /// </returns>
    /// <exception cref="ArgumentNullException"></exception>
    public bool MarkConversationAsRead(string senderId, string receiverId)
    {
        if (string.IsNullOrEmpty(senderId))
        {
            throw new ArgumentNullException(nameof(senderId));
        }

        if (string.IsNullOrEmpty(receiverId))
        {
            throw new ArgumentNullException(nameof(receiverId));
        }

        string query = "UPDATE [Messages] SET [Read] = 1 WHERE ([SenderId] = @senderId) AND ([ReceiverId] = @receiverId)";

        Command command = new Command(query);

        command.AddParameter("senderId", senderId);
        command.AddParameter("receiverId", receiverId);

        return _connection.ExecuteNonQuery(command) > 0;
    }

    /// <summary>
    /// Mark a message between sender and receiver as read
    /// </summary>
    /// <param name="senderId">The id of the message sender</param>
    /// <param name="receiverId">The id of the message receiver</param>
    /// <param name="messageId">The id of the message</param>
    /// <returns>
    /// true If the message has been marked as read, false otherwise
    /// </returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public bool MarkAsRead(string senderId, string receiverId, int messageId)
    {
        if (string.IsNullOrEmpty(senderId))
        {
            throw new ArgumentNullException(nameof(senderId));
        }

        if (string.IsNullOrEmpty(receiverId))
        {
            throw new ArgumentNullException(nameof(receiverId));
        }

        if (messageId < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(messageId));
        }

        string query =
            @"UPDATE [Messages] 
            SET [IsRead] = 1 
            WHERE ([SenderId] = @senderId) 
            AND ([ReceiverId] = @receiverId)
            AND ([Id] = @id)";

        Command command = new Command(query);

        command.AddParameter("senderId", senderId);
        command.AddParameter("receiverId", receiverId);
        command.AddParameter("id", messageId);

        return _connection.ExecuteNonQuery(command) > 0;
    }

    public IEnumerable<MessageEntity> GetUserMessages()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    /// <exception cref="NotImplementedException"></exception>
    public IEnumerable<MessageEntity> GetAll()
    {
        throw new NotImplementedException();
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

        command.AddParameter("content", entity.Content);
        command.AddParameter("senderId", entity.SenderId);
        command.AddParameter("receiverId", entity.ReceiverId);

        string? scalarResult = _connection.ExecuteScalar(command).ToString();

        if (string.IsNullOrEmpty(scalarResult))
        {
            throw new DatabaseScalarNullException(nameof(scalarResult));
        }

        return int.Parse(scalarResult);
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public MessageEntity? GetByKey(int key)
    {
        if (key <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(key));
        }

        string query = "SELECT * FROM [dbo].[Messages] WHERE [Messages].[Id] = @id";

        Command command = new Command(query);
        command.AddParameter("id", key);

        return _connection
            .ExecuteReader(command, message => message.DbToMessage())
            .SingleOrDefault();
    }

    /// <inheritdoc />
    /// <exception cref="NotImplementedException"></exception>
    public bool Update(int key, MessageEntity entity)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    /// <exception cref="NotImplementedException"></exception>
    public bool Delete(int key)
    {
        throw new NotImplementedException();
    }
}
