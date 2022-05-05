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

            return plants
                .Select(p => new Response
                {
                    Id = p.Id,
                    Name = p.Name,
                    Location = p.Location,
                    Status = p.Status,
                    LastWateredAt = p.LastWateredAt
                })
                .ToImmutableList();
        }
    }

    public record Response
    {
        public Guid Id { get; init; } = Guid.Empty;

        public string Name { get; init; } = string.Empty;

        public string Location { get; init; } = string.Empty;

        public DateTimeOffset? LastWateredAt { get; init; }

        public PlantStatus Status { get; init; } = PlantStatus.Normal;
    }
}