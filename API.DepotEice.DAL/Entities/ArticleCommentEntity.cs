using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DepotEice.DAL.Entities
{
    public class ArticleCommentEntity
    {
        /// <summary>
        /// Article's comment's ID 
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Article's given note by the user
        /// </summary>
        public int Note { get; set; }

        /// <summary>
        /// User's review text
        /// </summary>
        public string Review { get; set; }

        /// <summary>
        /// Date and time at which this article's comment was created in the database
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Date and time at which this article's comment was updated in the database.
        /// If the article comment has been created and never update, <see cref="UpdatedAt"/> will
        /// be the same as <see cref="CreatedAt"/>
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Article's ID on which this comment is written
        /// </summary>
        public int ArticleId { get; set; }

        /// <summary>
        /// ID of the user writing the comment
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Instanciate an object <see cref="ArticleCommentEntity"/> with each property initalized
        /// </summary>
        /// <param name="id">ArticleComment's ID from the database</param>
        /// <param name="note">The given note by the user. Must be between 0 and 5 included</param>
        /// <param name="review">The review text</param>
        /// <param name="createdAt">The creation date and time</param>
        /// <param name="updatedAt">The update date and time</param>
        /// <param name="articleId">The related article's ID</param>
        /// <param name="userId">The ID of the user who wrote this comment</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public ArticleCommentEntity(int id, int note, string review, DateTime createdAt,
            DateTime? updatedAt, int articleId, string userId)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }

            if (note < 0 || note > 5)
            {
                throw new ArgumentOutOfRangeException(nameof(note));
            }

            if (string.IsNullOrEmpty(review))
            {
                throw new ArgumentNullException(nameof(review));
            }

            if (updatedAt is not null)
            {
                if (updatedAt <= createdAt)
                {
                    throw new ArgumentOutOfRangeException(nameof(updatedAt));
                }
            }

            if (articleId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(articleId));
            }

            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException(nameof(userId));
            }

            Id = id;
            Note = note;
            Review = review;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
            ArticleId = articleId;
            UserId = userId;
        }

        /// <summary>
        /// Instanciate an object <see cref="ArticleCommentEntity"/> without initializing the ID
        /// </summary>
        /// <param name="note">The given note by the user. Must be between 0 and 5 included</param>
        /// <param name="review">The review text</param>
        /// <param name="articleId">The related article's ID</param>
        /// <param name="userId">The ID of the user who wrote this comment</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public ArticleCommentEntity(int note, string review, int articleId, string userId)
        {
            if (note < 0 || note > 5)
            {
                throw new ArgumentOutOfRangeException(nameof(note));
            }

            if (string.IsNullOrEmpty(review))
            {
                throw new ArgumentNullException(nameof(review));
            }

            if (articleId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(articleId));
            }

            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException(nameof(userId));
            }

            Id = 0;
            Note = note;
            Review = review;
            ArticleId = articleId;
            UserId = userId;
        }
    }
}
