using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using ChessEngine.Core;




public class ConsoleInputProvider : IInputProvider//this is taking data input from console
{
    public string ReadLine()
    {
        return Console.ReadLine();
    }
}

//public class TestInputProvider : IInputProvider//this is taking data input from test
//{
//	//burayı anlamadım
//	//private readonly Queue<string> inputs;
//	//public TestInputProvider(IEnumerable<string> inputs)
//	//{
//	//	this.inputs = new Queue<string>(inputs);
//	//}
//	//public string ReadLine()
//	//{
//	//	return inputs.Count > 0 ? inputs.Dequeue() : null;
//	//}
//}




public class ConsoleOutputProvider : IOutputProvider
{
	public void Write(string message)
	{
		Console.Write(message);
    }

	public void WriteLine(string message)
	{
		Console.WriteLine(message);
	}
}
class Program
{

	static void Main()
	{
		
		ChessContext ctx = new ChessContext();
		ctx.Board = new char[8, 8]{
		{ 'r', 'n', 'b', 'q', 'k', 'b', 'n', 'r' }, // 8. sıra (siyah)
		{ 'p', 'p', 'p', '.', '.', '.', 'p', 'p' },
		{ '.', '.', '.', '.', '.', 'q', '.', '.' },
		{ '.', '.', 'b', '.', '.', '.', '.', '.' },
		{ '.', '.', '.', '.', '.', '.', '.', '.' },
		{ '.', '.', '.', '.', '.', '.', '.', '.' },
		{ 'P', 'P', 'P', '.', '.', '.', 'P', 'P' },
		{ 'R', '.', '.', 'Q', 'K', 'B', 'N', 'R' }  // 1. sıra (beyaz)
		};

		IInputProvider inputProvider = new ConsoleInputProvider();
		IOutputProvider outputProvider = new ConsoleOutputProvider();

		outputProvider.WriteLine("Welcome to Console Chess!");



        ctx.WhitePlayer = new Player("Alice", true);
        ctx.BlackPlayer = new Player("Bob", false);
		DataStorage.LoadPlayers("Players.txt");



        BoardPrinter.PrintBoard(ref ctx);
		
		
		int i = 0;
		
		
		//ctx.MoveHistory = DataStorage.LoadMoveHistory("moves.json");//varsa dosyadan gelsin



		while (true)
		{
            InputHandler.TakeFrom(ctx, inputProvider);
            Console.Clear();
            BoardPrinter.PrintBoard(ref ctx);


            InputHandler.TakeTo(ctx, inputProvider);
            Console.Clear();


            var result = ChessEngine.Core.ChessEngine.TryMove(ctx.inputFrom, ctx.inputTo, ctx);
            if (!result.Success)
            {
                outputProvider.WriteLine(result.ErrorMessage);
            }
			BoardPrinter.PrintBoard(ref ctx);


            //if just king is left  
            if (BoardState.IsThereJustKing(ctx).Item1)
			{
                outputProvider.WriteLine($"White King moved {ctx.howMuchWhitekingMoved} times.");
			}
			if (BoardState.IsThereJustKing(ctx).Item2)
			{
                outputProvider.WriteLine($"Black King moved {ctx.howMuchBlackkingMoved} times.");

			}

			
            BoardState.CheckGameEnd(ctx);
            // Check if the game has ended
            if (ctx.whiteWins)
			{
				Console.WriteLine(" Whites Won.:)");
                ctx.WhitePlayer.Wins++;
                ctx.BlackPlayer.Losses--;
                break;
			}
			else if (ctx.blackWins)
			{
				Console.WriteLine(" Blacks Won.:)");
                ctx.BlackPlayer.Wins++;
                ctx.WhitePlayer.Losses--;
                break;
			}
			else if (ctx.drawStuation)
			{
				Console.WriteLine(" There is not Winner. Draw :)");
				break;
			}



            //Write History Move
            DataStorage.MoveHistoryPrinter(ctx);
            //Console.WriteLine($"Last Move: {ctx.MoveHistory.Last()}");
            DataStorage.SaveMoveHistory(ctx.MoveHistory, "moves.json");// her hamleden osnra dosyaya kaydet

            DataStorage.SavePlayers(ctx.WhitePlayer, ctx.BlackPlayer, "Players.txt");

        }
    }

}
//eksikler
//passant move tahtanın ortasındada oldu siyah taşda