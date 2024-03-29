﻿using API.DepotEice.DAL.Entities;
using API.DepotEice.UIL.Models;
using API.DepotEice.UIL.Models.Forms;
using AutoMapper;

namespace API.DepotEice.UIL.Profiles
{
    /// <summary>
    /// Profile class for anything related to user models
    /// </summary>
    public class UserProfile : Profile
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public UserProfile()
        {
            CreateMap<UserEntity, UserModel>()
                .ForMember(
                    dest => dest.ProfilePictureUrl,
                    opt => opt.MapFrom(
                        src =>
                            src.ProfilePictureId.HasValue
                                ? $"Files/ById/{src.ProfilePictureId}"
                                : $"Files/DefaultProfilePicture"));

            CreateMap<UserEntity, UserRequestingModuleModel>();

            CreateMap<UserForm, UserEntity>();

            CreateMap<RegisterForm, UserEntity>();
        }
    }
}
