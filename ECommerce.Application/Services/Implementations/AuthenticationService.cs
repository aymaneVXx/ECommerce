using AutoMapper;
using ECommerce.Application.DTOs.Identity;
using ECommerce.Application.Interfaces.Identity;
using ECommerce.Application.Responses;
using ECommerce.Application.Services.Authentication;
using ECommerce.Application.Services.Logging;
using ECommerce.Application.Services.Validations;
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
    private readonly IValidationService _validationService;

    // Logger générique 
    private readonly IApplicationLogger<AuthenticationService> _logger;

    public AuthenticationService(
        ITokenManagement tokenManagement,
        IUserManagement userManagement,
        IRoleManagement roleManagement,
        IMapper mapper,
        IValidator<CreateUser> createUserValidator,
        IValidator<LoginUser> loginUserValidator,
        IValidationService validationService,
        IApplicationLogger<AuthenticationService> logger)
    {
        _tokenManagement = tokenManagement;
        _userManagement = userManagement;
        _roleManagement = roleManagement;
        _mapper = mapper;

        _createUserValidator = createUserValidator;
        _loginUserValidator = loginUserValidator;
        _validationService = validationService;
        _logger = logger;
    }

    public async Task<ServiceResponse> CreateUserAsync(CreateUser model)
    {
        // Validation via service (concatène les erreurs )
        var validation = await _validationService.ValidateAsync(model, _createUserValidator);
        if (!validation.Success)
            return validation;

        // Compter AVANT création => savoir si c'est le 1er user
        var usersBefore = await _userManagement.GetAllUsers();
        var isFirstUser = usersBefore is null || !usersBefore.Any();

        // Mapper -> AppUser
        var user = _mapper.Map<ECommerce.Domain.Identity.AppUser>(model);
        user.UserName = model.Email;

        // Créer user
        var created = await _userManagement.CreateUser(user, model.Password);
        if (!created)
            return ServiceResponse.Fail("Email address might be already in use or unknown error occurred.");

        // Recharger depuis la DB
        var savedUser = await _userManagement.GetUserByEmail(model.Email);
        if (savedUser is null)
            return ServiceResponse.Fail("Error occurred while creating account.");

        // Assigner rôle (1er = Admin, sinon User)
        var roleToAssign = isFirstUser ? "Admin" : "User";

        var addedToRole = await _roleManagement.AddUserToRole(savedUser, roleToAssign);
        if (!addedToRole)
        {
            // rollback : si pas de rôle -> on supprime l'utilisateur
            var removed = await _userManagement.RemoveUserByEmail(savedUser.Email!);
            if (removed <= 0)
            {
                _logger.LogWarning($"User with email '{savedUser.Email}' failed to be removed after role assignment failure.");
            }

            _logger.LogWarning($"Role assignment failed for user '{savedUser.Email}'. User removed.");
            return ServiceResponse.Fail("Error occurred while creating account.");
        }

        return ServiceResponse.Ok("Account created successfully.");
    }

    public async Task<LoginResponse> LoginUserAsync(LoginUser model)
    {
        // Validation via service
        var validation = await _validationService.ValidateAsync(model, _loginUserValidator);
        if (!validation.Success)
            return new LoginResponse(false, validation.Message);

        var user = await _userManagement.GetUserByEmail(model.Email);
        if (user is null)
            return new LoginResponse(false, "Invalid credentials.");

        // refuser login si aucun rôle
        var role = await _roleManagement.GetUserRole(user.Email!);
        if (string.IsNullOrWhiteSpace(role))
            return new LoginResponse(false, "Invalid credentials.");

        var loggedIn = await _userManagement.LoginUser(user, model.Password);
        if (!loggedIn)
            return new LoginResponse(false, "Invalid credentials.");

        var claims = await _userManagement.GetUserClaims(user.Email!);
        if (claims.Count == 0)
            return new LoginResponse(false, "Unable to generate claims.");

        var token = _tokenManagement.GenerateToken(claims);
        var refreshToken = _tokenManagement.GenerateRefreshToken();

        // Upsert refresh token
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
