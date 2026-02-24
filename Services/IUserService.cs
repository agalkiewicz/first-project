public interface IUserService
{
    Task<string> RegisterAsync(UserRegistrationRequest request);
    Task<AuthenticationResponse> GetTokenAsync(TokenRequest model);
}
