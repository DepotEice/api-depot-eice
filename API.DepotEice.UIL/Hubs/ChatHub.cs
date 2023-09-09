using API.DepotEice.DAL.Entities;
using API.DepotEice.DAL.IRepositories;
using API.DepotEice.UIL.AuthorizationAttributes;
using API.DepotEice.UIL.Interfaces;
using API.DepotEice.UIL.Managers;
using API.DepotEice.UIL.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using static API.DepotEice.UIL.Data.RolesData;

namespace API.DepotEice.UIL.Hubs
{
    /// <summary>
    /// The chat hub for the application
    /// </summary>
    [HasRoleAuthorize(RolesEnum.GUEST)]
    public class ChatHub : Hub
    {
        private readonly ILogger<ChatHub> _logger;
        private readonly ChatManager _chatManager;
        private readonly IMessageRepository _messageRepository;
        private readonly IUserManager _userManager;
        private readonly IMapper _mapper;

        public ChatHub(ILogger<ChatHub> logger, ChatManager chatManager, IMessageRepository messageRepository,
            IUserManager userManager, IMapper mapper)
        {
            if (logger is null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            if (chatManager is null)
            {
                throw new ArgumentNullException(nameof(chatManager));
            }

            if (messageRepository is null)
            {
                throw new ArgumentNullException(nameof(messageRepository));
            }

            if (userManager is null)
            {
                throw new ArgumentNullException(nameof(userManager));
            }

            if (mapper is null)
            {
                throw new ArgumentNullException(nameof(mapper));
            }

            _logger = logger;
            _chatManager = chatManager;
            _messageRepository = messageRepository;
            _userManager = userManager;
            _mapper = mapper;
        }

        /// <inheritdoc/>
        public override async Task OnConnectedAsync()
        {
            string? currentUserId = _userManager.GetCurrentUserId;

            if (string.IsNullOrWhiteSpace(currentUserId))
            {
                _logger.LogError(
                    "Could not get the current user id when user with connection id \"{connectionId}\" connected to the chat hub",
                    Context.ConnectionId
                );

                await Clients.Caller.SendAsync(
                    "sendMessageError",
                    "You need to be authenticated to connect to the chat"
                );

                return;
            }

            string? userId = _chatManager.AddConnectedUser(Context.ConnectionId, currentUserId);

            if (string.IsNullOrEmpty(userId))
            {
                await Clients.Caller.SendAsync(
                    "sendMessageError",
                    "You need to be authenticated to connect to the chat"
                );

                return;
            }

            await Clients.All.SendAsync("newUserConnected", currentUserId);
        }

        /// <inheritdoc/>
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            if (exception is not null)
            {
                _logger.LogError(
                    "An exception was throw when connected user \"{connectionId}\" was disconnected from the chat hub.\n" +
                    "{e.msg}\n{e.str}",
                    Context.ConnectionId,
                    exception.Message,
                    exception.StackTrace
                );
            }

            if (!_chatManager.RemoveUser(Context.ConnectionId))
            {
                _logger.LogError(
                    "Could not disconnect user with connection id \"{connectionId}\" from the chat hub",
                    Context.ConnectionId
                );

                await Clients.Caller.SendAsync(
                    "sendMessageError",
                    "You need to provide the user and the message"
                );

                return;
            }

            _logger.LogInformation(
                "User with connection id \"{id}\" disconnected successfully",
                Context.ConnectionId
            );
        }

        /// <summary>
        /// Send a message to a user
        /// </summary>
        /// <param name="userId">
        /// The id of the user to whom the message is sent
        /// </param>
        /// <param name="message">
        /// The content of the message
        /// </param>
        /// <returns></returns>
        [HubMethodName("SendMessage")]
        public async Task SendMessageAsync(string userId, string message)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(message))
            {
                await Clients.Caller.SendAsync(
                    "sendMessageError",
                    "You need to provide the user and the message"
                );

                return;
            }

            MessageModel? sentMessage = SaveMessage(Context.ConnectionId, userId, message);

            if (sentMessage is null)
            {
                _logger.LogError(
                    "Could not save the message \"{message}\" sent to \"{id}\" by \"{sender}\"",
                    message,
                    userId,
                    Context.ConnectionId
                );

                await Clients.Caller.SendAsync(
                    "sendMessageError",
                    "An error occurred. Could not save the sent message"
                );

                return;
            }

            string[] connectedUsers = _chatManager.GetUsers(userId);

            await Clients.Clients(connectedUsers).SendAsync("receiveMessage", sentMessage);
        }

        /// <summary>
        /// Save a message to the database
        /// </summary>
        /// <param name="requesterConnectionId">The connection id of the user requesting</param>
        /// <param name="userId">The user id of the user receiving the message</param>
        /// <param name="message">The content of the message</param>
        /// <returns>
        /// true If the message was properly saved to the database
        /// </returns>
        /// <exception cref="ArgumentNullException"></exception>
        private MessageModel? SaveMessage(string requesterConnectionId, string userId, string message)
        {
            if (string.IsNullOrEmpty(requesterConnectionId))
            {
                throw new ArgumentNullException(nameof(requesterConnectionId));
            }

            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException(nameof(userId));
            }

            if (string.IsNullOrEmpty(message))
            {
                throw new ArgumentNullException(nameof(message));
            }

            string? senderId = Context?.User?.Claims.SingleOrDefault(c => c.Type.Equals(ClaimTypes.Sid))?.Value;

            if (string.IsNullOrEmpty(senderId))
            {
                _logger.LogError("Could not retrieve the logged in user id");

                return null;
            }

            int messageId = _messageRepository.Create(new MessageEntity()
            {
                Content = message,
                SenderId = senderId,
                ReceiverId = userId
            });

            if (messageId <= 0)
            {
                return null;
            }

            MessageEntity? messageFromRepo = _messageRepository.GetByKey(messageId);

            if (messageFromRepo is null)
            {
                return null;
            }

            return _mapper.Map<MessageModel>(messageFromRepo);
        }
    }
}
