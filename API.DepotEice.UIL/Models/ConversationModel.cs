namespace API.DepotEice.UIL.Models
{
    /// <summary>
    /// Represents a model class for a conversation between two users.
    /// </summary>
    public class ConversationModel
    {
        /// <summary>
        /// Gets or sets the id of the sender in the conversation.
        /// </summary>
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the full name of the sender in the conversation.
        /// </summary>
        public string UserFullName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the id of the receiver in the conversation.
        /// </summary>
        public string UserWithId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the full name of the receiver in the conversation.
        /// </summary>
        public string UserWithFullName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the id of the profile picture of the receiver in the conversation.
        /// </summary>
        public int? UserWithProfilePictureId { get; set; }

        /// <summary>
        /// Gets or sets a collection of messages exchanged in the conversation.
        /// </summary>
        public IEnumerable<MessageModel> Messages { get; set; } = Enumerable.Empty<MessageModel>();
    }
}
