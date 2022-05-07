using System.Text.Json;
using Domain.PlantAggregate;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Plant.Commands;

public class UpdateWateringStatusCommand : IRequest
{
    public class Handler : IRequestHandler<UpdateWateringStatusCommand>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger _logger;

        public Handler(ILogger<UpdateWateringStatusCommand> logger,
            ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<Unit> Handle(UpdateWateringStatusCommand command, CancellationToken cancellationToken)
        {
            // try
            // {
            //     var plants = await _context.Plants.ToListAsync();
            //
            //     foreach (var plant in plants)
            //     {
            //         if (plant.LastWateredAt == null || plant.LastWateredAt.Value.AddHours(6) < DateTimeOffset.Now)
            //         {
            //             plant.Status = PlantStatus.NeededWater;
            //             continue;
            //         }
            //
            //         if (plant.LastWateredAt != null
            //             && plant.LastWateredAt.Value.AddSeconds(30) < DateTimeOffset.Now
            //             && plant.Status == PlantStatus.Resting)
            //         {
            //             plant.Status = PlantStatus.Normal;
            //             continue;
            //         }
            //
            //         if (plant.LastWateredAt != null
            //             && plant.LastWateredAt.Value.AddSeconds(30) >= DateTimeOffset.Now
            //             && plant.Status != PlantStatus.Resting)
            //         {
            //             plant.Status = PlantStatus.Resting;
            //         }
            //     }
            //
            //     await _context.SaveChangesAsync(cancellationToken);
            //
            //     return Unit.Value;
            // }
            // catch (Exception ex)
            // {
            //     _logger.LogError(ex, "Failed when execute command with {Payload}",
            //         JsonSerializer.Serialize(new {command}));
            //     throw;
            // }
            
            return Unit.Value;
        }
    }
}