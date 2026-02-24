using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly JWT _jwt;

    public UserService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IOptions<JWT> jwt)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _jwt = jwt.Value;
    }

    public async Task<string> RegisterAsync(UserRegistrationRequest request)
    {
        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            FirstName = request.FirstName ?? string.Empty,
            LastName = request.LastName ?? string.Empty
        };

        var userWithSameEmail = await _userManager.FindByEmailAsync(request.Email);
        if (userWithSameEmail != null)
        {
            return $"Email {request.Email} is already registered.";
        }

        var result = await _userManager.CreateAsync(user, request.Password);
        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, Authorization.default_role.ToString());
            return $"User registered with username {user.UserName}";
        }

        return string.Join(", ", result.Errors.Select(e => e.Description));
    }

    public async Task<AuthenticationResponse> GetTokenAsync(TokenRequest request)
    {
        var authenticationResponse = new AuthenticationResponse();
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            authenticationResponse.IsAuthenticated = false;
            authenticationResponse.Message = $"No Accounts Registered with {request.Email}.";
            return authenticationResponse;
        }
        if (await _userManager.CheckPasswordAsync(user, request.Password))
        {
            authenticationResponse.IsAuthenticated = true;
            JwtSecurityToken jwtSecurityToken = await CreateJwtToken(user);
            authenticationResponse.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            authenticationResponse.Email = user.Email ?? string.Empty;
            authenticationResponse.UserName = user.UserName ?? string.Empty;
            var rolesList = await _userManager.GetRolesAsync(user).ConfigureAwait(false);
            authenticationResponse.Roles = rolesList.ToList();
            return authenticationResponse;
        }
        authenticationResponse.IsAuthenticated = false;
        authenticationResponse.Message = $"Incorrect Credentials for user {user.Email}.";
        return authenticationResponse;
    }
    private async Task<JwtSecurityToken> CreateJwtToken(ApplicationUser user)
    {
        var userClaims = await _userManager.GetClaimsAsync(user);
        var roles = await _userManager.GetRolesAsync(user);

        var roleClaims = new List<Claim>();

        for (int i = 0; i < roles.Count; i++)
        {
            roleClaims.Add(new Claim("roles", roles[i]));
        }

        var claims = new[]
        {
        new Claim(JwtRegisteredClaimNames.Sub, user.UserName ?? string.Empty),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
        new Claim("uid", user.Id)
    }
        .Union(userClaims)
        .Union(roleClaims);

        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
        var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

        var jwtSecurityToken = new JwtSecurityToken(
            issuer: _jwt.Issuer,
            audience: _jwt.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwt.DurationInMinutes),
            signingCredentials: signingCredentials);
        return jwtSecurityToken;
    }

    public async Task<string> AddRoleAsync(AddRoleDto request)
{
    var user = await _userManager.FindByEmailAsync(request.Email);
    if (user == null)
    {
        return $"No Accounts Registered with {request.Email}.";
    }
    if (await _userManager.CheckPasswordAsync(user, request.Password))
    {
        var roleExists = Enum.GetNames(typeof(Authorization.Roles)).Any(x => x.ToLower() == request.Role.ToLower());
        if (roleExists)
        {
            var validRole = Enum.GetValues(typeof(Authorization.Roles)).Cast<Authorization.Roles>().FirstOrDefault(x => x.ToString().ToLower() == request.Role.ToLower());
            await _userManager.AddToRoleAsync(user, validRole.ToString());
            return $"Added {request.Role} to user {request.Email}.";
        }
        return $"Role {request.Role} not found.";
    }
    return $"Incorrect Credentials for user {user.Email}.";
}
}