using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DepotEice.BLL.Models
{
    public class ScheduleFileModel
    {
        public int Id { get; set; }
        public string Path { get; set; }
        public ScheduleModel Schedule { get; set; }
    }
}
