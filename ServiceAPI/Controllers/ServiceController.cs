using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServiceAPI.Data;

namespace ServiceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceController : ControllerBase
    {
        private readonly ServiceContext _context;

        public ServiceController(ServiceContext context)
        {
            _context = context;
        }

        [HttpPost("Create")]
        public async Task<ActionResult<List<Service>>> AddService(Service service)
        {
            _context.services.Add(service);
            await _context.SaveChangesAsync();
            return Ok(await _context.services.ToListAsync());
        }

        [HttpGet("Read")]
        public async Task<ActionResult<List<Service>>> Get()
        {
            return Ok(await _context.services.ToListAsync());
        }

        [HttpPut("Update")]
        public async Task<ActionResult<List<Service>>> UpdateService(Service changedService)
        {
            var service = await _context.services.FindAsync(changedService.Id);
            if (service == null)
            {
                return BadRequest("Service not found!");
            }
            service.Name = changedService.Name;
            service.ShortDesc = changedService.ShortDesc;
            if (service.StateId != changedService.StateId)
            {
                var newServiceState = new ServiceState
                {
                    ServiceId = service.Id,
                    OldStateId = service.StateId,
                    NewStateId = changedService.StateId,
                    Time = DateTime.Now
                };
                _context.serviceStates.Add(newServiceState);
            }
            service.StateId = changedService.StateId;
            await _context.SaveChangesAsync();
            return Ok(await _context.services.ToListAsync());
        }

        [HttpDelete("Delete/{id}")]
        public async Task<ActionResult<List<Service>>> DeleteService(int id)
        {
            var service = await _context.services.FindAsync(id);
            if (service == null)
            {
                return BadRequest("Service not found!");
            }
            _context.services.Remove(service);
            await _context.SaveChangesAsync();
            return Ok(await _context.services.ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Service>> Get(int id)
        {
            var services = await _context.services.Join(_context.states, service => service.StateId, state => state.Id, (service, state) => new
            {
                id = service.Id,
                Name = service.Name,
                State = state.Name,
                ShortDesc = service.ShortDesc
            }).ToArrayAsync();
            return Ok(services);
        }

        [HttpGet("get_state_of_services")]
        public async Task<ActionResult<List<Service>>> GetStateOfServices()
        {
            var services = await _context.services.Join(_context.states, service => service.StateId, state => state.Id, (service, state) => new
            {
                Name = service.Name,
                State = state.Name
            }).ToArrayAsync();
            return Ok(services);
        }

        [HttpGet("get_service_state_logs/{name}")]
        public async Task<ActionResult<List<Service>>> GetServiceStateLogs(string name)
        {
            var serviceStates = await _context.serviceStates.Join(_context.states, serviceState => serviceState.OldStateId, state => state.Id, (serviceState, state) => new 
            {
                Id = serviceState.Id,
                ServiceId = serviceState.ServiceId,
                OldState = state.Name,
                NewStateId = serviceState.NewStateId,
                Time = serviceState.Time
            }).Join(_context.states, serviceState => serviceState.NewStateId, state => state.Id, (serviceState, state) => new 
            {
                Id = serviceState.Id,
                ServiceId = serviceState.ServiceId,
                OldState = serviceState.OldState,
                NewState = state.Name,
                Time = serviceState.Time
            }).Join(_context.services, serviceState => serviceState.ServiceId, service=>service.Id, (serviceState, service) => new 
            {
                Id = serviceState.Id,
                Service = service.Name,
                OldState = serviceState.OldState,
                NewState = serviceState.NewState,
                Time = serviceState.Time

            }).ToArrayAsync();
            return Ok(serviceStates);
        }

    }
}
