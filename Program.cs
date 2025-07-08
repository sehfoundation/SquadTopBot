using DSharpPlus;
using DSharpPlus.EventArgs;
using DSharpPlus.SlashCommands;
using SharpBotTopOnline.SlashCommands;

namespace SharpBotTopOnline
{
    public class Program
    {
        static void Main()
        {
            Task.Run(async () => await RunBotAsync()).Wait();
        }

        public static async Task RunBotAsync()
        {
            try
            {
                var discord = new DiscordClient(new DiscordConfiguration // create a new instance of the bot
                {
                    Token = Settings.TOKEN_BOT,
                    TokenType = TokenType.Bot,
                    AutoReconnect = true,
                    MinimumLogLevel = Microsoft.Extensions.Logging.LogLevel.Debug,
                    Intents = DiscordIntents.All
                });

                var slashCommands = discord.UseSlashCommands(); // register slash commands

                slashCommands.RegisterCommands<TopOnlineCommand>(); // toponline


                await discord.ConnectAsync();
                await Task.Delay(-1);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private static async Task Discord_MessageCreated(DiscordClient sender, MessageCreateEventArgs args)
        {

        }
    }
}