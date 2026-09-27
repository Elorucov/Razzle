using ELOR.Razzle.Attributes;
using ELOR.Razzle.DTO.Requests;
using ELOR.Razzle.Services;

namespace ELOR.Razzle.Controllers
{
    public sealed class NotesController : APIControllerBase
    {
        private readonly NotesService _service;

        public NotesController(NotesService service)
        {
            _service = service;
        }

        [AuthRequired]
        [Idempotent]
        public async Task<object> CreateAsync(NoteCreateRequest request)
        {
            return await _service.CreateAsync(request);
        }
    }
}
