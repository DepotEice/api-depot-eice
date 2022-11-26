using API.DepotEice.DAL.Entities;
using API.DepotEice.DAL.IRepositories;
using API.DepotEice.UIL.Models;
using API.DepotEice.UIL.Models.Forms;
using AutoMapper;

namespace API.DepotEice.UIL.Profiles
{
    /// <summary>
    /// Article profile for Automapper
    /// </summary>
    public class ArticleProfile : Profile
    {
        /// <summary>
        /// Constructor containing all mapping
        /// </summary>
        public ArticleProfile()
        {
            CreateMap<ArticleForm, ArticleEntity>()
                .ForMember(
                    dest => dest.IsPinned,
                    opt => opt.MapFrom(
                        src => src.Pinned));

            CreateMap<ArticleEntity, ArticleModel>()
                .ForMember(
                    dest => dest.Pinned,
                    opt => opt.MapFrom(
                        src => src.IsPinned));
        }
    }
}
