using API.DepotEice.DAL.Entities;
using AutoMapper;

namespace API.DepotEice.UIL.Profiles
{
    /// <summary>
    /// Hold the mapping profile for the file entity, models, forms, etc.
    /// </summary>
    public class FileProfile : Profile
    {
        /// <summary>
        /// FileProfile controller
        /// </summary>
        public FileProfile()
        {
            CreateMap<IFormFile, FileEntity>()
                .ForMember(
                    dest => dest.Key,
                    opt => opt.MapFrom(src => src.FileName))
                .ForMember(
                    dest => dest.Size,
                    opt => opt.MapFrom(src => src.Length))
                .ForMember(
                    dest => dest.Type,
                     opt => opt.MapFrom(src => src.ContentType));
        }
    }
}
