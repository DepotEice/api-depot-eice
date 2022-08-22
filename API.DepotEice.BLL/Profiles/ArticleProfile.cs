using API.DepotEice.BLL.Dtos;
using API.DepotEice.DAL.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DepotEice.BLL.Profiles
{
    public class ArticleProfile : Profile
    {
        public ArticleProfile()
        {
            // ArticleModel --> ArticleEntity
            CreateMap<ArticleDto, ArticleEntity>()
                .ForMember(
                    dest => dest.UserId,
                    opt => opt.MapFrom(src => src.User.Id));

            // ArticleEntity --> Article Model

            CreateMap<ArticleEntity, ArticleDto>();

            // ArticleModel --> Other objects

            CreateMap<ArticleDto, ArticleCommentDto>()
                .ForMember(
                    dest => dest.Article,
                    opt => opt.MapFrom(src => src));
        }
    }
}
