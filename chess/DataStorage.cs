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
        public static void UndoMove(ChessContext ctx)
        {
            if (ctx.MoveHistory.Count == 0)
            {
                //Console.WriteLine("There is not a move can be undo!");
                return;
            }


            var lastMove = ctx.MoveHistory.Last();


            var (fromRow, fromCol) = BoardConverter.StringToIndex(lastMove.From);
            var (toRow, toCol) = BoardConverter.StringToIndex(lastMove.To);


            //return 
            ctx.board[fromRow, fromCol] = ctx.board[toRow, toCol];
            ctx.board[toRow, toCol] = lastMove.Captured == '.' ? ctx.empty : lastMove.Captured;



            // deleting the move from the past
            ctx.MoveHistory.RemoveAt(ctx.MoveHistory.Count - 1);

            var BeforeLast = ctx.MoveHistory.Last();

            var (beforeFromRow, beforeFromCol) = BoardConverter.StringToIndex(BeforeLast.From);
            var (beforeToRow, beforeToCol) = BoardConverter.StringToIndex(BeforeLast.To);

            // updating
            ctx.lastFromCell = new Cell(beforeFromRow, beforeFromCol, ctx);
            ctx.lastToCell = new Cell(beforeToRow, beforeToCol, ctx);


            // Change order back
            ctx.whiteTurn = !ctx.whiteTurn;

            //Console.WriteLine($"Hamle geri alındı: {lastMove}");

        }


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




        public static void MoveHistoryPrinter(ChessContext ctx)
        {

            Console.WriteLine("Move History: ");
            for (int i = 0; i < ctx.MoveHistory.Count; i++)
            {
                var move = ctx.MoveHistory[i];

                if (move.IsWhiteTurn)
                    Console.Write($"{move} ");
                else
                    Console.WriteLine(move);
            }
            if (ctx.MoveHistory.Count % 2 == 1)
            {
                Console.WriteLine();
            }
        }


    }
}
