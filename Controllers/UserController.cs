using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PeyulErp.Exceptions;
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
        private const string AdminRole = "Admin";
        private readonly IPasswordService _passwordService;
        private readonly PasswordSettings _passwordSettings;

        public UsersController(
            IUsersService usersService,
            IPasswordService passwordService,
            IOptions<JwtSettings> jwtSettings,
            IOptions<PasswordSettings> passwordSettings)
        {
            _usersService = usersService;
            _jwtSettings = jwtSettings.Value;
            _passwordService = passwordService;
            _passwordSettings = passwordSettings.Value;
        }

        [HttpGet]
        public async Task<ActionResult> GetAllUsers()
        {
            try
            {
                var users = (await _usersService.GetAllAsync())
                    .Select(u => new {
                        u.Id,
                        u.Name,
                        u.Email,
                        u.PhoneNumber,
                        u.CreateDate,
                        u.Role,
                        u.Status,
                        u.UpdateDate
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
        //[AllowAnonymous]
        [Authorize(Roles = AdminRole)]
        public async Task<ActionResult> CreateUser(User user)
        {
            try
            {
                var newUser = await _usersService.CreateAsync(user);
                return CreatedAtAction(nameof(GetUser), new { id = user.Id }, new
                {
                    newUser.Name,
                    newUser.Email,
                    newUser.PhoneNumber,
                    newUser.CreateDate
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = AdminRole)]
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
                    updatedUser.Name,
                    updatedUser.Email,
                    updatedUser.PhoneNumber,
                    updatedUser.CreateDate
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = AdminRole)]
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

#pragma warning disable CS4014
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult> Login(User loginUser)
        {
            try
            {
                var user = await _usersService.GetByEmailAsync(loginUser.Email);
                var message = "UserName Or Password Incorrect";

                if (user == null)
                    return Unauthorized(message);

                var userPassword = await _passwordService.GetByUserId(user.Id);
                var isPasswordValid = await IsLoginValid(user, userPassword, loginUser.Password);

                if (!isPasswordValid)
                {
                    await _passwordService.IncrementFailedAttempts(user.Id);

                    return Unauthorized(message);
                }

                if(userPassword.FailedAttempts > 0)
                {
                    _passwordService.ResetPasswordAttempts(user.Id).ConfigureAwait(false);
                }

                return Ok(new LoginResponse(user.GetUserToken(_jwtSettings),user.Email,user.Role.ToString(), userPassword.ForceReset));
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
#pragma warning restore CS4014

        private async Task<bool> IsLoginValid(User user, UserPassword userPassword, string password)
        {
            if(user.Status != UserStatus.Active)
            {
                throw new UserException("User is inactive");
            }

            if(userPassword == null)
            {
                await _passwordService.Upsert(user);
                userPassword = await _passwordService.GetByUserId(user.Id);
            }

            if( userPassword.FailedAttempts > _passwordSettings.MaxAttempts)
            {
                throw new PasswordException("Maximum password attempts exceeded");
            }

            return user.Password == Password.HashPassword(password);            
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