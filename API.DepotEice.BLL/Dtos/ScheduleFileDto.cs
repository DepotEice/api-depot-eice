namespace API.DepotEice.BLL.Dtos
{
    public class ScheduleFileDto
    {
        public int Id { get; set; }
        public string FilePath { get; set; }
        public ScheduleDto Schedule { get; set; }
    }
}
