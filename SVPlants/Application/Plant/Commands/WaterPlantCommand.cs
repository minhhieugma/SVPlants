using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Plant.Commands;

public class WaterPlantCommand : IRequest
{
    public Guid? Id { get; set; } = Guid.NewGuid();

    public class Validator : AbstractValidator<WaterPlantCommand>
    {
        public Validator()
        {
            RuleFor(p => p.Id).NotEqual(Guid.Empty);
        }
    }

    public class Handler : IRequestHandler<WaterPlantCommand>
    {
        private readonly ILogger _logger;

        public Handler(ILogger<WaterPlantCommand> logger)
        {
            _logger = logger;
        }

        public async Task<Unit> Handle(WaterPlantCommand command, CancellationToken cancellationToken)
        {
            try
            {
                return Unit.Value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed when execute command with {Payload}",
                    JsonSerializer.Serialize(new { command }));
                throw;
            }
        }
    }
}

