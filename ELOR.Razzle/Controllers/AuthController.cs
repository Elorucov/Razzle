using ELOR.Razzle.Attributes;
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

        public async Task<object> SignInAsync(SignInRequest request)
        {
            return await _service.SignInAsync(request);
        }

        public async Task<object> SignUpAsync(SignUpRequest request)
        {
            return await _service.SignUpAsync(request);
        }

        [AuthRequired]
        public async Task<object> TestAsync()
        {
            var test = this.HttpContext.RequestServices.GetService<UserSession>();
            return test.Username;
        }
    }
}
