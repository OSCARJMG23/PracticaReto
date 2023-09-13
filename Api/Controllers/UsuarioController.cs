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
        return Ok(result);
    }

    [HttpPost("addrole")]
    public async Task<ActionResult> AddRoleAsync(AddRoleDto model)
    {
        var result = await _userService.AddRoleAsync(model);
        return Ok(result);
    }
    [HttpPost("refreshToken")]
    public async Task<ActionResult> GetRefreshToken(DatosUsuarioDto model)
    {
        var result = await _userService.RefreshTokenAsync(model.RefreshToken);
        return Ok(result);
    }
}

