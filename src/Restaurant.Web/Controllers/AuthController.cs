using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restaurant.Application.Common.Interfaces;
using Restaurant.Application.Features.Authentications.Commands;
using Restaurant.Application.Features.Authentications.Models;
using Restaurant.Infrastructure.Notifications.Telegram;
using Restaurant.Mediator.Helper.Extensions;
using Restaurant.Web.Models;
using Telegram.Bot;

namespace Restaurant.Web.Controllers;

public class AuthController : BaseController
{
    private readonly ITelegramBotService _telegramBotService;
    public AuthController(IMediator mediator, ITelegramBotService telegramBotService) : base(mediator)
    {
        _telegramBotService = telegramBotService;
    }

    /// <summary>
    /// Получение access + refresh токенов по логину и паролю
    /// </summary>
    /// <remarks>
    /// Стандартная точка входа для получения токенов (аналог /token в OAuth2 Password Grant).
    /// </remarks>
    /// <param name="command">Данные для аутентификации (login(PhoneNumber) + пароль)</param>
    /// <response code="200">Успешная аутентификация, возвращены токены</response>
    /// <response code="400">Некорректный формат запроса</response>
    /// <response code="401">Неверные учетные данные</response>
    /// <response code="403">У пользователя нет доступа</response>
    [HttpPost]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [AllowAnonymous]
    public async Task<AuthenticateResponse> Authenticate([FromBody] AuthenticateCommand command)
    {
        return await _mediator.Send(command);
    }
    
    /// <summary>
    /// Обновление access-токена по refresh-токену
    /// </summary>
    /// <remarks>
    /// Стандартный OAuth2 Refresh Token Grant.
    /// </remarks>
    /// <param name="command">RefreshToken и AccessToken</param>
    /// <response code="200">Выдан новый access-токен (и иногда новый refresh)</response>
    /// <response code="400">Некорректный refresh-токен</response>
    /// <response code="401">Refresh-токен недействителен / отозван / истёк</response>
    [HttpPost("refresh")]
    [ProducesResponseType(typeof(AuthenticateResponse), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    public async Task<AuthenticateResponse> RefreshToken([FromBody] RefreshTokenCommand command)
    {
        return await _mediator.Send(command);
    }
    
    /// <summary>
    /// Отзывает указанный refresh токен.
    /// </summary>
    [Authorize]
    [HttpPost("revoke")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult> Revoke(
        [FromBody] RevokeTokenRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!User.TryGetUserId(out var userId))
            return new BadRequestResult();

        await _mediator.Send(new RevokeTokenCommand(userId, request.RefreshToken), cancellationToken);

        return new NoContentResult();
    }
    
    /// <summary>
    /// Смена пароля по токену
    /// </summary>
    /// <param name="command">Login(PhoneNumber), Token, NewPassword</param>
    /// /// <param name="cancellationToken">Токен отмены</param>
    /// <response code="204">Пароль успешно изменён</response>
    /// <response code="400">Неверный старый пароль / слабый новый пароль</response>
    /// <response code="401">Пользователь не аутентифицирован</response>
    [HttpPut("changePassword")]
    public async Task<NoContentResult> ChangePassword(
        [FromBody] ChangePasswordCommand command,
        CancellationToken cancellationToken = default)
    {
        await _mediator.Send(command, cancellationToken);
        return new NoContentResult();
    }
}