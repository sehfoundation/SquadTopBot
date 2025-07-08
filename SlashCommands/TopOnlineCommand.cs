using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace SharpBotTopOnline.SlashCommands
{
    public class TopOnlineCommand : ApplicationCommandModule
    {
        [SlashCommand("top", "Топ 100 онлайн за поточний місяць (нік + час)")]
        public static async Task TopCommand(InteractionContext ctx)
        {
            await HandleTopCommand(ctx, isCurrentMonth: true, isAdmin: false);
        }

        [SlashCommand("topad", "Топ 100 онлайн за поточний місяць (нік + час + SteamID)")]
        public static async Task TopAdminCommand(InteractionContext ctx)
        {
            await HandleTopCommand(ctx, isCurrentMonth: true, isAdmin: true);
        }

        [SlashCommand("toppr", "Топ 100 онлайн за попередній місяць (нік + час + SteamID)")]
        public static async Task TopPreviousMonthCommand(InteractionContext ctx)
        {
            await HandleTopCommand(ctx, isCurrentMonth: false, isAdmin: true);
        }

        private static async Task HandleTopCommand(InteractionContext ctx, bool isCurrentMonth, bool isAdmin)
        {
            var players_list = await Parser.FetchAndParseLeaderboard(isAdmin, isCurrentMonth);
            string leaderboard_message = "";

            for (int i = 0; i < players_list.Count; i++)
            {
                var player = players_list[i];
                string line = $"{i + 1}. **{player.Name}**: {Tools.FormatTime(player.Value)}";
                if (isAdmin)
                {
                    line += $" — {player.SteamID}";
                }
                leaderboard_message += line + "\n";
            }

            var embed = new DiscordEmbedBuilder
            {
                Title = "Top 100 Online — SQUAD UKRAINE",
                Description = leaderboard_message,
                Color = DiscordColor.Blue
            }.Build();

            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder().AddEmbed(embed));
        }
    }
}
