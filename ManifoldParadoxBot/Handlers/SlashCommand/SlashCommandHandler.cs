using System.Globalization;
using System.Text;
using Discord;
using Discord.WebSocket;
using Google.Cloud.Firestore;

namespace ManifoldParadoxBot.Handlers.SlashCommand;

internal class SlashCommandHandler
{
    private DiscordSocketClient discordSocketClient;
    private FirestoreDb firestoreDb;

    public SlashCommandHandler(DiscordSocketClient discordSocketClient, FirestoreDb firestoreDb)
    {
        this.discordSocketClient = discordSocketClient;
        this.firestoreDb = firestoreDb;
    }

    public void Initialize()
    {
        discordSocketClient.SlashCommandExecuted += HandleSlashCommands;
    }

    private async Task HandleSlashCommands(SocketSlashCommand command)
    {
        switch (command.Data.Name)
        {
            case "reservar-cupo":
                await EnrollForWaitlist(command);
                break;
            case "ver-waitlist":
                await CheckWaitlist(command);
                break;
            default:
                await command.RespondAsync("Lo siento, parece que el comando ingresado no es válido.");
                break;
        }
    }

    private async Task EnrollForWaitlist(SocketSlashCommand command)
    {
        try
        {
            SocketUser socketUser = command.User;

            Console.WriteLine(
                $"Mention: {socketUser.Mention}, GlobalName {socketUser.GlobalName}, Username: {socketUser.Username}");

            DocumentReference docRef = firestoreDb.Collection("waitlist").Document(socketUser.Username);
            DocumentSnapshot documentSnapshot = await docRef.GetSnapshotAsync();
            if (!documentSnapshot.Exists)
            {
                DateTime utcDateTime = DateTime.UtcNow;

                WaitlistUser waitlistUser = new WaitlistUser()
                {
                    Mention = socketUser.Mention,
                    GlobalName = socketUser.GlobalName,
                    UserName = socketUser.Username,
                    DateTime = utcDateTime.ToString("yyyy-MM-ddTHH:mm:ss.fffffffZ")
                };

                await docRef.SetAsync(waitlistUser);

                await command.RespondAsync(
                    $"Te has registrado en la lista de espera con éxito. Un lider u oficial del gremio te contactará cuando haya cupos disponibles.");
            }
            else
            {
                await command.RespondAsync(
                    "Ya te has registrado en la lista de espera anteriormente. Un lider u oficial del gremio te contactará cuando haya cupos disponibles.");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            await command.RespondAsync(
                "Ha ocurrido un error interno en el servidor. Intenta nuevamente, o contacta con un administrador.");
        }
    }

    private async Task CheckWaitlist(SocketSlashCommand command)
    {
        try
        {
            SocketGuildUser user = command.User as SocketGuildUser;

            // Check if the user has a specific role (Manifold Paradox Bot role).
            ulong roleIdToCheck = 1174122709416943658;
            var hasPrivilegeRole = user.Roles.Any(r => r.Id == roleIdToCheck);

            if (hasPrivilegeRole)
            {
                Query waitlistQuery = firestoreDb.Collection("waitlist");
                QuerySnapshot waitlistSnapshots = await waitlistQuery.GetSnapshotAsync();
                if (waitlistSnapshots.Count == 0)
                {
                    await command.RespondAsync("No hay usuarios en la lista de espera.");
                }
                else
                {
                    StringBuilder stringBuilder = new StringBuilder();

                    foreach (DocumentSnapshot waitlistSnapshot in waitlistSnapshots.Documents)
                    {
                        Dictionary<string, object> waitlistUser = waitlistSnapshot.ToDictionary();

                        foreach (KeyValuePair<string, object> pair in waitlistUser)
                        {
                            if (pair.Key == "DateTime" && DateTime.TryParseExact(pair.Value.ToString(),
                                    "yyyy-MM-ddTHH:mm:ss.fffffffZ", CultureInfo.InvariantCulture,
                                    DateTimeStyles.AssumeUniversal, out DateTime convertedDateTime))
                            {
                                stringBuilder.Append($"{pair.Key}: {convertedDateTime}");
                            }
                            else
                            {
                                stringBuilder.Append($"{pair.Key}: {pair.Value} | ");
                            }
                        }

                        stringBuilder.Append('\n');
                    }

                    await command.RespondAsync(stringBuilder.ToString());
                }
            }
            else
            {
                await command.RespondAsync("Tienes que ser administrador para ver la lista de espera.");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            await command.RespondAsync("Ha ocurrido un error interno en el servidor.");
        }
    }
}