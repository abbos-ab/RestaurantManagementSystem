using MediatR;
using Microsoft.AspNetCore.Mvc;
using Restaurant.Application.Features.Medias.Commands;
using Restaurant.Mediator.Helper.Groups;
using Restaurant.Web.Models;

namespace Restaurant.Web.Controllers;

public class DishMediaController : BaseController
{
    public DishMediaController(IMediator mediator) : base(mediator)
    {
    }

    [HttpPost("{dishId:long}")]
    [GroupAuthorize(GroupNames.Administrators, GroupNames.Chefs)]
    public async Task<IActionResult> UploadPhoto(
        long dishId,
        [FromForm] UploadPhotoRequest request,
        CancellationToken cancellationToken = default)
    {
        await using var stream = request.Photo.OpenReadStream();

        await _mediator.Send(
            new UploadPhotoCommand(
                DishId: dishId,
                Order: request.Order,
                Stream: stream,
                FileLength: request.Photo.Length,
                FileName: request.Photo.FileName,
                FileWidth: request.FileWidth,
                FileHeight: request.FileHeight,
                ContentType: request.Photo.ContentType
            ),
            cancellationToken
        );

        return new NoContentResult();
    }

    [HttpDelete("{mediaId:long}")]
    public async Task<IActionResult> DeletePhoto(long mediaId, CancellationToken cancellationToken = default)
    {
        await _mediator.Send(new DeletePhotoCommand(mediaId), cancellationToken);
        return new NoContentResult();
    }
}