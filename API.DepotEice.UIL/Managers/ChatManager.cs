using API.DepotEice.DAL.Entities;
using API.DepotEice.DAL.IRepositories;
using API.DepotEice.UIL.Hubs;
using API.DepotEice.UIL.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace API.DepotEice.UIL.Managers
{
    /// <summary>
    /// The chat manager singleton
    /// </summary>
    public class ChatManager
    {
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// The chat hub context
        /// </summary>
        private readonly IHubContext<ChatHub> _hubContext;

        /// <summary>
        /// The user manager
        /// </summary>
        private readonly IUserManager _userManager;

        /// <summary>
        /// The dictionary of connected users in the chat hub with the connection id as key and the user id as value
        /// </summary>
        public Dictionary<string, string> ConnectedUsers { get; private set; } = new Dictionary<string, string>();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="hubContext"></param>
        /// <param name="userManager"></param>
        /// <param name="messageRepository"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public ChatManager(ILogger<ChatManager> logger, IHubContext<ChatHub> hubContext, IUserManager userManager)
        {
            if (logger is null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            if (hubContext is null)
            {
                throw new ArgumentNullException(nameof(hubContext));
            }

            if (userManager is null)
            {
                throw new ArgumentNullException(nameof(userManager));
            }

            _logger = logger;
            _hubContext = hubContext;
            _userManager = userManager;
        }

        /// <summary>
        /// Add a new user in the list of connected users
        /// </summary>
        /// <param name="connectionId">the user's connection id</param>
        /// <param name="userId">The user's id</param>
        /// <returns>The id of the user that logged in the chat</returns>
        public string? AddConnectedUser(string connectionId, string userId)
        {
            if (string.IsNullOrEmpty(connectionId))
            {
                throw new ArgumentNullException(nameof(connectionId));
            }

            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException(nameof(userId));
            }

            ConnectedUsers.Add(connectionId, userId);

            return ConnectedUsers.GetValueOrDefault(connectionId);
        }

        /// <summary>
        /// Get user's all connection ids
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public string[] GetUsers(string userId)
        {
            return ConnectedUsers
                .Where(x => x.Value.ToUpper().Equals(userId.ToUpper()))
                .Select(x => x.Key)
                .ToArray();
        }

        /// <summary>
        /// Remove a user from the connected users list
        /// </summary>
        /// <param name="connectionId">The connection id of the user making the request</param>
        /// <returns>
        /// true If the user was removed. false Otherwise
        /// </returns>
        public bool RemoveUser(string connectionId)
        {
            return ConnectedUsers.Remove(connectionId);
        }
    }
}
