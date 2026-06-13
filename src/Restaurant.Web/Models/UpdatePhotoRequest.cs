namespace Restaurant.Web.Models;

public sealed record UpdatePhotoRequest(
    int Order,
    double FileWidth,
    double FileHeight
);