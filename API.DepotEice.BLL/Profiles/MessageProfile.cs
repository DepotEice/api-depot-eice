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
    public class MessageProfile : Profile
    {
        public MessageProfile()
        {
            // MessageModel --> MessageEntity

            CreateMap<MessageModel, MessageEntity>()
                .ForMember(
                    dest => dest.SenderId,
                    opt => opt.MapFrom(src => src.Sender.Id))
                .ForMember(
                    dest => dest.ReceiverId,
                    opt => opt.MapFrom(src => src.Receiver.Id));

            // MessageEntity --> MessageModel

            CreateMap<MessageEntity, MessageModel>();
        }
    }
}
