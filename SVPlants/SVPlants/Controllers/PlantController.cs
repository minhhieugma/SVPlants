using Application.Plant.Commands;
using Application.Plant.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace SVPlants.Controllers
{
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
        public async Task<IEnumerable<GetAllPlantsQuery.Response>> Get()
        {
            var response = await _mediator.Send(new  GetAllPlantsQuery());

            return response;
        }
        
        [HttpGet("{id}")]
        public async Task<GetPlantDetailQuery.Response> Get(Guid id)
        {
            var response = await _mediator.Send(new  GetPlantDetailQuery{Id = id});

            return response;
        }

        [HttpPost("{plantId}/water/start")]
        public async Task<GetPlantDetailQuery.Response> StartWateringPlant(Guid plantId)
        {
            await _mediator.Send(new StartWateringPlantCommand{ Id = plantId});
            
            return await _mediator.Send(new  GetPlantDetailQuery{Id = plantId});
        }
        
        [HttpPost("{plantId}/water/stop")]
        public async Task<GetPlantDetailQuery.Response> StopWateringPlant(Guid plantId)
        {
            await _mediator.Send(new StopWateringPlantCommand{ Id = plantId});
            
            return await _mediator.Send(new  GetPlantDetailQuery{Id = plantId});
        }
    }
}