using Domain.PlantAggregate;
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : DbContext
{  
    public ApplicationDbContext()
    {

    }
    
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Plant> Plants { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
    
    // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    // {
    //     //if (!optionsBuilder.IsConfigured)
    //     {
    //         # warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
    //         //optionsBuilder.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString());
    //         optionsBuilder.UseSqlite($"Data Source=plant.db");
    //     }
    //
    // }
}