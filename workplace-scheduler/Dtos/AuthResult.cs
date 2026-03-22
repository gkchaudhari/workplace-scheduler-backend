namespace workplace_scheduler.Dtos
{
    public record AuthResult(string Token, DateTime ExpiresAt);
}
