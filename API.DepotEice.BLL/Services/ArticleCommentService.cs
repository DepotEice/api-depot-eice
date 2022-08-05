using API.DepotEice.BLL.Extensions;
using API.DepotEice.BLL.IServices;
using API.DepotEice.BLL.Models;
using API.DepotEice.DAL.Entities;
using API.DepotEice.DAL.IRepositories;
using AutoMapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DepotEice.BLL.Services
{
    public class ArticleCommentService : IArticleCommentService
    {
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly IArticleRepository _articleRepository;
        private readonly IArticleCommentRepository _articleCommentRepository;
        private readonly IUserRepository _userRepository;

        public ArticleCommentService(ILogger<ArticleCommentService> logger, IMapper mapper,
            IArticleRepository articleRepository, IArticleCommentRepository articleCommentRepository,
            IUserRepository userRepository)
        {
            if (logger is null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            if (mapper is null)
            {
                throw new ArgumentNullException(nameof(mapper));
            }

            if (articleRepository is null)
            {
                throw new ArgumentNullException(nameof(articleRepository));
            }

            if (articleCommentRepository is null)
            {
                throw new ArgumentNullException(nameof(articleCommentRepository));
            }

            if (userRepository is null)
            {
                throw new ArgumentNullException(nameof(userRepository));
            }

            _logger = logger;
            _mapper = mapper;
            _articleRepository = articleRepository;
            _articleCommentRepository = articleCommentRepository;
            _userRepository = userRepository;
        }

        /// <summary>
        /// Create a new ArticleComment in the database
        /// </summary>
        /// <param name="model">
        /// An instance of <see cref="ArticleCommentModel"/>
        /// </param>
        /// <returns>
        /// <c>null</c> If the article comment couldn't be created or if the linked Article does not
        /// exist in the database. An instance of <see cref="ArticleCommentModel"/> otherwise.
        /// </returns>
        /// <exception cref="ArgumentNullException"></exception>
        public ArticleCommentModel? CreateArticleComment(ArticleCommentModel model)
        {
            if (model is null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            ArticleCommentEntity articleCommentEntity = _mapper.Map<ArticleCommentEntity>(model);

            int newId = _articleCommentRepository.Create(articleCommentEntity);

            ArticleCommentEntity? articleCommentFromRepo = _articleCommentRepository.GetByKey(newId);

            if (articleCommentFromRepo is null)
            {
                _logger.LogError("{date} - The model couldn't be created. Returned ID is 0",
                    DateTime.Now);

                return null;
            }

            ArticleEntity? articleEntity = _articleRepository.GetByKey(articleCommentEntity.ArticleId);

            if (articleEntity is null)
            {
                _logger.LogError("{date} - The linked Article with ID \"{articleId}\" does " +
                    "not exist in the database", DateTime.Now, articleCommentFromRepo.ArticleId);

                return null;
            }

            UserEntity? userEntity = _userRepository.GetByKey(articleCommentFromRepo.UserId);

            if (userEntity is null)
            {
                _logger.LogWarning(
                    "{date} - The User with ID \"{userId}\" linked to the ArticleComment with ID " +
                    "\"{articleCommentId}\" does not exist in the database!",
                    DateTime.Now, articleCommentFromRepo.UserId, articleCommentFromRepo.Id);

                return null;
            }

            ArticleCommentModel articleComment =
                _mapper.MergeInto<ArticleCommentModel>(
                    articleCommentFromRepo,
                    _mapper.Map<ArticleModel>(articleEntity),
                    _mapper.Map<ArticleModel>(userEntity));

            return articleComment;
        }

        /// <summary>
        /// Delete an ArticleComment from the database
        /// </summary>
        /// <param name="id">
        /// The ArticleComment's ID
        /// </param>
        /// <returns>
        /// <c>true</c> If it was succesfully deleted. <c>false</c> If the linked Article does not
        /// exist or if the removal failed
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public bool DeleteArticleComment(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }

            ArticleCommentEntity? articleCommentFromRepo = _articleCommentRepository.GetByKey(id);

            if (articleCommentFromRepo is null)
            {
                _logger.LogError("{date} - There is no article comment in the database " +
                    "with ID \"{id}\"", DateTime.Now, id);

                return false;
            }

            return _articleCommentRepository.Delete(articleCommentFromRepo);
        }

        /// <summary>
        /// Retrieve all ArticleComments related to an Article defined the
        /// <paramref name="articleId"/> parameter
        /// </summary>
        /// <param name="articleId">
        /// Related Article's primary key
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> of <see cref="ArticleCommentModel"/>. If an
        /// ArticleComment's related Article does not exist in the database, the element is skipped
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public IEnumerable<ArticleCommentModel> GetAllArticleComments(int articleId)
        {
            if (articleId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(articleId));
            }

            IEnumerable<ArticleCommentEntity> articleCommentsFromRepo =
                _articleCommentRepository.GetArticleComments(articleId);

            foreach (ArticleCommentEntity articleComment in articleCommentsFromRepo)
            {
                ArticleEntity? articleFromRepo =
                    _articleRepository.GetByKey(articleComment.ArticleId);

                UserEntity? userFromRepo = _userRepository.GetByKey(articleComment.UserId);

                if (articleFromRepo is null)
                {
                    _logger.LogError(
                        "{date} - The linked Article with ID \"{articleId}\" for ArticleComment " +
                        "with ID \"{articleCommentId}\" does not exist in the database",
                        DateTime.Now, articleComment.ArticleId, articleComment.Id);
                }
                else if (userFromRepo is null)
                {
                    _logger.LogWarning(
                           "{date} - The User with ID \"{userId}\" linked to the ArticleComment with ID " +
                           "\"{articleCommentId}\" does not exist in the database!",
                           DateTime.Now, articleComment.UserId, articleComment.Id);
                }
                else
                {
                    ArticleCommentModel articleCommentModel =
                        _mapper.MergeInto<ArticleCommentModel>(
                            articleComment,
                            _mapper.Map<ArticleModel>(articleFromRepo),
                            _mapper.Map<ArticleModel>(userFromRepo));

                    yield return articleCommentModel;
                }
            }
        }

        /// <summary>
        /// Retrieve an ArticleComment from the database
        /// </summary>
        /// <param name="id">
        /// ArticleComment's ID
        /// </param>
        /// <returns>
        /// <c>null</c> If the ArticleComment does not exist or if the ArticleComment's related
        /// Article does not exist. An instance of <see cref="ArticleCommentModel"/> otherwise
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public ArticleCommentModel? GetArticleComment(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }

            ArticleCommentEntity? articleCommentFromRepo = _articleCommentRepository.GetByKey(id);

            if (articleCommentFromRepo is null)
            {
                _logger.LogWarning(
                    "{date} - The ArticleComment with ID \"{id}\" does not exist in " +
                    "the database",
                    DateTime.Now, id);

                return null;
            }

            ArticleEntity? articleEntity =
                _articleRepository.GetByKey(articleCommentFromRepo.ArticleId);

            if (articleEntity is null)
            {
                _logger.LogWarning(
                    "{date} - The Article with ID \"{articleId}\" related to the " +
                    "ArticleComment with Id \"{articleCommentId}\" does not exist in the database!",
                    DateTime.Now, articleCommentFromRepo.ArticleId, articleCommentFromRepo.Id);

                return null;
            }

            UserEntity? userEntity = _userRepository.GetByKey(articleCommentFromRepo.UserId);

            if (userEntity is null)
            {
                _logger.LogWarning(
                    "{date} - The User with ID \"{userId}\" linked to the ArticleComment with ID " +
                    "\"{articleCommentId}\" does not exist in the database!",
                    DateTime.Now, articleCommentFromRepo.UserId, articleCommentFromRepo.Id);

                return null;
            }

            ArticleCommentModel articleComment =
                _mapper.MergeInto<ArticleCommentModel>(
                    articleCommentFromRepo,
                    _mapper.Map<ArticleModel>(articleEntity),
                    _mapper.Map<ArticleModel>(userEntity));

            return articleComment;
        }

        /// <summary>
        /// Update the ArticleComment in the database
        /// </summary>
        /// <param name="model">
        /// The instance of <see cref="ArticleCommentModel"/> to update
        /// </param>
        /// <returns>
        /// <c>null</c> If the update failed or if the related Article is does not exist in the
        /// database. An instance of <see cref="ArticleCommentModel"/> otherwise
        /// </returns>
        /// <exception cref="ArgumentNullException"></exception>
        public ArticleCommentModel? UpdateArticleComment(ArticleCommentModel model)
        {
            if (model is null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (!_articleCommentRepository.Update(_mapper.Map<ArticleCommentEntity>(model)))
            {
                _logger.LogWarning(
                    "{date} - Could not update the ArticleComment with ID \"{id}\"",
                    DateTime.Now, model.Id);

                return null;
            }

            ArticleCommentEntity? articleCommentEntity =
                _articleCommentRepository.GetByKey(model.Id);

            if (articleCommentEntity is null)
            {
                _logger.LogError(
                    "{date} - The updated and retrieved ArticleComment with ID \"{id}\" " +
                    "is null",
                    DateTime.Now, model.Id);

                return null;
            }

            ArticleEntity? articleEntity =
                _articleRepository.GetByKey(articleCommentEntity.ArticleId);

            if (articleEntity is null)
            {
                _logger.LogWarning(
                    "{date} - The Article with ID \"{articleId}\" related to the " +
                    "ArticleComment with ID \"{articleCommentId}\" does not exist in the database",
                    DateTime.Now, articleCommentEntity.ArticleId, articleCommentEntity.Id);

                return null;
            }

            UserEntity? userEntity = _userRepository.GetByKey(articleCommentEntity.UserId);

            if (userEntity is null)
            {
                _logger.LogWarning(
                    "{date} - The User with ID \"{userId}\" linked to the ArticleComment with ID " +
                    "\"{articleCommentId}\" does not exist in the database!",
                    DateTime.Now, articleCommentEntity.UserId, articleCommentEntity.Id);

                return null;
            }

            ArticleCommentModel articleComment =
                _mapper.MergeInto<ArticleCommentModel>(
                    articleCommentEntity,
                    _mapper.Map<ArticleModel>(articleEntity),
                    _mapper.Map<ArticleModel>(userEntity));

            return articleComment;
        }
    }
}
