using API.DepotEice.DAL.Entities;
using API.DepotEice.UIL.Models;
using AutoMapper;

namespace API.DepotEice.UIL.Profiles
{
    /// <summary>
    /// Represents a configuration profile for mapping all classes related to messages
    /// </summary>
    public class MessageProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageProfile"/> class.
        /// </summary>
        public MessageProfile()
        {
            // CreateMap method configures a mapping between MessageEntity and ConversationModel.
            CreateMap<MessageEntity, ConversationModel>();
            // CreateMap method configures a mapping between MessageEntity and MessageModel.
            CreateMap<MessageEntity, MessageModel>();
        }
    }
}
