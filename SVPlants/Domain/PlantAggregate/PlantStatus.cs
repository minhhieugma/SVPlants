using System.Runtime.Serialization;

namespace Domain.PlantAggregate;

public enum PlantStatus
{ 
    [EnumMember(Value = "Normal")]
    Normal,
    [EnumMember(Value = "NeededWater")]
    NeededWater,
    [EnumMember(Value = "Watering")]
    Watering,
    [EnumMember(Value = "Resting")]
    Resting,
}