using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Api.Dtos;
using Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Api.Controllers;

public class UsuarioController : ApiBaseController
{
    private readonly IUserService _userService;
    public UsuarioController(IUserService uservice)
    {
        _userService = uservice;
    }

    [HttpPost("register")]
    public async Task<ActionResult> RegisterAsync(RegisterDto model)
    {
        var result = await _userService.RegisterAsync(model);
        return Ok(result);
    }
    
    [HttpPost("token")]
    public async Task<IActionResult> GetTokenAsync(LoginDto model)
    {
        var result = await _userService.GetTokenAsync(model);
        SetRefreshTokenInCookies(result.RefreshToken);
        return Ok(result);
    }

    [HttpPost("addrole")]
    public async Task<ActionResult> AddRoleAsync(AddRoleDto model)
    {
        var result = await _userService.AddRoleAsync(model);
        return Ok(result);
    }
    [HttpPost("refresh-Token")]
    public async Task<ActionResult> GetRefreshToken()
    {
        var refreshToken = Request.Cookies["refreshToken"];
        var result = await _userService.RefreshTokenAsync(refreshToken);
        if (!string.IsNullOrEmpty(result.RefreshToken))
        {
            SetRefreshTokenInCookies(result.RefreshToken);
        }
        return Ok(result);
    }

    private void SetRefreshTokenInCookies(string refreshToken)
    {
        var cookiesOptions = new CookieOptions
        {
            HttpOnly = true,
            Expires = DateTime.UtcNow.AddDays(10)
        };
        Response.Cookies.Append("refreshToken", refreshToken, cookiesOptions);
    }
}

