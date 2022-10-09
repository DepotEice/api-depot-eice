using API.DepotEice.DAL.Entities;

namespace API.DepotEice.DAL.IRepositories;

public interface IMessageRepository : IRepositoryBase<int, MessageEntity>
{
    IEnumerable<MessageEntity> GetUserMessages(string userId);
}
