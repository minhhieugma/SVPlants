using System.Runtime.Serialization;

namespace Domain.PlantAggregate;

public enum PlantStatus
{ 
    [EnumMember(Value = "Normal")]
    Normal,   
    [EnumMember(Value = "Watering")]
    Watering,
    [EnumMember(Value = "NeededWater")]
    NeededWater,
    [EnumMember(Value = "Resting")]
    Resting,
}