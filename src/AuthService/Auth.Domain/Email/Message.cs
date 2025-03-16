namespace Auth.Domain.Email
{
    public record Message(IEnumerable<string> To, string Subject, string Body);
}
