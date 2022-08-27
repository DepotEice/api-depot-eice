using API.DepotEice.BLL.Dtos;
using API.DepotEice.BLL.IServices;
using API.DepotEice.DAL.Entities;
using API.DepotEice.DAL.IRepositories;
using AutoMapper;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Logging;
using System.Text;

namespace API.DepotEice.BLL.Services;

public class AuthService : IAuthService
{
	private readonly ILogger<AuthService> _logger;
	private readonly IMapper _mapper;
	private readonly IAuthRepository _repository;

	public AuthService(
		ILogger<AuthService> logger,
		IMapper mapper,
		IAuthRepository repository)
	{
		_logger = logger;
		_mapper = mapper;
		_repository = repository;
	}

	public UserDto? SignIn(string email, string password, string salt)
	{
		// - récupérer le hash de l'utilisateur depuis la db à partir d'email,
		string hash = GetHashedPasswordFromEmail(email);

		if (string.IsNullOrEmpty(hash) || string.IsNullOrWhiteSpace(hash))
			throw new ArgumentNullException("Something went wrong, plase try again or contact the administration.");

		// - hasher le mot de passe,
		string hashedPwd = GenerateHMACSHA512(password, Encoding.UTF8.GetBytes(salt));

		// - comparer les mot de passe hashés.
		bool result = string.Equals(hash, hashedPwd);

		if (!result)
			return null;

		UserEntity? entity = _repository.SignIn(email, hashedPwd);

        return _mapper.Map<UserDto>(entity);
	}

	public bool SingUp(UserDto dto)
	{
		throw new NotImplementedException();
	}

	private string GetHashedPasswordFromEmail(string email)
	{

		return "vE8wUI9KYCiP3uQx+IAFQPnEBE132o6by9q2BKfdETvK4MTEQS1gScDn7ppCZIc7PkXsd/utztvZj0bu5wDGZg==";
	}

	// TODO - Add this feat in devhoptools
	private static string GenerateHMACSHA512(string input, byte[] salt)
	{
		return Convert.ToBase64String(KeyDerivation.Pbkdf2(input, salt, KeyDerivationPrf.HMACSHA512, 100000, 512 / 8));
	}
}
