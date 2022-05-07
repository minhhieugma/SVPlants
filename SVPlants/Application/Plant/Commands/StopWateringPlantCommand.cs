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
                var plant = await _context.Plants.SingleAsync(p => p.Id == command.Id);

                switch (plant.Status)
                {
                    case PlantStatus.Resting:
                        throw new MyApplicationException($"Plant {plant.Id} needs to rest from watering.");
                    case PlantStatus.Watering:
                        throw new MyApplicationException(
                            $"Plant {plant.Id} is being watering from {plant.LastWateredAt}. Please wait for a while for another shot.");
                }

                var duration = DateTimeOffset.UtcNow - plant.LastWateredAt.GetValueOrDefault(DateTimeOffset.MinValue);
                // if (duration <= TimeSpan.FromSeconds(10))
                //     throw new MyApplicationException($"Plant {plant.Name} is watering.");

                if(plant.IsWatering)
                    throw new MyApplicationException($"Plant {plant.Name} is watering.");
                
                if (duration <= TimeSpan.FromSeconds(30))
                    throw new MyApplicationException($"Plant {plant.Name} needs to rest from watering.");

                plant.LastWateredAt = DateTimeOffset.Now;
                //plant.Status = PlantStatus.Watering;

                await _context.SaveChangesAsync(cancellationToken);

                /*if(plant.LastWateredAt != null && plant.LastWateredAt.Value.AddSeconds(30) > DateTimeOffset.Now)
                    throw new MyApplicationException($"Plant {plant.Id} is being watering from {plant.LastWateredAt}. Please wait for a while for another shot.");return*/

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