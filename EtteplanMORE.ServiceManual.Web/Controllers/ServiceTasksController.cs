using EtteplanMORE.ServiceManual.ApplicationCore.Entities;
using EtteplanMORE.ServiceManual.ApplicationCore.Interfaces;

using Microsoft.AspNetCore.Mvc;

namespace EtteplanMORE.ServiceManual.Web.Controllers
{
    [Route("api/[controller]")]
    public class ServiceTasksController : Controller
    {
        private readonly IServiceTaskService _serviceTaskService;

        public ServiceTasksController(IServiceTaskService serviceTaskService)
        {
            _serviceTaskService = serviceTaskService;
        }

        /// <summary>
        ///     HTTP POST
        /// </summary>
        [HttpPost("add")]
        public async Task<IActionResult> Add([FromBody] ServiceTask serviceTask)
        {
            return await _serviceTaskService.Add(serviceTask) == true ? Succeeded(serviceTask) : BadRequest();
        }

        /// <summary>
        ///     HTTP GET: api/servicetasks/1
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var serviceTask = await _serviceTaskService.Get(id);
            return serviceTask != null ? Succeeded(serviceTask) : NotFound();
        }

        /// <summary>
        ///     HTTP PUT: api/servicetasks/edit
        /// </summary>

        [HttpPut("edit")]
        public async Task<IActionResult> Edit([FromBody] ServiceTask serviceTask)
        {
            return await _serviceTaskService.Edit(serviceTask) == true ? Succeeded(serviceTask) : NotFound();
        }

        /// <summary>
        ///     HTTP DELETE: api/servicetasks/remove/1
        /// </summary>

        [HttpDelete("remove/{id}")]
        public async Task<IActionResult> Remove(int id)
        {
            return await _serviceTaskService.Remove(id) == true ? Ok(id) : NotFound();
        }

        /// <summary>
        ///     HTTP GET: api/servicetasks/
        /// </summary>
        [HttpGet]
        public async Task<IEnumerable<ServiceTaskDto>> Get()
        {
            return (await _serviceTaskService.GetAll())
                .Select(st =>
                    new ServiceTaskDto
                    {
                        Id = st.Id,
                        FactoryDevice = st.FactoryDevice,
                        Created = st.Created,
                        Description = st.Description,
                        Criticality = st.Criticality,
                        State = st.State
                    }
                );
        }

        /// <summary>
        ///     HTTP GET: api/servicetasks/bydevice/1
        /// </summary>
        [HttpGet("bydevice/{deviceId}")]
        public async Task<IEnumerable<ServiceTaskDto>> GetByDevice(int deviceId)
        {
            return (await _serviceTaskService.GetByDevice(deviceId))
                .Select(st =>
                    new ServiceTaskDto
                    {
                        Id = st.Id,
                        FactoryDevice = st.FactoryDevice,
                        Created = st.Created,
                        Description = st.Description,
                        Criticality = st.Criticality,
                        State = st.State
                    }
                );
        }

        private OkObjectResult Succeeded(ServiceTask serviceTask)
        {
            return Ok(new ServiceTaskDto
            {
                Id = serviceTask.Id,
                FactoryDevice = serviceTask.FactoryDevice,
                Created = serviceTask.Created,
                Description = serviceTask.Description,
                Criticality = serviceTask.Criticality,
                State = serviceTask.State
            });
        }

    }
}
