using System.Collections.Immutable;
using Domain.PlantAggregate;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using static Application.Plant.Queries.GetAllPlantsQuery;

namespace Application.Plant.Queries;

public class GetAllPlantsQuery : IRequest<IEnumerable<Response>>
{
    public class Handler : IRequestHandler<GetAllPlantsQuery, IEnumerable<Response>>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger _logger;

        public Handler(ILogger<GetAllPlantsQuery> logger,
            ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IEnumerable<Response>> Handle(GetAllPlantsQuery querry, CancellationToken cancellationToken)
        {
            var plants = await _context.Plants.ToListAsync(cancellationToken);

            var responses = new List<Response>();
            foreach (var plant in plants)
            {
                var response = new Response();
                response.Id = plant.Id;
                response.Name = plant.Name;
                response.Location = plant.Location;
                response.LastWateredAt = plant.LastWateredAt;
                
                var duration = DateTimeOffset.UtcNow - plant.LastWateredAt.GetValueOrDefault(DateTimeOffset.MinValue);
                if (plant.IsWatering)
                    response.Status = PlantStatus.Watering;
                else if (duration < TimeSpan.FromSeconds(30))
                    response.Status = PlantStatus.Resting;
                else if (duration >= TimeSpan.FromHours(6))
                    response.Status = PlantStatus.NeededWater;
                else
                    response.Status = PlantStatus.Normal;
                
                responses.Add(response);
            }

            return responses;
        }
    }

    public record Response
    {
        public Guid Id { get; set; } = Guid.Empty;

        public string Name { get; set; } = string.Empty;

        public string Location { get; set; } = string.Empty;

        public DateTimeOffset? LastWateredAt { get; set; }

        public PlantStatus Status { get; set; } = PlantStatus.Normal;
    }
}