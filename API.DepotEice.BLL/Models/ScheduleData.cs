using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DepotEice.BLL.Models
{
    public class ScheduleData
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Details { get; set; }
        public DateTime StartsAt { get; set; }
        public DateTime EndsAt { get; set; }
        public ModuleData Module { get; set; }
        public IEnumerable<ScheduleFileData> ScheduleFiles { get; set; }
    }
}
