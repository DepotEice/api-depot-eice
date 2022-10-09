using API.DepotEice.BLL.Dtos;
using API.DepotEice.DAL.Entities;
using DevHopTools.Mappers;

namespace API.DepotEice.BLL.Mappers;

internal static class Mapper
{
    // Users
    internal static UserDto ToBll(this UserEntity entity) => entity.Map<UserDto>();
    internal static UserEntity ToDal(this UserDto dto) => dto.Map<UserEntity>();

    // Articles
    internal static ArticleDto ToBll(this ArticleEntity entity) => entity.Map<ArticleDto>();
    internal static ArticleEntity ToDal(this ArticleDto dto) => dto.Map<ArticleEntity>();

    // Comments
    internal static ArticleCommentDto ToBll(this ArticleCommentEntity entity) => entity.Map<ArticleCommentDto>();
    internal static ArticleCommentEntity ToDal(this ArticleCommentEntity dto) => dto.Map<ArticleCommentEntity>();
}
