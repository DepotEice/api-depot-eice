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
        public UserModel Sender { get; set; }
        public UserModel Receiver { get; set; }
        public string Content { get; set; }
        public DateTime SentAt { get; set; }
    }
}
