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

    /// <summary>
    /// Starter Discord-botten asynkront ved å bruke den oppgitte token.
    /// </summary>
    /// <param name="token">Tokenet som brukes for å autentisere botten.</param>
    /// <returns>Task </returns>
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

    /// <summary>
    /// Sender en melding til en spesifikk Discord-kanal.
    /// </summary>
    /// <param name="message">Meldingen som skal sendes.</param>
    /// <returns>Task</returns>
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
