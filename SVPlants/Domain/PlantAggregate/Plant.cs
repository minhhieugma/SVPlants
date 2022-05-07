namespace Domain.PlantAggregate;

public class Plant
{
    public  Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;
    
    public  string Location { get; set; }= string.Empty;
    
    public  DateTimeOffset? LastWateredAt { get; set; }

    // public PlantStatus Status { get; set; } = PlantStatus.Normal;

    public bool IsWatering { get; set; } = false;
}