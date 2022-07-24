using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DepotEice.BLL.Models
{
    public class AppointmentModel
    {
        public int Id { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public UserModel User { get; set; }
    }
}
