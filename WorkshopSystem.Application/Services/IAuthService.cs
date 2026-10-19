using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WorkshopSystem.Application.DTOs.Auth;

namespace WorkshopSystem.Application.Interfaces.Services;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(
        RegisterRequestDto request,
        CancellationToken ct);

    Task<AuthResponseDto> LoginAsync(
        LoginRequestDto request,
        CancellationToken ct);
}