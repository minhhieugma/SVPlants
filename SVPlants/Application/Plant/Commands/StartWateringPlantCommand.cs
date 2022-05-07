using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Application.Exceptions;
using Domain;
using Domain.PlantAggregate;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Plant.Commands;

public class StartWateringPlantCommand : IRequest
{
    public Guid? Id { get; set; } = Guid.NewGuid();

    public class Validator : AbstractValidator<StartWateringPlantCommand>
    {
        public Validator()
        {
            RuleFor(p => p.Id).NotEqual(Guid.Empty);
        }
    }

    public class Handler : IRequestHandler<StartWateringPlantCommand>
    {
        private readonly ILogger _logger;
        private readonly ApplicationDbContext _context;

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
                    var plant = await _context.Plants.SingleAsync(p => p.Id == command.Id);

                    var duration = DateTimeOffset.UtcNow -
                                   plant.LastWateredAt.GetValueOrDefault(DateTimeOffset.MinValue);

                    if (plant.IsWatering)
                        throw new MyApplicationException($"Plant {plant.Name} is watering.");

                    if (duration <= TimeSpan.FromSeconds(30))
                        throw new MyApplicationException($"Plant {plant.Name} needs to rest from watering.");

                    plant.IsWatering = true;

                    await _context.SaveChangesAsync(cancellationToken);
                });

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