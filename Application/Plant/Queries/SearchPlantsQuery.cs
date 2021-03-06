using Domain.PlantAggregate;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using static Application.Plant.Queries.SearchPlantsQuery;

namespace Application.Plant.Queries;

public class SearchPlantsQuery : IRequest<IEnumerable<Response>>
{
    public Guid[]? Ids { get; set; }
    
    public class Handler : IRequestHandler<SearchPlantsQuery, IEnumerable<Response>>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger _logger;

        public Handler(ILogger<SearchPlantsQuery> logger,
            ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IEnumerable<Response>> Handle(SearchPlantsQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Plants.AsNoTracking();

            if (request.Ids != null)
                query = query.Where(p => request.Ids.Contains(p.Id));
            
            var plants = await query.ToListAsync(cancellationToken);
            
            var responses = new List<Response>();
            foreach (var plant in plants)
            {
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
                
                responses.Add(response);
            }

            return responses;
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