using Discord.WebSocket;
using Discord;
using kartverketprosjekt.Data;

public class DiscordBot
{
    private readonly DiscordSocketClient _client;

    public DiscordBot(DiscordSocketClient client)
    {
        _client = client;
    }

    public async Task StartAsync(string token)
    {
        try
        {
            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error starting bot: {ex.Message}");
        }
    }

    public async Task SendMessageToDiscord(string message)
    {
        var channel = _client.GetChannel(1299414535555518494) as ITextChannel; // Replace with your channel ID
        if (channel != null)
        {
            await channel.SendMessageAsync(message);
        }
        else
        {
            Console.WriteLine("Could not find the specified text channel.");
        }
    }
}
