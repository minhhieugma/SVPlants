using Application.Plant.Commands;
using Application.Plant.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace SVPlants.Controllers;

[ApiController]
[Route("[controller]")]
public class PlantController : ControllerBase
{
    private readonly IMediator _mediator;

    public PlantController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IEnumerable<SearchPlantsQuery.Response>> Get()
    {
        var response = await _mediator.Send(new SearchPlantsQuery());

        return response;
    }

    [HttpGet("{id}")]
    public async Task<GetPlantDetailQuery.Response> Get(Guid id)
    {
        var response = await _mediator.Send(new GetPlantDetailQuery {Id = id});

        return response;
    }

    [HttpPost("water/start")]
    public async Task<IEnumerable<SearchPlantsQuery.Response>> StartWateringPlant(StartWateringPlantCommand command)
    {
        await _mediator.Send(command);

        var updatedPlants = await _mediator.Send(new SearchPlantsQuery {Ids = command.Ids});

        return updatedPlants;
    }

    [HttpPost("{plantId}/water/stop")]
    public async Task<GetPlantDetailQuery.Response> StopWateringPlant(Guid plantId)
    {
        await _mediator.Send(new StopWateringPlantCommand {Id = plantId});

        return await _mediator.Send(new GetPlantDetailQuery {Id = plantId});
    }
}