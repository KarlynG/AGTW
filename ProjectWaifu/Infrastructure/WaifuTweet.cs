namespace ProjectWaifu.Infrastructure;

public class WaifuTweet
{
    public Guid Id { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime CreationDate { get; set; }
}
