using ELOR.Razzle.Services;
using Microsoft.AspNetCore.Mvc;

namespace ELOR.Razzle.Controllers
{
    public sealed class AuthController : APIControllerBase
    {
        private readonly AuthService _service;

        public AuthController(AuthService service)
        {
            _service = service;
        }

        public async Task<object> TestAsync()
        {
            return await _service.TestAsync();
        }

        public async Task<object> ThrowAsync()
        {
            return await _service.ThrowAsync();
        }
    }
}
