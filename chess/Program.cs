using chess;
using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;


struct Cell
{
	public int Row { get; }
	public int Col { get; }
    public char stone { get; set; }

	public (int, int) cellIndex => (Row,Col);
	public string cellString => BoardConverter.IndexToString(Row, Col);

	public bool isEmpty => stone == '.';

    private static readonly char[] WhitePieces = { 'P', 'R', 'N', 'B', 'Q', 'K' };
    private static readonly char[] BlackPieces = { 'p', 'r', 'n', 'b', 'q', 'k' };

    public bool IsWhite => WhitePieces.Contains(stone);
    public bool IsBlack => BlackPieces.Contains(stone);


    public Cell(int i, int j, ChessContext ctx)
	{

		Row = i;
		Col = j;
		if (ctx != null && i > -1 && j > -1)
		{//chess context is not null
			stone = ctx.board[i, j];
		}
		else
		{
			stone = '.';// default empty stone

        }
		
	}
}
class ChessContext
{
	public char[,] board = new char[8, 8];
	public  string inputFrom= "";
	public  string inputTo= "";
	public char empty = '.'; 
	public bool whiteTurn = true;


	public Cell touchedCell = new Cell(0, 0, null);
	public Cell lastFromCell = new Cell(0, 0, null);
	public Cell lastToCell = new Cell(0, 0, null);


	public bool whiteKingMoved = false;
	public bool blackKingMoved = false;
	public bool whiteQueensideRookMoved = false;
	public bool whiteKingsideRookMoved = false;
	public bool blackQueensideRookMoved = false;
	public bool blackKingsideRookMoved = false;


	public bool IsFakeMovement = false; // for testing sub purposes without stack overflow exception
	
	public bool whiteWins = false;
	public bool blackWins = false;
	public bool drawStuation = false;
	public bool isGameEnd = false;



	public int howMuchWhitekingMoved = 0;
	public int howMuchBlackkingMoved = 0;



	public List<chess.Move> MoveHistory { get; set; } = new List<chess.Move>();


	public Player WhitePlayer { get; set; }
	public Player BlackPlayer { get; set; }





	
	

}

public interface IInputProvider
{
    string ReadLine();
}

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



public interface IOutputProvider
{
	void Write(string message);
	void WriteLine(string message);
}



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
		ctx.board = new char[8, 8]{
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


            var result = ChessEngine.TryMove(ctx.inputFrom, ctx.inputTo, ctx);
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