using API.DepotEice.BLL.Dtos;
using API.DepotEice.UIL.Models;
using API.DepotEice.UIL.Models.Forms;
using DevHopTools.Mappers;

namespace API.DepotEice.UIL.Mapper;

internal static class Mapper
{
    // Users
    internal static UserDto ToBll(this RegisterForm form) => form.Map<UserDto>();
    internal static LoggedInUserModel ToUil(this UserDto dto)
    {
        LoggedInUserModel model = new LoggedInUserModel()
        {
            Id = dto.Id,
            Email = dto.Email,
            ProfilePicture = dto.ProfilePicture,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            BirthDate = dto.BirthDate,
            CreatedAt = dto.CreatedAt,
            UpdatedAt = dto.UpdatedAt,
            DeletedAt = dto.DeletedAt,
        };

        return model;
    }

    // Modules
    internal static ModuleModel ToUil(this ModuleDto data)  => data.Map<ModuleModel>();
    internal static ModuleDto ToBll(this ModuleForm form) => form.Map<ModuleDto>();

    // Schedules
    internal static ScheduleModel ToUil(this ScheduleDto data) => data.Map<ScheduleModel>();
    internal static ScheduleDto ToBll(this ScheduleForm form) => form.Map<ScheduleDto>();

    // Schedule Files
    internal static ScheduleFileModel ToUil(this ScheduleFileDto data) => data.Map<ScheduleFileModel>();
    internal static ScheduleFileDto ToBll(this ScheduleFileDto data) => data.Map<ScheduleFileDto>();
    internal static ScheduleFileDto ToBll(this ScheduleFileModel model) => model.Map<ScheduleFileDto>();

}
