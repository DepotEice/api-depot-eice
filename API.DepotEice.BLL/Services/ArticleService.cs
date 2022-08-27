using API.DepotEice.BLL.Dtos;
using API.DepotEice.BLL.Extensions;
using API.DepotEice.BLL.IServices;
using API.DepotEice.DAL.Entities;
using API.DepotEice.DAL.IRepositories;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace API.DepotEice.BLL.Services;

public class ArticleService : IArticleService
{
    private readonly ILogger _logger;
    private readonly IMapper _mapper;
    private readonly IArticleRepository _articleRepository;
    private readonly IUserRepository _userRepository;
    private readonly IArticleCommentRepository _articleCommentRepository;

        public ArticleService(ILogger<ArticleService> logger, IMapper mapper, IArticleRepository articleRepository,
            IUserRepository userRepository, IArticleCommentRepository articleCommentRepository)
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

        if (userRepository is null)
        {
            throw new ArgumentNullException(nameof(userRepository));
        }

        if (articleCommentRepository is null)
        {
            throw new ArgumentNullException(nameof(articleCommentRepository));
        }

        _logger = logger;
        _mapper = mapper;
        _articleRepository = articleRepository;
        _userRepository = userRepository;
        _articleCommentRepository = articleCommentRepository;
    }

        /// <summary>
        /// Create a new article in the database
        /// </summary>
        /// <param name="article">
        /// An instance of <see cref="ArticleDto"/>. The property <see cref="ArticleDto.User"/>
        /// must be instanciated otherwise mapping from <see cref="ArticleDto"/> to 
        /// <see cref="ArticleEntity"/> won't work
        /// </param>
        /// <returns>
        /// <c>null</c> If the creation failed or if the related User does not exist. Otherwise An 
        /// instance of <see cref="ArticleDto"/> 
        /// </returns>
        /// <exception cref="ArgumentNullException"></exception>
        public ArticleDto? CreateArticle(ArticleDto article)
        {
            if (article is null)
            {
                throw new ArgumentNullException(nameof(article));
            }

            ArticleEntity articleEntity = _mapper.Map<ArticleEntity>(article);

            int newId = _articleRepository.Create(articleEntity);

        if (newId <= 0)
        {
            _logger.LogError(
                "{date} - The creation returned an ID smaller or equals to 0",
                DateTime.Now);

            return null;
        }

        ArticleEntity? createdArticle = _articleRepository.GetByKey(newId);

        if (createdArticle is null)
        {
            _logger.LogWarning(
                "{date} - The retrieved ArticleEntity with ID \"{id}\" is null",
                DateTime.Now, newId);

            return null;
        }

        UserEntity? userFromRepo = _userRepository.GetByKey(createdArticle.UserId);

        if (userFromRepo is null)
        {
            _logger.LogWarning(
                "{date} - The User with ID \"{userID}\" related to the Article with ID " +
                "\"{articleId}\" is null!",
                DateTime.Now, createdArticle.UserId, createdArticle.Id);

            return null;
        }

        IEnumerable<ArticleCommentEntity> articleCommentsFromRepo =
           _articleCommentRepository.GetArticleComments(createdArticle.Id);

            ArticleDto articleModel = _mapper.MergeInto<ArticleDto>(
                createdArticle,
                _mapper.Map<UserDto>(userFromRepo),
                _mapper.Map<IEnumerable<ArticleCommentDto>>(articleCommentsFromRepo));

        return articleModel;
    }

        public bool DeleteArticle(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }

            ArticleEntity? articleEntity = _articleRepository.GetByKey(id);

            if (articleEntity is null)
            {
                _logger.LogWarning(
                    "{date} - There is no Article with ID \"{id}\"!",
                    DateTime.Now, id);

                return false;
            }

            return _articleRepository.Delete(articleEntity);
        }

        /// <summary>
        /// Retrieve an Article from the database
        /// </summary>
        /// <param name="id">
        /// The id of the Article
        /// </param>
        /// <returns>
        /// <c>null</c> If the Article does not exist in the database or if the related User does
        /// not exist either. Otherwise return an instance of <see cref="ArticleDto"/>
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public ArticleDto? GetArticle(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }

            ArticleEntity? articleFromRepo = _articleRepository.GetByKey(id);

            if (articleFromRepo is null)
            {
                _logger.LogWarning(
                    "{date} - There is no article in the database with ID \"{id}\"",
                    DateTime.Now, id);

            return null;
        }

        UserEntity? userFromRepo = _userRepository.GetByKey(articleFromRepo.UserId);

        if (userFromRepo is null)
        {
            _logger.LogError(
                "{date} - There is no User in the database with ID \"{userId}\" for " +
                "Article with ID \"{articleID}\"",
                DateTime.Now, articleFromRepo.UserId, articleFromRepo.Id);

            return null;
        }

        IEnumerable<ArticleCommentEntity> articleCommentsFromRepo =
            _articleCommentRepository.GetArticleComments(articleFromRepo.Id);

        ArticleDto articleModel = _mapper.MergeInto<ArticleDto>(
            articleFromRepo,
            _mapper.Map<UserDto>(userFromRepo),
            _mapper.Map<IEnumerable<ArticleCommentDto>>(articleCommentsFromRepo));

        return articleModel;
    }

        /// <summary>
        /// Retrieve all Articles from the database
        /// </summary>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> of <see cref="ArticleDto"/>
        /// </returns>
        public IEnumerable<ArticleDto> GetArticles()
        {
            IEnumerable<ArticleEntity> articlesFromRepo = _articleRepository.GetAll();

            foreach (ArticleEntity article in articlesFromRepo)
            {
                UserEntity? userFromRepo = _userRepository.GetByKey(article.UserId);

                if (userFromRepo is null)
                {
                    _logger.LogError(
                        "{date} - The user with ID \"{userId}\" related to the article with ID " +
                        "\"{articleId}\" does not exist in the database!",
                        DateTime.Now, article.UserId, article.Id);
                }
                else
                {
                    IEnumerable<ArticleCommentEntity> articleCommentsFromRepo =
                        _articleCommentRepository.GetArticleComments(article.Id);

                    ArticleDto articleModel = _mapper.MergeInto<ArticleDto>(
                        article,
                        _mapper.Map<UserDto>(userFromRepo),
                        _mapper.Map<IEnumerable<ArticleCommentDto>>(articleCommentsFromRepo));

                    yield return articleModel;
                }
            }
        }

    /// <summary>
    /// Change the Article's pinned status
    /// </summary>
    /// <param name="id">
    /// The ID of the Article
    /// </param>
    /// <param name="isPinned">
    /// <c>true</c> To pin the article. <c>false</c> Otherwise
    /// </param>
    /// <returns>
    /// <c>true</c> If the article's pin status succesfully changed. <c>false</c> If 10 or more
    /// articles are already pinned or if the operation failed
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public bool PinArticle(int id, bool isPinned)
    {
        if (id <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(id));
        }

        if (isPinned == true && _articleRepository.GetAll().Count(a => a.Pinned == true) >= 10)
        {
            return false;
        }

        return _articleRepository.PinArticle(id, isPinned);
    }
}
