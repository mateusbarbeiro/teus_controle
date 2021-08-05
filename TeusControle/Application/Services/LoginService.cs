using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using TeusControle.Application.Interfaces.Services;
using TeusControle.Domain.Models;
using TeusControle.Domain.Models.Dtos;
using TeusControle.Infrastructure.Dtos;
using TeusControle.Infrastructure.Query;

namespace TeusControle.Application.Services
{
    public class LoginService : ILoginService
    {
        private IConfiguration _config;
        private IUsersService _usersService;
        public LoginService(
            IConfiguration configuration,
            IUsersService usersService
        )
        {
            _config = configuration;
            _usersService = usersService;
        }

        /// <summary>
        /// Gera token de acesso
        /// </summary>
        /// <param name="credential"></param>
        /// <returns></returns>
        public ResponseMessages<string> GenerateToken(TokenLogin credential)
        {
            var validUser = ValidateUser(credential);
            if (!validUser.Sucess)
                return new ResponseMessages<string>(
                    status: false,
                    message: "Usuário não encontrado!"
                );

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(Settings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(CustomClaimTypes.Id, validUser.Data.Id.ToString()),
                    new Claim(CustomClaimTypes.Name, validUser.Data.Name.ToString()),
                    new Claim(CustomClaimTypes.Email, validUser.Data.Email.ToString()),
                    new Claim(CustomClaimTypes.ProfileImage, validUser.Data.ProfileImage != null ? validUser.Data.ProfileImage.ToString() : ""),
                    new Claim(CustomClaimTypes.ProfileTypeId, validUser.Data.ProfileType.ToString()),
                    new Claim(CustomClaimTypes.ProfileTypeName, validUser.Data.ProfileType.ToString()),// alterar para descrição do enumerador
                    new Claim(CustomClaimTypes.UserName,validUser.Data.UserName.ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return new ResponseMessages<string>(
                status: true,
                message: "Login efetuado com sucesso!",
                data: tokenHandler.WriteToken(token)
            );
        }

        /// <summary>
        /// Usuário existe? Se sim, retorna usuário
        /// </summary>
        /// <param name="credential"></param>
        /// <returns></returns>
        private ReturnData<UserDataToken> ValidateUser(TokenLogin credential)
        {
            var filter = new FilterBy<Users>(x =>
                x.UserName == credential.UserName &&
                x.Password == credential.Password &&
                !x.Deleted &&
                x.Active
            );

            if (!_usersService.Any(filter))
                return new ReturnData<UserDataToken>(sucess: false);

            var user = _usersService.QueryDb(filter)
                .Select(s => new UserDataToken
                {
                    Id = s.Id,
                    Name = s.Name,
                    UserName = s.UserName,
                    Email = s.Email,
                    ProfileImage = s.ProfileImage,
                    ProfileType = s.ProfileType
                })
                .FirstOrDefault();

            return new ReturnData<UserDataToken>(
                sucess: true,
                data: user
            );
        }
    }

    public class UserDataToken
    {
        /// <summary>
        /// Identificador
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Nome
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Nome de usuário
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// E-mail
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Imagem de perfil
        /// </summary>
        public string ProfileImage { get; set; }

        /// <summary>
        /// Tipo do perfil
        /// </summary>
        public int ProfileType { get; set; }
    }

    public static class CustomClaimTypes
    {
        public const string Id = "id";

        public const string Name = "name";

        public const string Email = "email";
        
        public const string ProfileImage = "profileimage"; 
        
        public const string ProfileTypeId = "profiletypeid";
        
        public const string ProfileTypeName = "profiletypename"; 
        
        public const string UserName = "username";
    }
}
