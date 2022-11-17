using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dot_net.Data;
using dot_net.DTOs;
using dot_net.DTOs.User;
using Microsoft.AspNetCore.Mvc;

namespace dot_net.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authRepo;
        public AuthController(IAuthRepository authRepo)
        {
            _authRepo = authRepo;
            
        }


        [HttpPost("register")]
        public async Task<ActionResult<ServiceResponse<int>>> Register(UserRegisterDTO request)
        {
            var response = await _authRepo.Register(
                new User {Username = request.Username}, request.Password
            );

            if(!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<ActionResult<ServiceResponse<string>>> Login(UserLoginDTO request)
        {
            var response = await _authRepo.Login(request.Username, request.Password);

            if(!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        // public async Task<ActionResult<bool>> UserExists()
        // {
            
        // }
    }
}