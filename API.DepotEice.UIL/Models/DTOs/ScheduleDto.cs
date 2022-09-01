namespace API.DepotEice.UIL.DTOs;

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
