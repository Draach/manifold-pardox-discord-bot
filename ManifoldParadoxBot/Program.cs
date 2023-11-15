using Discord;
using Discord.Commands;
using Discord.Net;
using Discord.WebSocket;
using Google.Cloud.Firestore;
using ManifoldParadoxBot.Handlers.Message;
using ManifoldParadoxBot.Handlers.Message.AdminModule;
using ManifoldParadoxBot.Handlers.SlashCommand;
using ManifoldParadoxBot.Services.Logging;
using Newtonsoft.Json;

namespace ManifoldParadoxBot
{
    public class Program
    {
        private DiscordSocketClient discordSocketClient;
        private CommandService commandService;
        private SlashCommandHandler slashCommandHandler;
        private MessageHandler messageHandler;
        private LoggingService loggingService;

        private FirestoreDb firestoreDb;

        private Program()
        {
            discordSocketClient = new DiscordSocketClient();
            commandService = new CommandService();
            firestoreDb = InitializeFirebaseFirestore();
            slashCommandHandler = new SlashCommandHandler(discordSocketClient, firestoreDb);
            messageHandler = new MessageHandler(discordSocketClient, commandService);
            loggingService = new LoggingService(discordSocketClient, commandService);
        }

        public static async Task Main(string[] args)
        {
            try
            {
                await new Program().RunAsync(args);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                // Handle the exception or log it.
            }
        }

        private async Task RunAsync(string[] args)
        {
            if (!ValidateBotTokenArgument(args, out string botToken))
            {
                Console.WriteLine("Invalid bot token argument.");
                return;
            }

            await InitializeDependenciesAsync(botToken);

            discordSocketClient.Ready += DiscordSocketClientReady;
            //client.UserJoined += OnUserJoined;

            // Block until cancellation is requested.
            var cts = new CancellationTokenSource();
            Console.CancelKeyPress += (s, e) =>
            {
                e.Cancel = true;
                cts.Cancel();
            };
            await Task.Delay(-1, cts.Token);
        }

        private async Task InitializeDependenciesAsync(string botToken)
        {
            slashCommandHandler.Initialize();
            loggingService.Initialize();
            await messageHandler.InstallCommandsAsync();
            await InitializeBotAsync(botToken);
        }

        private FirestoreDb InitializeFirebaseFirestore()
        {
            return FirestoreDb.Create("manifold-paradox-discord-bot");
        }

        private bool ValidateBotTokenArgument(string[] args, out string botToken)
        {
            botToken = null;
            if (args.Length < 1 || string.IsNullOrEmpty(args[0]))
            {
                Console.WriteLine("Argument cannot be null or empty: bot_token");
                return false;
            }

            botToken = args[0];
            return true;
        }

        private async Task InitializeBotAsync(string botToken)
        {
            await SetDiscordActivityAsync();
            await discordSocketClient.LoginAsync(TokenType.Bot, botToken);
            await discordSocketClient.StartAsync();
        }

        private async Task SetDiscordActivityAsync()
        {
            await discordSocketClient.SetActivityAsync(new CustomActivity
            {
                Name = "Throne and Liberty",
                Type = ActivityType.Competing,
            });
        }

        private async Task DiscordSocketClientReady()
        {
            List<ApplicationCommandProperties> applicationCommandProperties = new List<ApplicationCommandProperties>();
            try
            {
                SlashCommandBuilder enrollForWaitlist = new SlashCommandBuilder();
                enrollForWaitlist.WithName("reservar-cupo")
                    .WithDescription(
                        "Reserva un cupo para cuando haya disponibilidad en el gremio para nuevos miembros.");
                applicationCommandProperties.Add(enrollForWaitlist.Build());

                SlashCommandBuilder checkWaitlist = new SlashCommandBuilder();
                checkWaitlist.WithName("ver-waitlist")
                    .WithDescription("Consulta los jugadores en la lista de espera para entrar al gremio.");
                applicationCommandProperties.Add(checkWaitlist.Build());

                // With global commands we don't need the guild.
                await discordSocketClient.BulkOverwriteGlobalApplicationCommandsAsync(applicationCommandProperties
                    .ToArray());
            }
            catch (HttpException exception)
            {
                // If our command was invalid, we should catch an ApplicationCommandException. This exception contains the path of the error as well as the error message. You can serialize the Error field in the exception to get a visual of where your error is.
                string json = JsonConvert.SerializeObject(exception.Errors, Formatting.Indented);

                // You can send this error somewhere or just print it to the console, for this example we're just going to print it.
                Console.WriteLine(json);
            }
        }
    }
}