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
        
        [HttpPost("{plantId}/water")]
        public async Task WaterPlant(Guid plantId)
        {
            await _mediator.Send(new WaterPlantCommand{ Id = plantId});
        }
    }
}