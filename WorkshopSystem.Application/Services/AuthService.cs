using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WorkshopSystem.Application.DTOs.Auth;
using WorkshopSystem.Application.Exceptions;
using WorkshopSystem.Application.Interfaces.Repositories;
using WorkshopSystem.Application.Interfaces.Services;
using WorkshopSystem.Application.Options;
using WorkshopSystem.Domain.Entities;
using WorkshopSystem.Domain.Enums;

namespace WorkshopSystem.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly PasswordHasher<User> _passwordHasher;
    private readonly JwtOptions _jwtOptions;

    public AuthService(
        IUserRepository userRepository,
        PasswordHasher<User> passwordHasher,
        IOptions<JwtOptions> jwtOptions)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtOptions = jwtOptions.Value;
    }

    public async Task<AuthResponseDto> RegisterAsync(
        RegisterRequestDto dto,
        CancellationToken ct)
    {
        if (dto.Password != dto.ConfirmPassword)
            throw new BusinessException("Passwords do not match.");

        if (await _userRepository.EmailExistsAsync(dto.Email, ct))
            throw new ConflictException("Email already exists.");

        var user = new User
        {
            Name = dto.Name,
            Email = dto.Email,
            Role = UserRole.User
        };

        user.PasswordHash =
            _passwordHasher.HashPassword(user, dto.Password);

        await _userRepository.AddAsync(user, ct);
        await _userRepository.SaveChangesAsync(ct);

        return CreateAuthResponse(user);
    }

    public async Task<AuthResponseDto> LoginAsync(
        LoginRequestDto dto,
        CancellationToken ct)
    {
        var user = await _userRepository.GetByEmailAsync(dto.Email, ct);

        if (user == null)
            throw new UnauthorizedException("Invalid email or password.");

        var result = _passwordHasher.VerifyHashedPassword(
            user,
            user.PasswordHash,
            dto.Password);

        if (result == PasswordVerificationResult.Failed)
            throw new UnauthorizedException("Invalid email or password.");

        return CreateAuthResponse(user);
    }

    private AuthResponseDto CreateAuthResponse(User user)
    {
        var claims = new[]
        {
            new Claim(
                ClaimTypes.NameIdentifier,
                user.Id.ToString()),

            new Claim(
                ClaimTypes.Email,
                user.Email),

            new Claim(
                ClaimTypes.Role,
                user.Role.ToString())
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_jwtOptions.Key));

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(
                _jwtOptions.ExpirationMinutes),
            signingCredentials: credentials);

        return new AuthResponseDto
        {
            Token = new JwtSecurityTokenHandler()
                .WriteToken(token),

            UserName = user.Name,
            Email = user.Email,
            Role = user.Role.ToString()
        };
    }
}