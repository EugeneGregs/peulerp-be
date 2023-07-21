using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PeyulErp.Model;
using PeyulErp.Models;
using PeyulErp.Services;
using PeyulErp.Settings;
using PeyulErp.Utility;

namespace PeyulErp.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UsersController: ControllerBase
    {
        private readonly IUsersService _usersService;
        private readonly JwtSettings _jwtSettings;

        public UsersController(IUsersService usersService, IOptions<JwtSettings> jwtSettings)
        {
            _usersService = usersService;
            _jwtSettings = jwtSettings.Value;
        }

        [HttpGet]
        public async Task<ActionResult> GetAllUsers()
        {
            try
            {
                var users = (await _usersService.GetAllAsync())
                    .Select(u => new {
                        Name = u.Name,
                        Email = u.Email,
                        PhoneNumber = u.PhoneNumber,
                        CreateDate = u.CreateDate
                    });

                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetUser(Guid id)
        {
            try
            {
                var user = await _usersService.GetByIdAsync(id);

                if (user == null)
                {
                    return NotFound();
                }

                return Ok(new
                {
                    Name = user.Name,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    CreateDate = user.CreateDate
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> CreateUser(User user)
        {
            try
            {
                var newUser = await _usersService.CreateAsync(user);
                return CreatedAtAction(nameof(GetUser), new { id = user.Id }, new
                {
                    Name = newUser.Name,
                    Email = newUser.Email,
                    PhoneNumber = newUser.PhoneNumber,
                    CreateDate = newUser.CreateDate
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> UpdateUser(Guid id, User user)
        {
            try
            {
                var existingUser = await _usersService.GetByIdAsync(id);

                if (existingUser == null)
                {
                    return NotFound();
                }

                var updatedUser = await _usersService.UpdateAsync(user);

                return Ok(new
                {
                    Name = updatedUser.Name,
                    Email = updatedUser.Email,
                    PhoneNumber = updatedUser.PhoneNumber,
                    CreateDate = updatedUser.CreateDate
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteUser(Guid id)
        {
            try
            {
                var existingUser = await _usersService.GetByIdAsync(id);

                if (existingUser == null)
                {
                    return NotFound();
                }

                await _usersService.DeleteAsync(id);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult> Login(User loginUser)
        {
            try
            {
                var user = await _usersService.GetByEmailAsync(loginUser.Email);

                if (user == null || user.Password != Password.HashPassword(loginUser.Password))
                {
                    return Unauthorized("UserName Or Password Incorrect");
                }

                return Ok(new LoginResponse(user.GetUserToken(_jwtSettings)));
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpPost("forgot-password")]
        public async Task<ActionResult> ForgotPassword(User user)
        {
            try
            {
                var existingUser = await _usersService.GetByEmailAsync(user.Email);

                if (existingUser == null)
                {
                    return NotFound();
                }

                await _usersService.ResetUserPasswordAsync(existingUser);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }            
        }

        //TODO: Authorize this endpoint
        [HttpPost("update-password")]
        public async Task<ActionResult> UpdatePassword(User user)
        {
            try
            {
                var existingUser = await _usersService.GetByEmailAsync(user.Email);

                if (existingUser == null)
                {
                    return NotFound();
                }

                await _usersService.UpdateUserPassWordAsync(existingUser.Id, user.Password);

                return Ok();
            }      
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}