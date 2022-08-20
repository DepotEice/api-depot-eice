using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DepotEice.BLL.Dtos
{
    public class ScheduleDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Details { get; set; }
        public DateTime StartsAt { get; set; }
        public DateTime EndsAt { get; set; }
        public ModuleDto Module { get; set; }
        public IEnumerable<ScheduleFileDto> ScheduleFiles { get; set; }
    }
}
