using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DepotEice.BLL.Models
{
    public class MessageModel
    {
        public int Id { get; set; }
        public UserDto Sender { get; set; }
        public UserDto Receiver { get; set; }
        public string Content { get; set; }
        public DateTime SentAt { get; set; }
    }
}
