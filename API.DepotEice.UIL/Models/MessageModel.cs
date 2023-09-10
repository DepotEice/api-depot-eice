using API.DepotEice.DAL.Entities;

namespace API.DepotEice.UIL.Models
{
    /// <summary>
    /// Represents a model class for chat messages.
    /// </summary>
    public class MessageModel
    {
        /// <summary>
        /// Gets or sets the unique identifier of the message.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the content of the message.
        /// </summary>
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the unique identifier of the sender of the message.
        /// </summary>
        public string SenderId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the unique identifier of the receiver of the message.
        /// </summary>
        public string ReceiverId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the timestamp when the message was sent.
        /// </summary>
        public DateTime SentAt { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the message has been read by the receiver.
        /// </summary>
        public bool Read { get; set; }
    }
}
