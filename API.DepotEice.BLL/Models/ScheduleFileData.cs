namespace API.DepotEice.BLL.Models
{
    public class ScheduleFileData
    {
        public int Id { get; set; }
        public string FilePath { get; set; }
        public ScheduleData Schedule { get; set; }
    }
}
