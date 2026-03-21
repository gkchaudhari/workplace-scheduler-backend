namespace workplace_scheduler.Dtos
{
    public record SignupDto(string Email, string Password, string FullName, bool AgreeToTerms);
    public record LoginDto(string Email, string Password);
    public record UserDto(int Id, string Email, DateTime CreatedAt, string FullName, bool AgreeToTerms);
}
