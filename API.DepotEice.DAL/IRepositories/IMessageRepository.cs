using API.DepotEice.DAL.Entities;

namespace API.DepotEice.DAL.IRepositories;

public interface IMessageRepository : IRepositoryBase<int, MessageEntity>
{
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
    IEnumerable<MessageEntity> GetUserMessages(string userId);

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
    bool MarkAsRead(string senderId, string receiverId, int messageId);

    /// <summary>
    /// Set all the message between sender and receiver as read
    /// </summary>
    /// <param name="senderId">The id of the sender</param>
    /// <param name="receiverId">The id of the receiver</param>
    /// <returns>
    /// true If the messages have been marked as read, false otherwise
    /// </returns>
    /// <exception cref="ArgumentNullException"></exception>
    bool MarkConversationAsRead(string senderId, string receiverId);
}
