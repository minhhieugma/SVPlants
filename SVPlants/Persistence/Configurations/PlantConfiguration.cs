using Domain.PlantAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal class PlantConfiguration :
    IEntityTypeConfiguration<Plant>
{
    public void Configure(EntityTypeBuilder<Plant> builder)
    {
        builder.ToTable("Accounts", "HzApp");

        builder.HasKey(p => p.Id);
            
        builder.Property(p => p.Status).HzHasEnumConversion();
    }
}