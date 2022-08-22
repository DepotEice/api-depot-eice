using API.DepotEice.BLL.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DepotEice.BLL.IServices
{
    public interface IMessageService
    {
        MessageDto? CreateMessage(MessageDto message);
        IEnumerable<MessageDto> GetUserMessages(string userId);
    }
}
