namespace Shared.DTO;

public class LoginResponseDTO
{
    public string JWTToken { get; set; }
    public string RefreshToken { get; set; }
}