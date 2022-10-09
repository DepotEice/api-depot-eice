namespace API.DepotEice.UIL.DTOs;

public class ScheduleFileDto
{
    public int Id { get; set; }
    public string FilePath { get; set; }
    public ScheduleDto Schedule { get; set; }
}
