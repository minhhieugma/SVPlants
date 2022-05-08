using System.Text.Json;
using Application.Exceptions;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Plant.Commands;

public class StartWateringPlantCommand : IRequest
{
    public Guid[] Ids { get; set; } = new Guid[0];

    public class Validator : AbstractValidator<StartWateringPlantCommand>
    {
        public Validator()
        {
            RuleFor(p => p.Ids.Length).NotEqual(0);
        }
    }

    public class Handler : IRequestHandler<StartWateringPlantCommand>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger _logger;

        public Handler(ILogger<StartWateringPlantCommand> logger,
            ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<Unit> Handle(StartWateringPlantCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var mainTask = Task.Run(async () =>
                {
                    var plants = await _context.Plants
                        .Where(p => command.Ids.Contains(p.Id))
                        .ToListAsync(cancellationToken);

                    foreach (var plant in plants)
                    {
                        var duration = DateTimeOffset.UtcNow -
                                       plant.LastWateredAt.GetValueOrDefault(DateTimeOffset.MinValue);

                        if (plant.IsWatering)
                            throw new MyApplicationException($"Plant {plant.Name} is watering.");

                        if (duration <= TimeSpan.FromSeconds(30))
                            throw new MyApplicationException($"Plant {plant.Name} needs to rest from watering.");

                        plant.IsWatering = true;
                    }

                    await _context.SaveChangesAsync(cancellationToken);
                });

                // Simulate the watering takes 10 seconds to complete
                Task.WaitAll(mainTask, Task.Delay(TimeSpan.FromSeconds(10)));

                return Unit.Value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed when execute command with {Payload}",
                    JsonSerializer.Serialize(new {command}));
                throw;
            }
        }
    }
}