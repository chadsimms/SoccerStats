using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;


namespace SoccerStats
{
    class Program
    {


        static void Main(string[] args)
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            DirectoryInfo directory = new DirectoryInfo(currentDirectory);
            var fileName = Path.Combine(directory.FullName, "SoccerGameResults.csv");
            var fileContents = ReadSoccerResults(fileName);
            fileName = Path.Combine(directory.FullName, "players.json");
            var players = DeserializePlayers(fileName);
            var topTenPlayers = GetTopTenPlayers(players);
            foreach (var player in topTenPlayers)
            {
                Console.WriteLine("Name: " + player.FirstName + "\t PPG: " + player.PointsPerGame);
            }
        }

        public static string ReadFile(string fileName)
        {
            using(var reader = new StreamReader(fileName))
            {
                return reader.ReadToEnd();
            }
        }

        public static List<GameResult> ReadSoccerResults(string fileName)
        {
            var soccerResults = new List<GameResult>();
            using (var reader = new StreamReader(fileName))
            {
                string line = "";
                reader.ReadLine();
                while((line = reader.ReadLine()) != null)
                {
                    //read the .csv line by line until end
                    var gameResult = new GameResult();
                    string[] values = line.Split(',');

                    //get the date and time of the game
                    DateTime gameDate;
                    if(DateTime.TryParse(values[0], out gameDate))
                    {
                        gameResult.GameDate = gameDate;
                    }

                    //get the team name as a string
                    gameResult.TeamName = values[1];

                    //get enum value for home or away game
                    HomeOrAway homeOrAway;
                    if(Enum.TryParse(values[2], out homeOrAway))
                    {
                        gameResult.HomeOrAway = homeOrAway;
                    }

                    //get ints from .csv
                    int ParseInt;
                    if(int.TryParse(values[3], out ParseInt))
                    {
                        gameResult.Goals = ParseInt;
                    }
                    if(int.TryParse(values[4], out ParseInt))
                    {
                        gameResult.GoalAttempts = ParseInt;
                    }
                    if(int.TryParse(values[5], out ParseInt))
                    {
                        gameResult.ShotsOnGoal = ParseInt;
                    }
                    if(int.TryParse(values[6], out ParseInt))
                    {
                        gameResult.ShotsOffGoals = ParseInt;
                    }

                    //get percentage as a double
                    double possessionPercent;
                    if(double.TryParse(values[7], out possessionPercent))
                    {
                        gameResult.PossessionPercent = possessionPercent;
                    }

                    soccerResults.Add(gameResult);
                }
            }
            return soccerResults;
        }

        public static List<Player> DeserializePlayers(string fileName)
        {
            var players = new List<Player>();
            var serializer = new JsonSerializer();
            using (var reader = new StreamReader(fileName))
            using (var jsonReader = new JsonTextReader(reader))
            {
                players = serializer.Deserialize<List<Player>>(jsonReader);
            }
            
            return players;
        }

        public static List<Player> GetTopTenPlayers(List<Player> players)
        {
            var topTenPlayers = new List<Player>();
            players.Sort(new PlayerComparer());
            int counter = 0;

            foreach(var player in players)
            {
                topTenPlayers.Add(player);
                counter++;
                if(counter == 10)
                {
                    break;
                }
            }

            return topTenPlayers;
        }
    }
}
