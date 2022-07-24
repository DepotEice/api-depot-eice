using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DepotEice.BLL.Models
{
    public class ScheduleModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Details { get; set; }
        public DateTime StartsAt { get; set; }
        public DateTime EndsAt { get; set; }
        public ModuleModel Module { get; set; }
        public IEnumerable<ScheduleFile> ScheduleFiles { get; set; }
    }
}
