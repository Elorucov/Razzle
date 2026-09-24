using ELOR.Razzle.DTO.Requests;
using ELOR.Razzle.Services;

namespace ELOR.Razzle.Controllers
{
    public sealed class AuthController : APIControllerBase
    {
        private readonly AuthService _service;

        public AuthController(AuthService service)
        {
            _service = service;
        }

        public async Task<object> SignUpAsync(SignUpRequest request)
        {
            return await _service.SignUpAsync(request);
        }
    }
}
