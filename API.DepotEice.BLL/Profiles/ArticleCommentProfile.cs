using API.DepotEice.BLL.Models;
using API.DepotEice.DAL.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DepotEice.BLL.Profiles
{
    public class ArticleCommentProfile : Profile
    {
        public ArticleCommentProfile()
        {
            // ArticleCommentModel -> ArticleCommentEntity

            CreateMap<ArticleCommentModel, ArticleCommentEntity>()
                .ForMember(
                    dest => dest.ArticleId,
                    opt => opt.MapFrom(src => src.Article.Id))
                .ForMember(
                    dest => dest.UserId,
                    opt => opt.MapFrom(src => src.User.Id));

            // ArticleCommentEntity -> ArticleCommentModel

            CreateMap<ArticleCommentEntity, ArticleCommentModel>();

            // IEnumerable<ArticleCommentModel> --> ArticleModel

            CreateMap<IEnumerable<ArticleCommentModel>, ArticleModel>()
                .ForMember(
                    dest => dest.ArticleComments,
                    opt => opt.MapFrom(src => src));
        }
    }
}
