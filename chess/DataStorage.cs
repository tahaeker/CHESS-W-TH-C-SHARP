using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections.Generic;
using System.Text.Json;


namespace chess
{
    internal class DataStorage
    {
        
        public static void SaveMoveHistory(List<Move> moves, string filePath)
        {
            // Serialize the list of moves to JSON and save to file:JsonSerializer.Serialize
            string json = JsonSerializer.Serialize(moves,new JsonSerializerOptions { WriteIndented = true});//görünümü güzel yapar
            File.WriteAllText(filePath, json);

        }

        public static List<Move> LoadMoveHistory(string filePath)
        {
            if (!File.Exists(filePath))
                return new List<Move>();

            string json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<List<Move>>(json) ?? new List<Move>();// eğer sol taraf nullsa sağ tarafı kullan
        }


        public static void SavePlayers(Player white,Player black, string filePath)
        {
           var players = new List<Player> {white,black };
            var json = JsonSerializer.Serialize(players, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, json);

        }

        public static (Player,Player) LoadPlayers(string filePath)
        {

            if (!File.Exists(filePath)) 
                return (new Player( "defaultWhite", true ),new Player("defaultBlack", false));

            var json = File.ReadAllText(filePath);
            var players = JsonSerializer.Deserialize<List<Player>>(json);

            if(players == null || players.Count < 2)
                return (new Player("defaultWhite", true), new Player("defaultBlack", false));

            return (players[0], players[1]);
        }


    }
}
