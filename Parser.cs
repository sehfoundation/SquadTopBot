using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;

namespace SharpBotTopOnline
{
    public class Player(string name, long id, int value)
    {
        public string Name { get; set; } = name;
        public long ID { get; set; } = id;
        public int Value { get; set; } = value;
        public long SteamID { get; set; }

        public async Task FetchSteamIDAsync()
        {
            string url = $"https://api.battlemetrics.com/players/{ID}?include=identifier&filter[identifiers]=steamID";

            using HttpClient client = new ();

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Settings.TOKEN_BM}");

            try
            {
                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var data = JObject.Parse(jsonResponse);
                    var identifiers = data["included"];

                    if (identifiers != null)
                    {
                        foreach (var identifier in identifiers)
                        {
                            string identifierValue = identifier["attributes"]?["identifier"]?.ToString() ?? "";
                            if (identifier["type"]?.ToString() == "identifier" &&
                                identifier["attributes"]?["type"]?.ToString() == "steamID")
                            {
                                if (long.TryParse(identifierValue, out long s_id))
                                {
                                    SteamID = s_id;
                                }
                                else
                                {
                                    Console.WriteLine($"Identifier '{identifierValue}' cannot be parsed as long.");
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }

    public static class Parser
    {
        public static async Task<List<Player>> FetchAndParseLeaderboard(bool isAdmin = false, bool isCurrentMonth = true)
        {
            string urlSq1 = $"https://api.battlemetrics.com/servers/{Settings.SERVER_ID_SQ_1}/relationships/leaderboards/time";
            string urlSq2 = $"https://api.battlemetrics.com/servers/{Settings.SERVER_ID_SQ_2}/relationships/leaderboards/time";
            //string urlSq3 = $"https://api.battlemetrics.com/servers/{Settings.SERVER_ID_SQ_3}/relationships/leaderboards/time";
            int pageSize = isAdmin ? 100 : 100; //30 : 100
            string period = isCurrentMonth ? Tools.GetPeriod() : Tools.GetPreviousMonthPeriod();
            Console.WriteLine(period);

            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Settings.TOKEN_BM}");

            var parameters = new Dictionary<string, string>
            {
                { "page[size]", pageSize.ToString() },
                { "filter[period]", period }
            };

            string queryString = string.Join("&", parameters.Select(p => $"{p.Key}={p.Value}"));

            List<Player> players = [];

            await FetchPlayersFromServer(client, $"{urlSq1}?{queryString}", players);
            await FetchPlayersFromServer(client, $"{urlSq2}?{queryString}", players);
            //await FetchPlayersFromServer(client, $"{urlSq3}?{queryString}", players);

            players = await RemoveDuplicatePlayersAsync(players);

            if (isAdmin)
            {
                int requestsCount = 0;
                foreach (var player in players)
                {
                    await player.FetchSteamIDAsync();
                    requestsCount++;
                    if (requestsCount >= 30)
                    {
                        await Task.Delay(1000);
                        requestsCount = 0;
                    }
                }
            }

            var sortedPlayers = players.OrderByDescending(x => x.Value).ToList();
            return isAdmin ? sortedPlayers.Take(100).ToList() : sortedPlayers.Take(100).ToList();
        }

        public static async Task<List<Player>> RemoveDuplicatePlayersAsync(List<Player> players)
        {
            var uniquePlayers = new Dictionary<long, Player>();

            await Task.Run(() =>
            {
                foreach (var player in players)
                {
                    if (uniquePlayers.TryGetValue(player.ID, out var existingPlayer))
                    {
                        existingPlayer.Value += player.Value;
                    }
                    else
                    {
                        uniquePlayers[player.ID] = player;
                    }
                }
            });

            return [.. uniquePlayers.Values];
        }

        private static async Task FetchPlayersFromServer(HttpClient client, string url, List<Player> players)
        {
            try
            {
                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsStringAsync();
                    var data = JObject.Parse(responseData);
                    var users = data["data"];

                    if (users != null)
                    {
                        foreach (var userData in users)
                        {
                            string strID = userData["id"]?.ToString() ?? "";
                            string name = userData["attributes"]?["name"]?.ToString() ?? "";
                            if (long.TryParse(strID, out long id))
                            {
                                string strVal = userData["attributes"]?["value"]?.ToString() ?? "";
                                if (int.TryParse(strVal, out int value))
                                {
                                    var player = new Player(name, id, value);
                                    players.Add(player);
                                }
                            }
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"Failed to fetch data from {url}. Status code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching data from {url}: {ex.Message}");
            }
        }
    }
}