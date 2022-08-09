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
    }
}
