using DAL.Entities.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ShopApi.Models;
using ShopApi.Services;

namespace ShopApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<UserEntity> _userManager;
        private readonly IJwtTokenService _jwtTokenService;
        public AccountController(UserManager<UserEntity> userManager, 
            IJwtTokenService jwtTokenService)
        {
            _userManager = userManager;
            _jwtTokenService = jwtTokenService;
        }
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody]LoginViewModel model)
        {
            var user = await _userManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                return BadRequest(new { error = "Дані вказано не вірно"});
            }
            var checkPassword = await _userManager.CheckPasswordAsync(user, model.Password);
            if (!checkPassword) {
                return BadRequest(new { error = "Дані вказано не вірно" });
            }
            string token = await _jwtTokenService.CreateToken(user);
            return Ok(new { token });
        }

        [HttpPost("GoogleExternalLogin")]
        public async Task<IActionResult> GoogleExternalLoginAsync([FromBody] ExternalLoginRequest request)
        {
            try {
                var payload = await _jwtTokenService.VerifyGoogleToken(request);
                if (payload == null)
                {
                    return BadRequest(new { error = "Щось пішло не так!" });
                }
                var info = new UserLoginInfo(request.Provider, payload.Subject, request.Provider);
                var user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
                if (user == null)
                {
                    user = await _userManager.FindByEmailAsync(payload.Email);
                    if (user == null)
                    {
                        user = new UserEntity
                        {
                            Email = payload.Email,
                            UserName = payload.Email,
                            FirstName = payload.GivenName,
                            LastName = payload.FamilyName
                        };
                        var resultCreate = await _userManager.CreateAsync(user);
                        if (!resultCreate.Succeeded)
                        {
                            return BadRequest(new { error = "Помилка створення користувача" });
                        }
                    }
                    var resultLOgin = await _userManager.AddLoginAsync(user, info);
                    if (!resultLOgin.Succeeded)
                    {
                        return BadRequest(new { error = "Створення входу через гугл" });
                    }
                }
                string token = await _jwtTokenService.CreateToken(user);
                return Ok(new { token });
            }
            catch(Exception ex)
            {
                return BadRequest(ex);
            }
            

            
        }
    }
}
