using API.DepotEice.BLL.IServices;
using API.DepotEice.BLL.Dtos;
using API.DepotEice.DAL.Entities;
using API.DepotEice.DAL.IRepositories;
using AutoMapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DepotEice.BLL.Services
{
    public class MessageService : IMessageService
    {
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly IMessageRepository _messageRepository;
        private readonly IUserRepository _userRepository;

        public MessageService(ILogger<MessageService> logger, IMapper mapper,
            IMessageRepository messageRepository, IUserRepository userRepository)
        {
            if (logger is null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            if (mapper is null)
            {
                throw new ArgumentNullException(nameof(mapper));
            }

            if (messageRepository is null)
            {
                throw new ArgumentNullException(nameof(messageRepository));
            }

            if (userRepository is null)
            {
                throw new ArgumentNullException(nameof(userRepository));
            }

            _logger = logger;
            _mapper = mapper;
            _messageRepository = messageRepository;
            _userRepository = userRepository;
        }

        /// <summary>
        /// Create a message in the database
        /// </summary>
        /// <param name="message">
        /// The message to create
        /// </param>
        /// <returns>
        /// <c>null</c> If the message couldn't be created or if the sender User or the receiver 
        /// User does not exist in the database. Otherwise an instance of <see cref="MessageDto"/>
        /// </returns>
        /// <exception cref="ArgumentNullException"></exception>
        public MessageDto? CreateMessage(MessageDto message)
        {
            if (message is null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            MessageEntity messageEntity = _mapper.Map<MessageEntity>(message);

            int newId = _messageRepository.Create(messageEntity);

            if (newId <= 0)
            {
                _logger.LogError(
                    "{date} - The created ID is smaller or equals to 0!",
                    DateTime.Now);

                return null;
            }

            MessageEntity? createdMessage = _messageRepository.GetByKey(newId);

            if (createdMessage is null)
            {
                _logger.LogWarning(
                    "{date} - The retrieved message with the newly created ID \"{id}\" " +
                    "does not exist in the database!",
                    DateTime.Now, newId);

                return null;
            }

            UserEntity? sender = _userRepository.GetByKey(createdMessage.SenderId);

            if (sender is null)
            {
                _logger.LogError(
                    "{date} - The sender User with ID \"{userId}\" related to the Message with ID " +
                    "\"{messageId}\" with  does not exist in the database!",
                    DateTime.Now, createdMessage.SenderId, createdMessage.Id);

                return null;
            }

            UserEntity? receiver = _userRepository.GetByKey(createdMessage.ReceiverId);

            if (receiver is null)
            {
                _logger.LogError(
                    "{date} - The receiver User with ID \"{userId}\" related to the Message with " +
                    "ID \"{messageId}\" with  does not exist in the database!",
                    DateTime.Now, createdMessage.ReceiverId, createdMessage.Id);

                return null;
            }

            MessageDto messageModel = _mapper.Map<MessageDto>(createdMessage);

            messageModel.Sender = _mapper.Map<UserDto>(sender);
            messageModel.Receiver = _mapper.Map<UserDto>(receiver);

            return messageModel;
        }

        /// <summary>
        /// Retrieve all messages from the database where User is either sender or receiver
        /// </summary>
        /// <param name="userId">
        /// The ID of the user
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> of <see cref="MessageDto"/>
        /// </returns>
        public IEnumerable<MessageDto> GetUserMessages(string userId)
        {
            IEnumerable<MessageEntity> messagesFromRepo = _messageRepository.GetUserMessages(userId);

            foreach (MessageEntity messageFromRepo in messagesFromRepo)
            {
                UserEntity? sender = _userRepository.GetByKey(messageFromRepo.SenderId);
                UserEntity? receiver = _userRepository.GetByKey(messageFromRepo.ReceiverId);

                if (sender is null)
                {
                    _logger.LogError(
                        "{date} - The sender User with ID \"{userId}\" related to the Message with ID " +
                        "\"{messageId}\" with  does not exist in the database!",
                        DateTime.Now, messageFromRepo.SenderId, messageFromRepo.Id);
                }
                else if (receiver is null)
                {

                    _logger.LogError(
                        "{date} - The receiver User with ID \"{userId}\" related to the Message with " +
                        "ID \"{messageId}\" with  does not exist in the database!",
                        DateTime.Now, messageFromRepo.ReceiverId, messageFromRepo.Id);
                }
                else
                {
                    MessageDto messageModel = _mapper.Map<MessageDto>(messageFromRepo);

                    messageModel.Sender = _mapper.Map<UserDto>(sender);

                    messageModel.Receiver = _mapper.Map<UserDto>(receiver);

                    yield return messageModel;
                }
            }
        }
    }
}
