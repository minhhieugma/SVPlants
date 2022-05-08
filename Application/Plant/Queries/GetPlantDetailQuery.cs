using System.Collections.Immutable;
using Domain.PlantAggregate;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using static Application.Plant.Queries.GetPlantDetailQuery;

namespace Application.Plant.Queries;

public class GetPlantDetailQuery : IRequest<Response>
{
    public  Guid Id { get; set; }
    
    public class Handler : IRequestHandler<GetPlantDetailQuery, Response>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger _logger;

        public Handler(ILogger<GetPlantDetailQuery> logger,
            ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<Response> Handle(GetPlantDetailQuery query, CancellationToken cancellationToken)
        {
            var plant = await _context.Plants
                .AsNoTracking()
                .Where(p=>p.Id == query.Id)
                .SingleAsync(cancellationToken);

            var response = new Response();
            response.Id = plant.Id;
            response.Name = plant.Name;
            response.Location = plant.Location;
            response.LastWateredAt = plant.LastWateredAt;
            response.IsWatering = plant.IsWatering;
            response.ImageUrl = plant.ImageUrl;
            
            var duration = DateTimeOffset.UtcNow - plant.LastWateredAt.GetValueOrDefault(DateTimeOffset.MinValue);
            if (plant.IsWatering)
                response.Status = PlantStatus.Watering;
            else if (duration < TimeSpan.FromSeconds(30))
                response.Status = PlantStatus.Resting;
            else if (duration >= TimeSpan.FromHours(6))
                response.Status = PlantStatus.NeededWater;
            else
                response.Status = PlantStatus.Normal;

            return response;
        }
    }

    public record Response
    {
        public Guid Id { get; set; } = Guid.Empty;

        public string Name { get; set; } = string.Empty;

        public string ImageUrl { get; set; } = string.Empty;
        
        public string Location { get; set; } = string.Empty;

        public DateTimeOffset? LastWateredAt { get; set; }

        public PlantStatus Status { get; set; } = PlantStatus.Normal;
        
        public bool IsWatering { get; set; }
    }
}