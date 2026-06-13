using MediatR;
using Microsoft.AspNetCore.Mvc;
using Restaurant.Application.Features.Medias.Commands;
using Restaurant.Application.Features.Medias.Models;
using Restaurant.Application.Features.Medias.Queries;
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
    [GroupAuthorize(GroupNames.Administrators, GroupNames.Chefs)]
    public async Task<IActionResult> DeletePhoto(long mediaId, CancellationToken cancellationToken = default)
    {
        await _mediator.Send(new DeletePhotoCommand(mediaId), cancellationToken);
        return new NoContentResult();
    }

    [HttpGet("{photoId:long}")]
    public async Task<ActionResult<DishMediaDto>> GetById(long photoId, CancellationToken cancellationToken)
    {
        return Ok(await _mediator.Send(new GetPhotoById(photoId), cancellationToken));
    }

    [HttpGet("Dish/{dishId:long}")]
    public async Task<ActionResult<List<DishMediaDto>>> GetByDishId(long dishId, CancellationToken cancellationToken)
    {
        return Ok(await _mediator.Send(new GetPhotoByDishId(dishId), cancellationToken));
    }

    [HttpPut("{photoId:long}")]
    [GroupAuthorize(GroupNames.Administrators, GroupNames.Chefs)]
    public async Task<IActionResult> Update(
        long photoId,
        UpdatePhotoRequest request,
        CancellationToken cancellationToken)
    {
        await _mediator.Send(
            new UpdatePhotoCommand(
                photoId,
                request.Order,
                request.FileWidth,
                request.FileHeight),
            cancellationToken);

        return NoContent();
    }
}