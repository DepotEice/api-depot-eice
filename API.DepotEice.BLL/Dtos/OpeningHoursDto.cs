using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DepotEice.BLL.Dtos
{
    public class OpeningHoursDto
    {
        public int Id { get; set; }
        public DateTime OpenAt { get; set; }
        public DateTime CloseAt { get; set; }
    }
}
