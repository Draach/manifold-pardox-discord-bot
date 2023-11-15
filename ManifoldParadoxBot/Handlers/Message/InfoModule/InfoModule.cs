using Discord.Commands;

namespace ManifoldParadoxBot.Handlers.Message.InfoModule;

/// <summary>
/// Example base message command for future references mostly.
/// </summary>
public class InfoModule : ModuleBase<SocketCommandContext>
{
    private string helpMessage = @"
            Aquí tienes una lista de comandos disponibles para el servidor de Manifold Paradox, recuerda usarlos en el canal 'bot-comandos':
            /reservar-cupo - Reserva un cupo para cuando haya disponibilidad en el gremio para nuevos miembros.";

    [Command("help")]
    [Summary("Retrieve a list of commands and their descriptions.")]
    public Task SayAsync()
        => ReplyAsync(helpMessage);
}