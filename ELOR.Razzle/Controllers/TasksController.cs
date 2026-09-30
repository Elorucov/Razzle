using ELOR.Razzle.Attributes;
using ELOR.Razzle.DTO.Requests;
using ELOR.Razzle.Services;

namespace ELOR.Razzle.Controllers
{
    public sealed class TasksController : APIControllerBase
    {
        private readonly TasksService _service;

        public TasksController(TasksService service)
        {
            _service = service;
        }

        [AuthRequired]
        [Idempotent]
        public async Task<object> CreateAsync(TaskCreateRequest request)
        {
            return await _service.CreateAsync(request);
        }
    }
}
