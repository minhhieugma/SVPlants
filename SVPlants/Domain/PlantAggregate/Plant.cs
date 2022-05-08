namespace Domain.PlantAggregate;

public class Plant
{
    public  Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string ImageUrl { get; set; } = string.Empty;
    
    public  string Location { get; set; }= string.Empty;
    
    public  DateTimeOffset? LastWateredAt { get; set; }
    
    public bool IsWatering { get; set; } = false;
}