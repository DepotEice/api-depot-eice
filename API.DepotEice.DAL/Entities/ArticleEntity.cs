﻿using System;
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
        /// <see cref="ArticleEntity"/>'s main image ID
        /// </summary>
        public int MainImageId { get; set; }

        /// <summary>
        /// <see cref="ArticleEntity"/>'s title
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// <see cref="ArticleEntity"/>'s body content
        /// </summary>
        public string Body { get; set; } = string.Empty;

        /// <summary>
        /// <see cref="ArticleEntity"/>'s creation date and time (hh:mm)
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// <see cref="ArticleEntity"/>'s update date and time
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// <see cref="ArticleEntity"/>'s deleted date and time
        /// </summary>
        public DateTime? DeletedAt { get; set; }

        /// <summary>
        /// Specify if <see cref="ArticleEntity"/> is pinned
        /// </summary>
        public bool IsPinned { get; set; }

        /// <summary>
        /// <see cref="UserEntity"/>'s ID
        /// </summary>
        public string UserId { get; set; } = string.Empty;
    }
}
