using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Api.Dtos;
using Api.Helpers;
using Dominio.Entities;
using Dominio.Interfaces;
using iText.StyledXmlParser.Jsoup.Parser;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Api.Services;

    public class UserService : IUserService
    {
        private readonly JWT _jWT;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher<User> _passwordHasher;

        public UserService(IUnitOfWork unitOfWork, IOptions<JWT>jwt, IPasswordHasher<User> passwordHasher)
        {
            _jWT = jwt.Value;
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
        }

        public async Task<string> RegisterAsync(RegisterDto registerDto)
        {
            var usuario = new User
            {
                UserEmail =registerDto.Email,
                UserName = registerDto.Username
            };
            usuario.Password = _passwordHasher.HashPassword(usuario, registerDto.Password);
            var usuarioExiste = _unitOfWork.Users
                                                .Find(u=> u.UserName.ToLower() == registerDto.Username.ToLower())
                                                .FirstOrDefault();

            if(usuarioExiste == null)
            {
                try
                {
                    _unitOfWork.Users.Add(usuario);
                    await _unitOfWork.SaveAsync();

                    return $"El usuario {registerDto.Username} ha sido creado exitisamente";
                }
                catch (Exception ex)
                {
                    var message = ex.Message;
                    return $"Error{message}";
                    
                }
            }
            else
            {
                return $"El usuario {registerDto.Username} ya se encuentra registrado";
            }
        }
        public async Task<string> AddRoleAsync(AddRoleDto model)
        {
            var usuario = await _unitOfWork.Users
                                .GetByUserNameAsync(model.Username);

            if(usuario ==null)
            {
                return $"No existe algun usuario registrado con la cuenta, olvido alguna caracter {model.Username}";
            }
            var resultado = _passwordHasher.VerifyHashedPassword(usuario, usuario.Password, model.Password);

            if(resultado == PasswordVerificationResult.Success)
            {
                var rolExiste = _unitOfWork.Rols
                                            .Find(u=>u.Nombre.ToLower() == model.Rol.ToLower())
                                            .FirstOrDefault();
                if(rolExiste != null)
                {
                    var usuarioTieneRol = usuario.Rols.Any(u =>u.Id == rolExiste.Id);

                    if(usuarioTieneRol == false)
                    {
                        usuario.Rols.Add(rolExiste);
                        _unitOfWork.Users.Update(usuario);
                        await _unitOfWork.SaveAsync();
                    }
                    return $"Rol {model.Rol} agregado a la cuenta {model.Username} de forma exitosa";
                }
                return $"Rol {model.Rol} no encontrado";
            }
            return $"Credenciales incorrectas para el usuario {usuario.UserName}";
        }
        public async Task<DatosUsuarioDto> GetTokenAsync(LoginDto model)
        {
            DatosUsuarioDto datosUsuarioDto = new DatosUsuarioDto();
            var usuario = await _unitOfWork.Users
                            .GetByUserNameAsync(model.Username);
        
            if(usuario == null)
            {
                datosUsuarioDto.EstaAutenticado = false;
                datosUsuarioDto.Mensaje =$"No existe ningun usuario con el username {model.Username}";
                return datosUsuarioDto;
            }
            var resultado = _passwordHasher.VerifyHashedPassword(usuario, usuario.Password, model.Password);
            if(resultado == PasswordVerificationResult.Success)
            {
                datosUsuarioDto.Mensaje ="Ok";
                datosUsuarioDto.EstaAutenticado = true;
                if(usuario != null)
                {
                    JwtSecurityToken jwtSecurityToken = CreateJwtToken(usuario);
                    datosUsuarioDto.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
                    datosUsuarioDto.Username = usuario.UserName;
                    datosUsuarioDto.Email = usuario.UserEmail;
                    datosUsuarioDto.Roles = usuario.Rols
                                                    .Select(p=>p.Nombre)
                                                    .ToList();
                    if (usuario.RefreshTokens.Any(a => a.IsActive))
                    {
                        var activeRefreshToken = usuario.RefreshTokens.Where(a=>a.IsActive ==true).FirstOrDefault();
                        datosUsuarioDto.RefreshToken = activeRefreshToken.Token;
                        datosUsuarioDto.RefreshTokenExpiration = activeRefreshToken.Expires;
                    }
                    else
                    {
                        var refreshToken = CreateRefreshToken();
                        datosUsuarioDto.RefreshToken = refreshToken.Token;
                        datosUsuarioDto.RefreshTokenExpiration = refreshToken.Expires;
                        usuario.RefreshTokens.Add(refreshToken);
                        _unitOfWork.Users.Update(usuario);
                        await _unitOfWork.SaveAsync();
                    }
                    return datosUsuarioDto;
                }
                else
                {
                    datosUsuarioDto.EstaAutenticado = false;
                    datosUsuarioDto.Mensaje = $"Credenciales incorrectas para el usuario {usuario.UserName}";
                    return datosUsuarioDto;
                }
            }
            datosUsuarioDto.EstaAutenticado = false;
            datosUsuarioDto.Mensaje =$"Credenciales incorrectas para el usuario {usuario.UserName}";
            return datosUsuarioDto;
        }
        private JwtSecurityToken CreateJwtToken(User usuario)
        {
            if(usuario == null)
            {
                throw new ArgumentNullException(nameof(usuario), "El usuario no puede ser nulo");
            }
            var roles = usuario.Rols;
            var roleClaims = new List<Claim>();
            foreach (var role in roles)
            {
                roleClaims.Add(new Claim("roles", role.Nombre));
            }
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, usuario.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, usuario.UserEmail),
                new Claim("uid", usuario.Id.ToString())
            }
            .Union(roleClaims);
            if(string.IsNullOrEmpty(_jWT.Key) || string.IsNullOrEmpty(_jWT.Issuer)|| string.IsNullOrEmpty(_jWT.Audience))
            {
                throw new ArgumentException("La condiguracion del JWT es nula o vacia");
            }
            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jWT.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256Signature);
            var JwtSecurityToken = new JwtSecurityToken(
                issuer: _jWT.Issuer,
                audience: _jWT.Audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(_jWT.DurationInMinutes),
                signingCredentials: signingCredentials);
            return JwtSecurityToken;
        }
        public async Task<DatosUsuarioDto> RefreshTokenAsync(string refreshToken)
        {
            var datausuarioDto = new DatosUsuarioDto();
            var usuario = await _unitOfWork.Users
                            .GetByRefreshTokenAsync(refreshToken);

            if (usuario == null)
            {
                datausuarioDto.EstaAutenticado = false;
                datausuarioDto.Mensaje = $"Token is not assigned to any user.";
                return datausuarioDto;
            }
            var refreshTokenBd = usuario.RefreshTokens.Single(x => x.Token == refreshToken );

            if (!refreshTokenBd.IsActive)
            {
                datausuarioDto.EstaAutenticado = false;
                datausuarioDto.Mensaje = $"Token is not active.";
                return datausuarioDto;
            }

            refreshTokenBd.Revoked = DateTime.UtcNow;
            var newRefreshToken = CreateRefreshToken();
            usuario.RefreshTokens.Add(newRefreshToken);
            _unitOfWork.Users.Update(usuario);
            await _unitOfWork.SaveAsync();
            datausuarioDto.EstaAutenticado = true;
            JwtSecurityToken jwtSecurityToken = CreateJwtToken(usuario);
            datausuarioDto.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            datausuarioDto.Username = usuario.UserName;
            datausuarioDto.Email = usuario.UserEmail;
            datausuarioDto.Roles = usuario.Rols
                                                .Select(u => u.Nombre)
                                                .ToList();
            datausuarioDto.RefreshToken = newRefreshToken.Token;
            datausuarioDto.RefreshTokenExpiration = newRefreshToken.Expires;
            return datausuarioDto;
        }
        private RefreshToken CreateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var generator = RandomNumberGenerator.Create())
            {
                generator.GetBytes(randomNumber);
                return new RefreshToken
                {
                    Token = Convert.ToBase64String(randomNumber),
                    Expires = DateTime.UtcNow.AddDays(10),
                    Created = DateTime.UtcNow
                };
            }
        }
    }
