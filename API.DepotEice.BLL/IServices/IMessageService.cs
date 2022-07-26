using API.DepotEice.BLL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DepotEice.BLL.IServices
{
    public interface IMessageService
    {
        MessageModel? CreateMessage(MessageModel message);
        IEnumerable<MessageModel> GetUserMessages(string userId);
    }
}
