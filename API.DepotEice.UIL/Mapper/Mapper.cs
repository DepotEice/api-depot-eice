using API.DepotEice.BLL.Dtos;
using API.DepotEice.UIL.Models;
using API.DepotEice.UIL.Models.Forms;
using DevHopTools.Extensions;

namespace API.DepotEice.UIL.Mapper;

internal static class Mapper
{
    // Users
    internal static UserModel ToUil(this UserDto dto) => dto.Map<UserModel>();

    // Modules
    internal static ModuleModel ToUil(this ModuleDto data) => data.Map<ModuleModel>();
    internal static ModuleDto ToBll(this ModuleForm form) => form.Map<ModuleDto>();

    // Schedules
    internal static ScheduleModel ToUil(this ScheduleDto data) => data.Map<ScheduleModel>();
    internal static ScheduleDto ToBll(this ScheduleForm form) => form.Map<ScheduleDto>();

    // Schedule Files
    internal static ScheduleFileModel ToUil(this ScheduleFileDto data) => data.Map<ScheduleFileModel>();
    internal static ScheduleFileDto ToBll(this ScheduleFileDto data) => data.Map<ScheduleFileDto>();
    internal static ScheduleFileDto ToBll(this ScheduleFileModel model) => model.Map<ScheduleFileDto>();

    // Articles
    internal static ArticleModel ToUil(this ArticleDto dto)
    {
        ArticleModel article = dto.Map<ArticleModel>();
        article.User = dto.User.ToUil();
        return article;
    }
    internal static ArticleDto ToBll(this ArticleForm form) => form.Map<ArticleDto>();

    // Comments
    internal static CommentModel ToUil(this ArticleCommentDto dto) => dto.Map<CommentModel>();
    internal static ArticleCommentDto ToBll(this CommentForm form) => form.Map<ArticleCommentDto>();
}
