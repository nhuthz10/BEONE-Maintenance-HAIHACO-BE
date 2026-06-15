using Maintenance.Entities.Auth;
using Maintenance.Entities.Responses;
using Maintenance.Infrastructure.SqlServer.Data;
using Maintenance.Infrastructure.SqlServer.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.Infrastructure.SqlServer.Repositories.Auth
{
    public class AuthRepository : IAuthRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthRepository(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _context = context;
            _configuration = configuration;
        }

        public async Task<OperationResult<ViewLoginModel>> AppSignIn(LoginModel model)
        {
            try
            {
                var validVersion = _configuration["AppSettings:ValidVersion"];
                if (model.Version != validVersion)
                    return OperationResult<ViewLoginModel>.Fail(ErrorCode.InvalidVersion, "Your app version is not supported. Please update to the latest version");
                var user = await _userManager.FindByNameAsync(model.UserName);
                if (user == null)
                {
                    return OperationResult<ViewLoginModel>.Fail(ErrorCode.InvalidUsernamePassword, "Incorrect account or password");
                }


                //if (user.IsOnline && user.DeviceId != model.DeviceId)
                //{
                //    return OperationResult<ViewLoginModel>.Fail(
                //        ErrorCode.AccountAlreadyOnline,
                //        "Account is currently in use on another device"
                //    );
                //}

                var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, false, false);
                if (!result.Succeeded)
                {
                    if (result.IsLockedOut)
                    {
                        return OperationResult<ViewLoginModel>.Fail(ErrorCode.AccountLocked, "Account locked");
                    }

                    await _userManager.AccessFailedAsync(user);
                    return OperationResult<ViewLoginModel>.Fail(ErrorCode.InvalidUsernamePassword, "Incorrect account or password");
                }
                //if (user.DeviceId == null || user.DeviceId == "")
                //{
                //    user.DeviceId = model.DeviceId;
                //    await _userManager.UpdateAsync(user);
                //    await _context.SaveChangesAsync();
                //}
                //else
                //{
                //    if (user.DeviceId != model.DeviceId)
                //    {
                //        return OperationResult<ViewLoginModel>.Fail(ErrorCode.DeviceDontMatch, "Device does not match");
                //    }
                //}

                user.DeviceId = model.DeviceId;
                await _userManager.UpdateAsync(user);


                var userRoles = await _userManager.GetRolesAsync(user);

                var jwtId = Guid.NewGuid().ToString();
                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Email, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, jwtId),
                };
                foreach (var role in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, role));
                }
                authClaims.Add(new Claim("DeviceId", model.DeviceId));
                authClaims.Add(new Claim("Version", _configuration["AppSettings:ValidVersion"]));
                var authenKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["AppSettings:SecretKey"] ?? ""));
                var token = new JwtSecurityToken(
                        issuer: _configuration["AppSettings:ValidIssuer"],
                        audience: _configuration["AppSettings:ValidAudience"],
                        //expires: TimeZoneInfo.ConvertTimeToUtc(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59), TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time")),
                        expires: DateTime.Now.AddYears(10),
                        claims: authClaims,
                        signingCredentials: new SigningCredentials(authenKey, SecurityAlgorithms.HmacSha512Signature)
                    );
                var jwt = new JwtSecurityTokenHandler().WriteToken(token);
                var info = new UserViewModel()
                {
                    Id = user.Id,
                    UserName = user.UserName.Split('@')[0],
                    FullName = user.FullName,
                    Email = user.Email ?? string.Empty,
                    PhoneNumber = user.PhoneNumber ?? string.Empty,
                    Roles = new List<string>()
                };
                var roles = await _userManager.GetRolesAsync(user);
                foreach (var role in roles)
                {
                    info.Roles.Add(role);
                }

                await _context.SaveChangesAsync();

                return OperationResult<ViewLoginModel>.Success(new ViewLoginModel { token = jwt, userInfo = info }, "Signin successfully");
            }
            catch (Exception ex) { throw new Exception(ex.Message, ex); }
        }

        public async Task<OperationResult<ViewLoginModel>> SignIn(LoginAdminModel model)
        {
            try
            {
                var validVersion = _configuration["AppSettings:ValidVersion"];

                var user = await _userManager.FindByNameAsync(model.UserName);
                if (user == null)
                {
                    return OperationResult<ViewLoginModel>.Fail(ErrorCode.InvalidUsernamePassword, "Incorrect account or password");
                }
                var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, false, false);
                if (!result.Succeeded)
                {
                    if (result.IsLockedOut)
                    {
                        return OperationResult<ViewLoginModel>.Fail(ErrorCode.AccountLocked, "Incorrect account or password");
                    }

                    await _userManager.AccessFailedAsync(user);
                    return OperationResult<ViewLoginModel>.Fail(ErrorCode.InvalidUsernamePassword, "Incorrect account or password");
                }

                var userRoles = await _userManager.GetRolesAsync(user);

                //if (!userRoles.Contains("Admin"))
                //{
                //    return OperationResult<ViewLoginModel>.Fail(ErrorCode.InvalidUsernamePassword, "Incorrect account or password");
                //}

                var jwtId = Guid.NewGuid().ToString();
                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Email, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, jwtId),
                };
                foreach (var role in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, role));
                }
                var authenKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["AppSettings:SecretKey"] ?? ""));
                var token = new JwtSecurityToken(
                        issuer: _configuration["AppSettings:ValidIssuer"],
                        audience: _configuration["AppSettings:ValidAudience"],
                        expires: TimeZoneInfo.ConvertTimeToUtc(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59), TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time")),
                        claims: authClaims,
                        signingCredentials: new SigningCredentials(authenKey, SecurityAlgorithms.HmacSha512Signature)
                    );
                var jwt = new JwtSecurityTokenHandler().WriteToken(token);
                var info = new UserViewModel()
                {
                    Id = user.Id,
                    UserName = user.UserName.Split('@')[0],
                    FullName = user.FullName,
                    Email = user.Email ?? string.Empty,
                    PhoneNumber = user.PhoneNumber ?? string.Empty,
                    Roles = new List<string>()
                };
                var roles = await _userManager.GetRolesAsync(user);
                foreach (var role in roles)
                {
                    info.Roles.Add(role);
                }
                return OperationResult<ViewLoginModel>.Success(new ViewLoginModel { token = jwt, userInfo = info }, "Signin successfully");
            }
            catch (Exception ex) { throw new Exception(ex.Message, ex); }
        }

        public async Task<OperationResult<bool>> CreateUser(CreateUserModel model)
        {
            try
            {
                var existingUser = await _userManager.FindByNameAsync(model.UserName);
                if (existingUser != null)
                {
                    return OperationResult<bool>.Fail(ErrorCode.UserNameAlreadyExists, "UserName already exists");
                }

                var user = new ApplicationUser
                {
                    UserName = model.UserName,
                    FullName = model.FullName,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber
                };

                var result = await _userManager.CreateAsync(user, "123456aB@");
                if (!result.Succeeded)
                {
                    var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                    return OperationResult<bool>.Fail(ErrorCode.FailedCreateAccount, errors);
                }

                return OperationResult<bool>.Success(true, "Created user successfully");
            }
            catch (Exception ex) { throw new Exception(ex.Message, ex); }
        }

        public async Task<OperationResult<bool>> LockoutUser(string id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                    return OperationResult<bool>.Fail(ErrorCode.NotFound, "Cannot find user");
                await _userManager.SetLockoutEnabledAsync(user, true);
                await _userManager.SetLockoutEndDateAsync(user, DateTime.MaxValue);
                return OperationResult<bool>.Success(true, "Lockout user successfully");
            }
            catch (Exception ex) { throw new Exception(ex.Message, ex); }
        }

        public async Task<OperationResult<bool>> UnlockoutUser(string id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                    return OperationResult<bool>.Fail(ErrorCode.NotFound, "Cannot find user");
                await _userManager.SetLockoutEndDateAsync(user, null);
                return OperationResult<bool>.Success(true, "Unlock user successfully");
            }
            catch (Exception ex) { throw new Exception(ex.Message, ex); }
        }

        public async Task<OperationResult<bool>> CheckLockUser(string id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null) { throw new InvalidOperationException("Cannot find user"); }
                bool isLocked = await _userManager.IsLockedOutAsync(user);

                return !isLocked ? OperationResult<bool>.Success(isLocked) : OperationResult<bool>.Fail(ErrorCode.TokenExpired, "Token has expired");
            }
            catch (Exception ex) { throw new Exception(ex.Message, ex); }
        }
    }
}
