using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DepotEice.BLL.Models
{
    public class UserTokenDto
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
        public DateTime DeliveryDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public UserDto? User { get; set; }
    }
}
