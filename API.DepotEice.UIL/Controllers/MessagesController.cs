using API.DepotEice.DAL.Entities;
using API.DepotEice.DAL.IRepositories;
using API.DepotEice.UIL.AuthorizationAttributes;
using API.DepotEice.UIL.Interfaces;
using API.DepotEice.UIL.Models;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static API.DepotEice.UIL.Data.RolesData;

namespace API.DepotEice.UIL.Controllers
{
    /// <summary>
    /// Represents a controller for managing chat messages in the API.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        /// <summary>
        /// Logger for logging messages.
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// Mapper for mapping between data models.
        /// </summary>
        private readonly IMapper _mapper;

        /// <summary>
        /// User manager for logged-in user-related operations.
        /// </summary>
        private readonly IUserManager _userManager;

        /// <summary>
        /// The repository managing messages in the database.
        /// </summary>
        private readonly IMessageRepository _messageRepository;

        /// <summary>
        /// The repository managing users in the database.
        /// </summary>
        private readonly IUserRepository _userRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagesController"/> class.
        /// </summary>
        /// <param name="logger">An instance of a logger for logging messages.</param>
        /// <param name="mapper">An instance of a mapper for mapping data models.</param>
        /// <param name="userManager">An instance of a user manager for user-related operations.</param>
        /// <param name="messageRepository"></param>
        /// <param name="userRepository"></param>
        /// <exception cref="ArgumentNullException">Thrown if any of the provided dependencies is null.</exception>
        public MessagesController(ILogger<MessagesController> logger, IMapper mapper, IUserManager userManager,
            IMessageRepository messageRepository, IUserRepository userRepository)
        {
            // Check if the provided logger parameter is null and throw an exception if it is.
            if (logger is null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            // Check if the provided mapper parameter is null and throw an exception if it is.
            if (mapper is null)
            {
                throw new ArgumentNullException(nameof(mapper));
            }

            // Check if the provided userManager parameter is null and throw an exception if it is.
            if (userManager is null)
            {
                throw new ArgumentNullException(nameof(userManager));
            }

            // Check if the provided messageRepository parameter is null and throw an exception if it is.
            if (messageRepository is null)
            {
                throw new ArgumentNullException(nameof(messageRepository));
            }

            // Check if the provided userRepository parameter is null and throw an exception if it is.
            if (userRepository is null)
            {
                throw new ArgumentNullException(nameof(userRepository));
            }

            _logger = logger;
            _mapper = mapper;
            _userManager = userManager;
            _messageRepository = messageRepository;
            _userRepository = userRepository;
        }

        /// <summary>
        /// Retrieves a list of conversations for the authenticated user.
        /// </summary>
        /// <remarks>
        /// This endpoint returns a list of conversation models containing user information and messages for the authenticated user.
        /// </remarks>
        /// <returns>A list of conversation models containing user information and messages.</returns>
        [HttpGet("conversations/me")]
        [HasRoleAuthorize(RolesEnum.GUEST)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ConversationModel>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(string))]
        public IActionResult GetConversations()
        {
            try
            {
                string? currentUserId = _userManager.GetCurrentUserId;

                if (string.IsNullOrEmpty(currentUserId))
                {
                    return Unauthorized("You are not authorized to perform this action");
                }

                IEnumerable<MessageEntity> messages = _messageRepository.GetUserMessages(currentUserId);

                IEnumerable<UserEntity?> distinctUsers = messages
                    .SelectMany(m => new[] { m.SenderId, m.ReceiverId })
                    .Distinct()
                    .Select(di => _userRepository.GetByKey(di));

                IEnumerable<ConversationModel> conversations = messages
                    .GroupBy(m => m.SenderId == currentUserId ? m.ReceiverId : m.SenderId)
                    .Select(g => new ConversationModel
                    {
                        // Determine the SenderId and ReceiverId based on the grouping key.
                        UserId = currentUserId, // SenderId

                        // Set SenderFullName based on the grouping key.
                        UserFullName = GetFullName(currentUserId, distinctUsers),

                        // Set the receiver id
                        UserWithId = g.Key != currentUserId ? g.Key : currentUserId,

                        // Set the ReceiverFullName based on the grouping key.
                        UserWithFullName = g.Key != currentUserId
                            ? GetFullName(g.Key, distinctUsers)
                            : GetFullName(currentUserId, distinctUsers),

                        UserWithProfilePictureId = g.Key == currentUserId
                            ? GetProfilePictureId(currentUserId, distinctUsers)
                            : GetProfilePictureId(g.Key, distinctUsers),

                        // Map the messages to the MessageModel.
                        Messages = _mapper.Map<IEnumerable<MessageModel>>(
                                messages.Where(m =>
                                            (m.SenderId == currentUserId && m.ReceiverId == g.Key) ||
                                            (m.SenderId == g.Key && m.ReceiverId == currentUserId))
                                        .OrderByDescending(m => m.SentAt)
                            )
                    });

                return Ok(conversations);
            }
            catch (Exception e)
            {
                _logger.LogError(
                     "{date} - An exception was thrown during \"{fnName}\":\n{e.Message}\"\n\"{e.StackTrace}\"",
                     DateTime.Now,
                     nameof(GetConversations),
                     e.Message,
                     e.StackTrace
                 );
#if DEBUG
                return BadRequest(e.Message);
#else
                return BadRequest("An error occurred while trying to get conversations, please contact the administrator");
#endif
            }
        }

        /// <summary>
        /// Marks a conversation as read for the authenticated user.
        /// </summary>
        /// <param name="receiverId">The id of the user receiver</param>
        /// <remarks>
        /// This endpoint marks a conversation as read for the authenticated user with the specified receiver.
        /// </remarks>
        /// <returns>
        /// A boolean value indicating whether the operation was successful (true) or not (false).
        /// </returns>
        [HttpPut("conversation/{receiverId}/read")]
        [HasRoleAuthorize(RolesEnum.GUEST)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(string))]
        public IActionResult MarkAsRead(string receiverId)
        {
            if (string.IsNullOrEmpty(receiverId))
            {
                return BadRequest("You must specify the receiver id");
            }

            try
            {
                string? currentUserId = _userManager.GetCurrentUserId;

                if (string.IsNullOrEmpty(currentUserId))
                {
                    return Unauthorized("You are not authorized to perform this action");
                }

                return Ok(_messageRepository.MarkConversationAsRead(currentUserId, receiverId));
            }
            catch (Exception e)
            {
                _logger.LogError(
                    "{date} - An exception was thrown during \"{fnName}\":\n{e.Message}\"\n\"{e.StackTrace}\"",
                    DateTime.Now,
                    nameof(MarkAsRead),
                    e.Message,
                    e.StackTrace
                );

#if DEBUG
                return BadRequest(e.Message);
#else
                return BadRequest("An error occurred while trying to mark conversation as read, please contact the administrator");
#endif
            }
        }

        /// <summary>
        /// Retrieves the full name of a user based on their unique identifier.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <param name="users">A collection of user entities to search for the user.</param>
        /// <returns>
        /// The full name of the user if found; otherwise, returns "Utilisateur introuvable"
        /// (User not found in French).
        /// </returns>
        private static string GetFullName(string userId, IEnumerable<UserEntity?> users)
        {
            // Attempt to find the user by their unique identifier.
            UserEntity? user = users.SingleOrDefault(u => u is not null && u.Id.Equals(userId));

            // If the user is not found, return a message indicating that the user was not found.
            if (user is null)
            {
                return "Utilisateur introuvable"; // User not found
            }

            // If the user is found, return their full name.
            return $"{user.FirstName} {user.LastName}";
        }

        /// <summary>
        /// Get the profile picture id of the given user in the given list of user entities
        /// </summary>
        /// <param name="userId">
        /// The id of the user
        /// </param>
        /// <param name="users">
        /// The list of user entities
        /// </param>
        /// <returns>
        /// A nullable integer representing the profile picture id of the user. If the user i null or not found, 
        /// returns null
        /// </returns>
        private static int? GetProfilePictureId(string userId, IEnumerable<UserEntity?> users)
        {
            return users.SingleOrDefault(u => u.Id.Equals(userId))?.ProfilePictureId;
        }
    }
}
