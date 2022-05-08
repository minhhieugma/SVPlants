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

public class StopWateringPlantCommand : IRequest
{
    public Guid? Id { get; set; } = Guid.NewGuid();

    public class Validator : AbstractValidator<StopWateringPlantCommand>
    {
        public Validator()
        {
            RuleFor(p => p.Id).NotEqual(Guid.Empty);
        }
    }

    public class Handler : IRequestHandler<StopWateringPlantCommand>
    {
        private readonly ILogger _logger;
        private readonly ApplicationDbContext _context;

        public Handler(ILogger<StopWateringPlantCommand> logger,
            ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<Unit> Handle(StopWateringPlantCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var plant = await _context.Plants.SingleAsync(p => p.Id == command.Id);

                var duration = DateTimeOffset.UtcNow - plant.LastWateredAt.GetValueOrDefault(DateTimeOffset.MinValue);
                
                if(plant.IsWatering == false)
                    throw new MyApplicationException($"Plant {plant.Name} is not watering.");

                plant.IsWatering = false;
                plant.LastWateredAt = DateTimeOffset.Now;

                await _context.SaveChangesAsync(cancellationToken);

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