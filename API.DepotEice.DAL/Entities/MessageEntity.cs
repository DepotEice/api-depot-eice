﻿namespace API.DepotEice.DAL.Entities
{
    /// <summary>
    /// Represent the <c>Messages</c> table in the database
    /// </summary>
    public class MessageEntity
    {
        /// <summary>
        /// <see cref="MessageEntity"/>'s ID in the database
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// <see cref="MessageEntity"/>'s content
        /// </summary>
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// ID of <see cref="UserEntity"/> sending the message
        /// </summary>
        public string SenderId { get; set; } = string.Empty;

        /// <summary>
        /// ID of <see cref="UserEntity"/> receving the message
        /// </summary>
        public string ReceiverId { get; set; } = string.Empty;

        /// <summary>
        /// The datetime when the message was sent
        /// </summary>
        public DateTime SentAt { get; set; }

        /// <summary>
        /// Flag to know if the message has been read. Default value is false
        /// </summary>
        public bool Read { get; set; }
    }
}
