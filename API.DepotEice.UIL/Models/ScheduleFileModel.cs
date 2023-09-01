namespace API.DepotEice.UIL.Models
{
    public class ScheduleFileModel
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public int ScheduleId { get; set; }
        public string FileExtension { get; set; }
        public int FileId { get; set; }
        public string FilePath { get; set; }
    }
}
