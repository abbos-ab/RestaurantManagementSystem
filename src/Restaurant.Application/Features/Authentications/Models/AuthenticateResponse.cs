using System.Text.Json.Serialization;

namespace Restaurant.Application.Features.Authentications.Models;

[JsonDerivedType(typeof(TokenAuthenticationResponse), "Token")]
[JsonDerivedType(typeof(ChangePasswordAuthenticateResponse), "ChangePassword")]
public abstract record AuthenticateResponse;

public sealed record TokenAuthenticationResponse(string AccessToken, string RefreshToken, DateTimeOffset ExpiresAt) : AuthenticateResponse;

public sealed record ChangePasswordAuthenticateResponse(string Token) : AuthenticateResponse;