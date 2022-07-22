namespace API.DepotEice.DAL.Entities
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
        public string Content { get; set; }

        /// <summary>
        /// ID of <see cref="UserEntity"/> sending the message
        /// </summary>
        public string SenderId { get; set; }

        /// <summary>
        /// ID of <see cref="UserEntity"/> receving the message
        /// </summary>
        public string ReceiverId { get; set; }

        /// <summary>
        /// Instanciate an object <see cref="MessageEntity"/> and initialize all its properties
        /// </summary>
        /// <param name="id">Message's ID</param>
        /// <param name="content">The content of the message</param>
        /// <param name="senderId">The ID of the user sending the message</param>
        /// <param name="receiverId">The ID of the user receiving the message</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public MessageEntity(int id, string content, string senderId, string receiverId)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }

            if (string.IsNullOrEmpty(content))
            {
                throw new ArgumentNullException(nameof(content));
            }

            if (string.IsNullOrEmpty(senderId))
            {
                throw new ArgumentNullException(nameof(senderId));
            }

            if (string.IsNullOrEmpty(receiverId))
            {
                throw new ArgumentNullException(nameof(receiverId));
            }

            Id = id;
            Content = content;
            SenderId = senderId;
            ReceiverId = receiverId;
        }

        /// <summary>
        /// Instanciate an object <see cref="MessageEntity"/> and initialize all its properties
        /// except the <see cref="Id"/>
        /// </summary>
        /// <param name="content">The content of the message</param>
        /// <param name="senderId">The ID of the user sending the message</param>
        /// <param name="receiverId">The ID of the user receiving the message</param>
        /// <exception cref="ArgumentNullException"></exception>
        public MessageEntity(string content, string senderId, string receiverId)
        {
            if (string.IsNullOrEmpty(content))
            {
                throw new ArgumentNullException(nameof(content));
            }

            if (string.IsNullOrEmpty(senderId))
            {
                throw new ArgumentNullException(nameof(senderId));
            }

            if (string.IsNullOrEmpty(receiverId))
            {
                throw new ArgumentNullException(nameof(receiverId));
            }

            Id = 0;
            Content = content;
            SenderId = senderId;
            ReceiverId = receiverId;
        }
    }
}
