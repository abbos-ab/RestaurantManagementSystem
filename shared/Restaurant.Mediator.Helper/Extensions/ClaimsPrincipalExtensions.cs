using Restaurant.Mediator.Helper.Common.Models;
using Restaurant.Mediator.Helper.Exceptions;
using System.Security.Claims;

namespace Restaurant.Mediator.Helper.Extensions;

/// <summary>
/// Расширения для <see cref="ClaimsPrincipal"/>.
/// </summary>
public static class ClaimsPrincipalExtensions
{
    /// <param name="user">Авторизованный пользователь.</param>
    extension(ClaimsPrincipal user)
    {
        /// <summary>
        /// Получает идентификатор авторизованного пользователя.
        /// </summary>
        /// <param name="userId">Идентификатор пользователя при наличии, иначе <c>default</c>.</param>
        /// <returns><see langword="true" /> если удалось получить идентификатор; иначе - <see langword="false" />.</returns>
        public bool TryGetUserId(out long userId)
        {
            userId = 0;
            var nameIdentifier = user.FindFirst(CustomClaimTypes.Id)?.Value;
            return !string.IsNullOrWhiteSpace(nameIdentifier)
                   && long.TryParse(nameIdentifier, out userId);
        }

        /// <summary>
        /// Возвращает идентификатор авторизованного пользователя.
        /// </summary>
        /// <returns>Идентификатор пользователя.</returns>
        /// <exception cref="UnauthorizedException"></exception>
        public long GetUserId()
        {
            if (!user.TryGetUserId(out var userId))
            {
                throw new UnauthorizedException(
                    new Error(
                        "Auth.UserNull",
                        "Не удалось получить идентификатор пользователя."
                    )
                );
            }

            return userId;
        }
    }
}

