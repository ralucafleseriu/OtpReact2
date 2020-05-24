using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OtpReact.Models;
using OtpReact.Services;

namespace OtpReact.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OneTimePasswordController : ControllerBase
    {
        private readonly OneTimePasswordService _oneTimePasswordService;

        public OneTimePasswordController()
        {
            _oneTimePasswordService = new OneTimePasswordService();
        }

        [HttpGet]
        public async Task<OneTimePassword> Get()
        {
            var userId = Guid.NewGuid();
            var dateTime = DateTime.UtcNow;
            var password = await _oneTimePasswordService.GenerateNewOneTimePassword(userId, dateTime);
            return await Task.FromResult(password);
        }

        [HttpPost("validate/{password}")]
        public async Task<bool> ValidateOneTimePassword(string password, [FromBody]  string userId)
        {
            var isValid = await _oneTimePasswordService.ValidateOneTimePassword(new Guid(userId), password);
            return await Task.FromResult(isValid);
        }
    }
}
