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

        // For testing enum conventions
        public async Task<object> AddAsync(TagAddRequest request)
        {
            return request;
        }
    }
}
