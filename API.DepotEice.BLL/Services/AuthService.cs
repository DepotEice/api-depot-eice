using API.DepotEice.BLL.Dtos;
using API.DepotEice.BLL.IServices;
using API.DepotEice.BLL.Mappers;
using API.DepotEice.DAL.Entities;
using API.DepotEice.DAL.IRepositories;
using API.DepotEice.Helpers.Tools;
using AutoMapper;
using Microsoft.Extensions.Logging;
using System.Text;

namespace API.DepotEice.BLL.Services;

public class AuthService : IAuthService
{
	private readonly ILogger<AuthService> _logger;
	private readonly IMapper _mapper;
	private readonly IUserRepository _userRepository;
	private readonly IUserTokenRepository _userTokenRepository;

	public AuthService(
		ILogger<AuthService> logger,
		IMapper mapper,
		IUserRepository userRepository,
		IUserTokenRepository userTokenRepository)
	{
		_logger = logger;
		_mapper = mapper;
		_userRepository = userRepository;
		_userTokenRepository = userTokenRepository;
	}

	public UserDto? SignIn(string email, string password, string salt)
	{
		// - récupérer le hash de l'utilisateur depuis la db à partir d'email,
		string? hash = _userRepository.GetHashPwdFromEmail(email);

		if (string.IsNullOrEmpty(hash) || string.IsNullOrWhiteSpace(hash))
		{
			_logger.LogWarning(
				"{date} - The hash is is null or empty.",
				DateTime.Now);

			throw new ArgumentNullException(nameof(hash));
		}

		// - hasher le mot de passe,
		string hashedPwd = password.GenerateHMACSHA512(Encoding.UTF8.GetBytes(salt));

		// - comparer les mot de passe hashés.
		bool result = string.Equals(hash, hashedPwd);

		if (!result)
			return null;

		UserDto? dto = _userRepository.GetUserByEmail(email)?.ToBll();

		return _mapper.Map<UserDto>(dto);
	}

	public bool SingUp(UserDto dto, string salt)
	{
		if (dto == null)
		{
			_logger.LogWarning(
				"{date} - The userDto is is null.",
				DateTime.Now);

			throw new ArgumentNullException(nameof(dto));
		}

		bool emailExists = _userRepository.GetAll().Any(u => u.NormalizedEmail.Equals(dto.Email.ToUpper()));

		if (emailExists)
		{
			_logger.LogWarning(
				"{date} - The user with this email already exists.",
				DateTime.Now);
			return false;
		}

		string hash = dto.Password.GenerateHMACSHA512(Encoding.UTF8.GetBytes(salt));

		UserEntity entity = new UserEntity()
		{
			Id = dto.Id,
			Email = dto.Email,
			NormalizedEmail = dto.Email.ToUpper(),
			EmailConfirmed = dto.EmailConfirmed,
			PasswordHash = hash,
			FirstName = dto.FirstName,
			LastName = dto.LastName,
			BirthDate = dto.BirthDate,
			ProfilePicture = dto.ProfilePicture,
			IsActive = dto.IsActive,
			ConcurrencyStamp = dto.ConcurrencyStamp,
			SecurityStamp = dto.SecurityStamp,
			CreatedAt = dto.CreatedAt,
			UpdatedAt = dto.UpdatedAt,
			DeletedAt = dto.DeletedAt
		};

		string newId = _userRepository.Create(entity);

		if (string.IsNullOrEmpty(newId) || string.IsNullOrWhiteSpace(newId))
		{
			_logger.LogWarning(
				"{date} - The User could not be created!",
				DateTime.Now);
			return false;
		}

		UserEntity? userFromRepo = _userRepository.GetByKey(newId);

		if (userFromRepo is null)
		{
			_logger.LogError(
				"{date} - The retrieval of the newly created User with ID \"{id}\" " +
				"returned null!",
				DateTime.Now, newId);

			return false;
		}

		if (!newId.Equals(userFromRepo.Id))
		{
			_logger.LogError(
				"{date} - the ID's do not match {newId} != {userId}",
				DateTime.Now, newId, userFromRepo.Id);

			return false;
		}

		string createdUserTokenID = _userTokenRepository.Create(new UserTokenEntity()
		{
			Type = UserTokenTypes.EMAIL_CONFIRMATION_TOKEN,
			ExpirationDateTime = DateTime.Now.AddDays(2),
			UserId = userFromRepo.Id,
			UserSecurityStamp = userFromRepo.SecurityStamp
		});

		if (string.IsNullOrEmpty(createdUserTokenID))
		{
			_logger.LogWarning(
				"{date} - The UserToken creation failed!",
				DateTime.Now);

			return false;
		}

		return true;
	}
}
