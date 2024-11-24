using Discord.WebSocket;
using Discord;
using kartverketprosjekt.Data;

public class DiscordBot : IDiscordBot
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
            // Logger inn botten med gitt token.
            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync(); // Starter botten.
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error starting bot: {ex.Message}");
        }
    }

    // Metode for å sende en melding til en spesifikk Discord-kanal.
    public async Task SendMessageToDiscord(string message)
    {
        var channel = _client.GetChannel(1299414535555518494) as ITextChannel; // GetChannel(ID til kanalen)

        // Sjekker om kanalen finnes.
        if (channel != null)
        {
            await channel.SendMessageAsync(message);
        }
        else
        {
            Console.WriteLine("Kunne ikke finne kanalen du har angitt.");
        }
    }
}
