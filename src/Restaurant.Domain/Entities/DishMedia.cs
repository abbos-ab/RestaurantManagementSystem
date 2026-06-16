namespace Restaurant.Domain.Entities;

public sealed class DishMedia : BaseEntity
{
    public required MediaType MediaType { get; set; }
    
    public required string Path { get; set; }
    
    public required int SortOrder { get; set; }
    
    public double FileWidth { get; set; }
    
    public double FileHeight { get; set; }
    
    public double FileSize { get; set; }

    public static string GetMediaFullPath(long dishId, string folder, string fileName)
    {
        return $"{folder}/medias/{dishId}_{fileName}";
    }
}

public enum MediaType
{
    Image,
    Video,
}