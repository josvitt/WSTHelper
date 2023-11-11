using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Utils;
using System.Text.RegularExpressions;

namespace WSTHelper;
public class WSTHelper : BasePlugin
{
    Dictionary<string, Vector[]> wstSpawns = new Dictionary<string, Vector[]>();
    public override string ModuleName => "WSTHelper";
    public override string ModuleVersion => "0.6";
    public override string ModuleAuthor => "dustOff";

    public override void Load(bool hotReload)
    {
        wstSpawns["surf_beginner"] = new Vector[] { new Vector(-129, -15, 384) };
        wstSpawns["surf_utopia_njv"] = new Vector[] { new Vector(-13840, -464, 12864) };
        wstSpawns["surf_mesa_revo"] = new Vector[] { new Vector(64, -992, 8992) };
        wstSpawns["surf_kitsune"] = new Vector[] { new Vector(112, -1216, 160) };

        Console.WriteLine("WSTHelper Loaded");
    }

    class LeaderboardParser
    {
        public Dictionary<string, double> ParseLeaderboard(string filePath)
        {
            Dictionary<string, double> leaderboardData = new Dictionary<string, double>();

            try
            {
                string fileContent = File.ReadAllText(filePath);
                var matches = Regex.Matches(fileContent, "\"time\"\\s+\"([^\"]+)\"\\s+\"name\"\\s+\"([^\"]+)\"");

                foreach (Match match in matches)
                {
                    string username = match.Groups[2].Value;
                    double seconds;

                    if (double.TryParse(match.Groups[1].Value, out seconds))
                    {
                        leaderboardData.Add(username, seconds);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

            return leaderboardData;
        }
    }

    [ConsoleCommand("reset", "Reset to initial spawn.")]
    public void OnCommandReset(CCSPlayerController? player, CommandInfo command)
    {
        if (player == null)
        {
            return;
        }
        string mapName = Server.MapName;
        Vector[] spawnPoints = null;

        if (wstSpawns.ContainsKey(mapName))
        {
            spawnPoints = wstSpawns[mapName];
            if (spawnPoints.Length > 0)
            {
                player.PlayerPawn.Value.Teleport(spawnPoints[0], new QAngle(0, 90, 0), new Vector());
            }
            else
            {
                Console.WriteLine($"No spawn points defined for map: {mapName}");
            }
        }
        else
        {
            Console.WriteLine($"Spawn points not defined for map: {mapName}");
        }
    }

    [ConsoleCommand("r", "Reset to initial spawn.")]
    public void OnCommandR(CCSPlayerController? player, CommandInfo command)
    {
        if (player == null)
        {
            return;
        }
        string mapName = Server.MapName;
        Vector[] spawnPoints = null;

        if (wstSpawns.ContainsKey(mapName))
        {
            spawnPoints = wstSpawns[mapName];
            if (spawnPoints.Length > 0)
            {
                player.PlayerPawn.Value.Teleport(spawnPoints[0], new QAngle(0, 90, 0), new Vector());
            }
            else
            {
                Console.WriteLine($"No spawn points defined for map: {mapName}");
            }
        }
        else
        {
            Console.WriteLine($"Spawn points not defined for map: {mapName}");
        }
    }

    [ConsoleCommand("top", "Report top 5 times.")]
    public void OnCommandTop(CCSPlayerController? player, CommandInfo command)
    {
        if (player == null)
        {
            return;
        }
        string recordsFolder = "/home/steam/cs2-ds/game/csgo/scripts/wst_records";
        string fileName = ($"{Server.MapName}.txt");
        string filePath = Path.Combine(recordsFolder, fileName);
        LeaderboardParser leaderboardParser = new LeaderboardParser();
        Dictionary<string, double> leaderboardData = leaderboardParser.ParseLeaderboard(filePath);

        if (leaderboardData.Count > 0)
        {
            var lowestTimes = leaderboardData.OrderBy(kvp => kvp.Value).Take(5);
            player.PrintToChat("Top 5 Times:");
            foreach (var kvp in lowestTimes)
            {
                player.PrintToChat($"{kvp.Key}, Time: {kvp.Value}");
            }
        }
        else
        {
            player.PrintToChat("No data found in the leaderboard.");
        }
    }
}