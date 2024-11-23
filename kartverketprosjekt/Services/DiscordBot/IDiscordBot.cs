public interface IDiscordBot
{
    Task StartAsync(string token);
    Task SendMessageToDiscord(string message);
}
