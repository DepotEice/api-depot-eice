﻿using API.DepotEice.BLL.Models;
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
            CreateMap<ArticleEntity, ArticleModel>();

            CreateMap<ArticleModel, ArticleCommentModel>()
                .ForMember(
                    dest => dest.Article,
                    opt => opt.MapFrom(src => src));
        }
    }
}
