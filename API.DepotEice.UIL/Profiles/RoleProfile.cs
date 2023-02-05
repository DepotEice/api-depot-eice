using API.DepotEice.DAL.Entities;
using API.DepotEice.UIL.Models;
using API.DepotEice.UIL.Models.Forms;
using AutoMapper;

namespace API.DepotEice.UIL.Profiles
{
    /// <summary>
    /// The profile for the role entities and models
    /// </summary>
    public class RoleProfile : Profile
    {
        /// <summary>
        /// Instanciate the RoleProfile
        /// </summary>
        public RoleProfile()
        {
            // RoleForm -> RoleEntity
            CreateMap<RoleForm, RoleEntity>();

            // RoleEntity -> RoleModel
            CreateMap<RoleEntity, RoleModel>();
        }
    }
}
