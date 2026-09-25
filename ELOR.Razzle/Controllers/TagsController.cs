using ELOR.Razzle.Attributes;
using ELOR.Razzle.DTO.Requests;
using ELOR.Razzle.Services;

namespace ELOR.Razzle.Controllers
{
    public class TagsController : APIControllerBase
    {
        private readonly TagsService _service;

        public TagsController(TagsService service)
        {
            _service = service;
        }

        [AuthRequired]
        [Idempotent]
        public async Task<object> AddAsync(TagAddRequest request)
        {
            return await _service.AddAsync(request);
        }

        [AuthRequired]
        public async Task<object> GetAsync()
        {
            return await _service.GetAsync();
        }
    }
}
