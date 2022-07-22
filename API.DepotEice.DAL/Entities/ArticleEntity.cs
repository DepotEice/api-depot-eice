using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DepotEice.DAL.Entities
{
    /// <summary>
    /// Represent the table <c>Articles</c> in the database
    /// </summary>
    public class ArticleEntity
    {
        /// <summary>
        /// <see cref="ArticleEntity"/>'s ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// <see cref="ArticleEntity"/>'s title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// <see cref="ArticleEntity"/>'s body content
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// <see cref="ArticleEntity"/>'s creation date and time (hh:mm)
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// <see cref="ArticleEntity"/>'s update date and time
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Specify if <see cref="ArticleEntity"/> is pinned
        /// </summary>
        public bool Pinned { get; set; }

        /// <summary>
        /// <see cref="UserEntity"/>'s ID
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Instanciate an object <see cref="ArticleEntity"/> by initializing all properties
        /// </summary>
        /// <param name="id">Article's ID</param>
        /// <param name="title">Article's title</param>
        /// <param name="body">Article's body</param>
        /// <param name="createdAt">Article's creation date and time</param>
        /// <param name="updatedAt">Article's update date and time</param>
        /// <param name="pinned">Specify if an article is pinned to home page</param>
        /// <param name="userId">ID of the user who crote the article</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="DateTimeOutOfRangeException"></exception>
        public ArticleEntity(int id, string title, string body, DateTime createdAt, DateTime?
            updatedAt, bool pinned, string userId)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }

            if (string.IsNullOrEmpty(title))
            {
                throw new ArgumentNullException(nameof(title));
            }

            if (string.IsNullOrEmpty(body))
            {
                throw new ArgumentNullException(nameof(body));
            }

            if (createdAt > DateTime.Now)
            {
                throw new ArgumentOutOfRangeException(nameof(createdAt));
            }

            if (updatedAt < createdAt)
            {
                throw new ArgumentOutOfRangeException(nameof(updatedAt));
            }

            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException(nameof(userId));
            }

            Id = id;
            Title = title;
            Body = body;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
            Pinned = pinned;
            UserId = userId;
        }

        /// <summary>
        /// Instanciate an object <see cref="ArticleEntity"/> by initializing all properties except
        /// the ID whose default value is 0
        /// </summary>
        /// <param name="title">Article's title</param>
        /// <param name="body">Article's body</param>
        /// <param name="createdAt">Article's creation date and time</param>
        /// <param name="updatedAt">Article's update date and time</param>
        /// <param name="pinned">Specify if an article is pinned to home page</param>
        /// <param name="userId">ID of the user who crote the article</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="DateTimeOutOfRangeException"></exception>
        public ArticleEntity(string title, string body, DateTime createdAt, DateTime updatedAt,
            bool pinned, string userId)
        {
            if (string.IsNullOrEmpty(title))
            {
                throw new ArgumentNullException(nameof(title));
            }

            if (string.IsNullOrEmpty(body))
            {
                throw new ArgumentNullException(nameof(body));
            }

            if (createdAt > DateTime.Now)
            {
                throw new ArgumentOutOfRangeException(nameof(createdAt));
            }

            if (updatedAt < createdAt)
            {
                throw new ArgumentOutOfRangeException(nameof(updatedAt));
            }

            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException(nameof(userId));
            }

            Id = 0;
            Title = title;
            Body = body;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
            Pinned = pinned;
            UserId = userId;
        }
    }
}
