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
    /// Mark all the messages sent by the sender and received by the receiver as read
    /// </summary>
    /// <param name="senderId">
    /// The id of the sender
    /// </param>
    /// <param name="receiverId">
    /// The id of the receiver
    /// </param>
    /// <returns>
    /// true if the messages have been marked as read, false otherwise
    /// </returns>
    /// <exception cref="ArgumentNullException"></exception>
    bool MarkAsRead(string senderId, string receiverId);
}
