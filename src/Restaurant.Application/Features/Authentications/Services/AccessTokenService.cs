using Auth.Application.UsersGroups.Specifications;
using Microsoft.IdentityModel.Tokens;
using Restaurant.Application.Features.Authentications.Interfaces;
using Restaurant.Domain.Entities;
using Restaurant.Mediator.Helper;
using Restaurant.Mediator.Helper.Common.Extensions;
using Restaurant.Mediator.Helper.Settings;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Restaurant.Application.Features.UsersGroups.Repositories;

namespace Restaurant.Application.Features.Authentications.Services
{
    internal sealed class AccessTokenService : IAccessTokenService
    {
        private readonly IUserGroupRepository _userGroupRepository;
        private readonly TimeProvider _timeProvider;
        private readonly JwtSettings _jwtSettings;

        public AccessTokenService(
            IUserGroupRepository userGroupRepository,
            TimeProvider timeProvider,
            JwtSettings jwtSettings)
        {
            _userGroupRepository = userGroupRepository;
            _timeProvider = timeProvider;
            _jwtSettings = jwtSettings;
        }

        public async Task<(DateTime expiresAt, string accessToken)> CreateToken(User user)
        {
            ArgumentNullException.ThrowIfNull(user);

            var @params = new UserGroupByUserIdParams
            {
                UserId = user.Id,
                AsNoTracking = true,
                IncludeGroup = true,
            };


            var spec = new UserGroupByUserIdSpec(@params);
            var usersGroups = await _userGroupRepository.ListAsync(spec);

            var claims = new List<Claim>
        {
            new(CustomClaimTypes.Id, user.Id.ToString()),
            new(CustomClaimTypes.ContactInformation, user.PhoneNumber.ToString()),
            new(CustomClaimTypes.FullName, user.FirstName),
            new(CustomClaimTypes.Groups, string.Join(",", usersGroups.Select(x => x.Group.Name))),
        };

            var credentials = new SigningCredentials(
            _jwtSettings.GetSignInKey(),
            SecurityAlgorithms.HmacSha512
        );

            var now = _timeProvider.GetLocalDateTimeNowKindUtc();

            var lifeTime = _jwtSettings.AccessTokenLifeTime;

            var expiresAt = now.Add(lifeTime);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expiresAt,
                SigningCredentials = credentials,
                Issuer = _jwtSettings.Issuer,
                IssuedAt = now,
                NotBefore = now,
            };


            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return (expiresAt, tokenHandler.WriteToken(token));

        }

        public ClaimsPrincipal GetClaimsFromExpiredToken(string accessToken)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(accessToken);

            var tokenValidationParams = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = true,
                ValidIssuer = _jwtSettings.Issuer,
                IssuerSigningKey = _jwtSettings.GetSignInKey(),
                ValidateLifetime = false,
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(accessToken, tokenValidationParams, out _);

            return principal;
        }
    }
}
