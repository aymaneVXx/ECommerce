using AutoMapper;
using ECommerce.Application.DTOs.Identity;
using ECommerce.Application.Interfaces.Identity;
using ECommerce.Application.Responses;
using ECommerce.Application.Services.Authentication;
using FluentValidation;

namespace ECommerce.Application.Services.Implementations;

public class AuthenticationService : IAuthenticationService
{
    private readonly ITokenManagement _tokenManagement;
    private readonly IUserManagement _userManagement;
    private readonly IRoleManagement _roleManagement;
    private readonly IMapper _mapper;

    private readonly IValidator<CreateUser> _createUserValidator;
    private readonly IValidator<LoginUser> _loginUserValidator;

    public AuthenticationService(
        ITokenManagement tokenManagement,
        IUserManagement userManagement,
        IRoleManagement roleManagement,
        IMapper mapper,
        IValidator<CreateUser> createUserValidator,
        IValidator<LoginUser> loginUserValidator)
    {
        _tokenManagement = tokenManagement;
        _userManagement = userManagement;
        _roleManagement = roleManagement;
        _mapper = mapper;
        _createUserValidator = createUserValidator;
        _loginUserValidator = loginUserValidator;
    }

    public async Task<ServiceResponse> CreateUserAsync(CreateUser model)
    {
        var validation = await _createUserValidator.ValidateAsync(model);
        if (!validation.IsValid)
            return ServiceResponse.Fail(validation.Errors.First().ErrorMessage);

        // Compter AVANT création => savoir si c'est le 1er user
        var usersBefore = await _userManagement.GetAllUsers();
        var isFirstUser = usersBefore is null || !usersBefore.Any();

        // Créer user
        var user = _mapper.Map<ECommerce.Domain.Identity.AppUser>(model);
        user.UserName = model.Email;

        var created = await _userManagement.CreateUser(user, model.Password);
        if (!created)
            return ServiceResponse.Fail("User already exists.");

        // Recharger depuis la DB 
        var savedUser = await _userManagement.GetUserByEmail(model.Email);
        if (savedUser is null)
            return ServiceResponse.Fail("User created but cannot be loaded.");

        // Assigner rôle 
        var roleToAssign = isFirstUser ? "Admin" : "User";

        var addedToRole = await _roleManagement.AddUserToRole(savedUser, roleToAssign);
        if (!addedToRole)
            return ServiceResponse.Fail("User created but role assignment failed.");

        return ServiceResponse.Ok("Account created successfully.");
    }


    public async Task<LoginResponse> LoginUserAsync(LoginUser model)
    {
        var validation = await _loginUserValidator.ValidateAsync(model);
        if (!validation.IsValid)
            return new LoginResponse(false, validation.Errors.First().ErrorMessage);

        var user = await _userManagement.GetUserByEmail(model.Email);
        if (user is null)
            return new LoginResponse(false, "User not found.");

        var loggedIn = await _userManagement.LoginUser(user, model.Password);
        if (!loggedIn)
            return new LoginResponse(false, "Invalid credentials.");

        var claims = await _userManagement.GetUserClaims(user.Email!);
        if (claims.Count == 0)
            return new LoginResponse(false, "Unable to generate claims.");

        var token = _tokenManagement.GenerateToken(claims);
        var refreshToken = _tokenManagement.GenerateRefreshToken();

        await _tokenManagement.UpdateRefreshToken(user.Id, refreshToken);

        return new LoginResponse(true, "Login successful.", token, refreshToken);
    }

    public async Task<LoginResponse> ReviveTokenAsync(string refreshToken)
    {
        var valid = await _tokenManagement.ValidateRefreshToken(refreshToken);
        if (!valid)
            return new LoginResponse(false, "Invalid refresh token.");

        var userId = await _tokenManagement.GetUserIdByRefreshToken(refreshToken);
        if (string.IsNullOrWhiteSpace(userId))
            return new LoginResponse(false, "Invalid refresh token.");

        var user = await _userManagement.GetUserById(userId);
        if (user is null)
            return new LoginResponse(false, "User not found.");

        var claims = await _userManagement.GetUserClaims(user.Email!);
        if (claims.Count == 0)
            return new LoginResponse(false, "Unable to generate claims.");

        var newToken = _tokenManagement.GenerateToken(claims);
        var newRefreshToken = _tokenManagement.GenerateRefreshToken();

        await _tokenManagement.UpdateRefreshToken(user.Id, newRefreshToken);

        return new LoginResponse(true, "Token revived.", newToken, newRefreshToken);
    }
}
