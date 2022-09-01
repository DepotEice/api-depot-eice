namespace API.DepotEice.UIL.DTOs;

public class ModuleDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public IEnumerable<ScheduleDto> Schedules { get; set; }
    public IEnumerable<UserDto> Users { get; set; }
}
