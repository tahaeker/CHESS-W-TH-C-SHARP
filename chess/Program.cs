using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;


class Cell
{
	public int i;
	public int j;
	public char stone;
	public (int, int) cellIndex;
	public string cellString;

	


	public Cell(int i, int j, ChessContext ctx)
	{

		this.i = i;
		this.j = j;
		if (ctx != null)
		{//chess context is not null


			if (i != -1 && j != -1) {
				this.stone = ctx.board[i, j];
			}       
		}
		this.cellIndex = (this.i,this.j);
		this.cellString = BoardHelper.IndexToString(this.i, this.j);
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


	
	

}

class BoardHelper
{
	public static (int, int) StringToIndex(string position)
	{
		int i = 0;
		int j = 0;
		switch (position[0])
		{
			case 'a': j = 0; break;
			case 'b': j = 1; break;
			case 'c': j = 2; break;
			case 'd': j = 3; break;
			case 'e': j = 4; break;
			case 'f': j = 5; break;
			case 'g': j = 6; break;
			case 'h': j = 7; break;

		}

		switch (position[1])
		{
			case '1': i = 7; break;
			case '2': i = 6; break;
			case '3': i = 5; break;
			case '4': i = 4; break;
			case '5': i = 3; break;
			case '6': i = 2; break;
			case '7': i = 1; break;
			case '8': i = 0; break;
		}

		return (i, j);
	}




	public static Cell StringToCell(string position, ChessContext ctx)
	{
		(int i, int j) = StringToIndex(position);

		Cell cell = new Cell(i, j, ctx);
		return cell;

	}


	public static string IndexToString(int i, int j)
	{
		//string stringI = i.ToString();
		//string stringJ = j.ToString();

		//return stringI + stringJ;// ilk j sonra i çünkü a2 de ilk j!!!!!!!!!!!!
		//böyle yazıldığında olmuyor
		char col = (char)('a' + j);
		int row = 8 - i;
		return $"{col}{row}";
	}


	public static Cell CellToCell(Cell MethodCell, Cell CtxCell)
	{
		CtxCell.i = MethodCell.i;
		CtxCell.j = MethodCell.j;
		CtxCell.stone = MethodCell.stone;


		return CtxCell;

	}



	public static void PrintBoard(ref ChessContext ctx)
	{
		Console.WriteLine("    a   b   c   d   e   f   g   h");

		for (int i = 0; i < ctx.board.GetLength(0); i++)//0satır 1 stn
		{
			Console.Write("  ");
			for (int m = 0; m < Math.Sqrt(ctx.board.Length); m++)
			{
				Console.Write("+---");
			}
			Console.Write("+");
			Console.WriteLine();

			Console.Write(8 - i + " | ");

			for (int j = 0; j < 8; j++)
			{
				if (ctx.board[i, j] == ctx.board[0, 1])
				{

				}
				char stone = ctx.board[i, j];

				if (ctx.inputFrom != "")
				{
					string pos = IndexToString(i, j);
					string moveError = MoveError(ctx.inputFrom, pos, ctx);

					if (i == ctx.touchedCell.i && j == ctx.touchedCell.j)
					{
						Console.ForegroundColor = ConsoleColor.Red;
					}
					else if (moveError == "")
					{
						Console.ForegroundColor = ConsoleColor.Green;
					}
				}

				Console.Write(stone);
				Console.ResetColor();
				Console.Write(" | ");
			}
			Console.WriteLine();
		}
		Console.Write("  +---+---+---+---+---+---+---+---+");
		Console.WriteLine();
		if (ctx.whiteTurn)
		{
			Console.WriteLine("(White Turn.)");
		}
		else if (!ctx.whiteTurn)
		{
			Console.WriteLine("(Black Turn.)");
		}


	}



	public static void TakeFrom(ref ChessContext ctx)
	{
        string errorMessage;
        do
        {
            Console.WriteLine("Hangi taşı oynatmak istiyorsunuz? ");
            ctx.inputFrom = Console.ReadLine();
            ctx.inputFrom = ctx.inputFrom.ToLower();
        int errorNo = IsValidFromToCondition(ctx.inputFrom, ctx.inputTo, ctx);
            if (errorNo == 1 || errorNo == 4 || errorNo == 2 || errorNo == 3)
            {
                errorMessage = "Invalid From condition!!";

                Console.WriteLine(errorMessage);
			}
			else
			{
                errorMessage = "";
                ctx.touchedCell = BoardHelper.StringToCell(ctx.inputFrom, ctx);

			}
			Console.WriteLine();
        } while (errorMessage != "");
    }



	public static void TakeTo(ref ChessContext ctx)
	{
        string errorMessage = "";
        do
        {
            Console.WriteLine("Nereye taşımak istiyorsunuz? ");
            ctx.inputTo = Console.ReadLine();
            ctx.inputTo = ctx.inputTo.ToLower();
        int errorNo = IsValidFromToCondition(ctx.inputFrom, ctx.inputTo, ctx);
            if (errorNo == 11 || errorNo == 5 || errorNo == 6 )
            {
                errorMessage = "Invalid From condition!!";

                Console.WriteLine(errorMessage);
            }
            Console.WriteLine();
        } while (errorMessage != "");

    }






	public static void MoveStone(string from, string to, ChessContext ctx)
	{
		Cell fromCell = StringToCell(from, ctx);
		Cell toCell = StringToCell(to, ctx);

		if (IsValidCastlingMove(fromCell, toCell, ctx))
		{
			//castling move
			if (fromCell.cellIndex == (7, 4) && toCell.cellIndex == (7, 2))//white queenside castling
			{
				ctx.board[7, 0] = '.';
				ctx.board[7, 3] = 'R';
			}
			else if (fromCell.cellIndex == (7, 4) && toCell.cellIndex == (7, 6))//white kingside castling
			{
				ctx.board[7, 7] = '.';
				ctx.board[7, 5] = 'R';
			}
			else if (fromCell.cellIndex == (0, 4) && toCell.cellIndex == (0, 2))//black queenside castling
			{
				ctx.board[0, 0] = '.';
				ctx.board[0, 3] = 'r';
			}
			else if (fromCell.cellIndex == (0, 4) && toCell.cellIndex == (0, 6))//black kingside castling
			{
				ctx.board[0, 7] = '.';
				ctx.board[0, 5] = 'r';
			}
		}




		ctx.board[toCell.i, toCell.j] = ctx.board[fromCell.i, fromCell.j];
		ctx.board[fromCell.i, fromCell.j] = '.';

		


		// for passant movable
		if (char.ToLower(ctx.lastFromCell.stone) == 'p' &&
			char.ToLower(ctx.lastToCell.stone) == 'p')
		{
			int dir = ctx.whiteTurn ? -1 : 1;
			ctx.board[toCell.i - dir, toCell.j] = '.';
		}


		//Turn change
		ctx.whiteTurn = !ctx.whiteTurn;
		//last movements
		ctx.lastFromCell = CellToCell(fromCell, ctx.lastFromCell);
		ctx.lastToCell = CellToCell(toCell, ctx.lastToCell);


		// did rooks move?
		if (!ctx.whiteQueensideRookMoved && ctx.lastFromCell.stone == 'R' && ctx.lastFromCell.cellIndex == (7, 0))
		{
			ctx.whiteQueensideRookMoved = true;

		} else if (!ctx.whiteKingsideRookMoved && ctx.lastFromCell.stone == 'R' && ctx.lastFromCell.cellIndex == (7, 7))
		{
			ctx.whiteKingsideRookMoved = true;

		} else if (!ctx.blackQueensideRookMoved && ctx.lastFromCell.stone == 'r' && ctx.lastFromCell.cellIndex == (0, 0))
		{

			ctx.blackQueensideRookMoved = true;

		} else if (!ctx.blackKingsideRookMoved && ctx.lastFromCell.stone == 'r' && ctx.lastFromCell.cellIndex == (0, 7))
		{
			ctx.blackKingsideRookMoved = true;
		}

		// did kings move?
		if (!ctx.blackKingMoved && ctx.lastFromCell.stone == 'k' && ctx.lastFromCell.cellIndex == (0, 4))
		{

			ctx.blackKingMoved = true;

		}
		if (!ctx.whiteKingMoved && ctx.lastFromCell.stone == 'K' && ctx.lastFromCell.cellIndex == (7, 4))
		{

			ctx.whiteKingMoved = true;

		}

		//how many the kimg moved?
		if (IsThereJustKing(ctx).Item1 && ctx.lastFromCell.stone == 'K')
		{
			// yanlış ctx.howMuchWhitekingMoved = ctx.howMuchWhitekingMoved++;
			ctx.howMuchWhitekingMoved++;
		} else if (IsThereJustKing(ctx).Item2 && ctx.lastFromCell.stone == 'k')
		{
			//yanlış ctx.howMuchWhitekingMoved = ctx.howMuchWhitekingMoved++;
			ctx.howMuchBlackkingMoved++;
		}




	}



	public static int IsValidFromToCondition(string from, string to, ChessContext ctx)
	{

		if (string.IsNullOrEmpty(from) )
		{
			//Console.WriteLine("From cannot be empty!!");
			return 1;
		}

		if (from.Length != 2)
		{
			return 4; // From must be 2 characters long
		}

		char letter = from[0];

		if (!(letter >= 'a' & letter <= 'h'))
		{
			//Console.WriteLine("From or To must be a valid letter!!\");
			return 2;
		}

		int number = from[1] - '0';// '0' karakterini çıkararak sayıya çeviriyoruz '0' eşittir 48 ASCII kodu
		if (!(number >= 1 & number <= 8))
		{
			// Console.WriteLine("From or To must be a valid number!!\");
			return 3;
		}


		//to part
		if (string.IsNullOrEmpty(to))
		{
			//Console.WriteLine("To cannot be empty!!");
			return 11;
			
		}

		if (to.Length != 2)
		{
			return 5; // To must be 2 characters long
		}


		if (from == to)
		{
			//Console.WriteLine("From and To cannot be the same!!");
			return 6;
		}



		return 0;

	}



	public static bool IsStoneWhite(Cell Stone, ChessContext ctx)
	{
		char stone = Stone.stone;


		if (stone == 'P' | stone == 'R' |
			stone == 'N' | stone == 'B' |
			stone == 'Q' | stone == 'K')
		{
			return true;
		}

		return false;

	}
	public static bool IsStoneWhiteC(char ch)
	{
		char stone = ch;

		if (stone == 'P' | stone == 'R' |
			stone == 'N' | stone == 'B' |
			stone == 'Q' | stone == 'K')
		{
			return true;
		}

		return false;

	}
	public static bool IsStoneBlackC(char ch)
	{
		char stone = ch;

		if (stone == 'p' | stone == 'r' |
			stone == 'n' | stone == 'b' |
			stone == 'q' | stone == 'k')
		{
			return true;
		}

		return false;

	}
	public static bool IsStoneEmptyC(char ch)
	{

		char stone = ch;
		

		if (stone == '.')
		{
			return true;
		}
		return false;


	}
	public static bool IsStoneEmpty(Cell Stone, ChessContext ctx)
	{

		char stone = Stone.stone;

		if (stone == '.')
		{
			return true;
		}
		return false;


	}
	public static bool IsStoneBlack(Cell Stone, ChessContext ctx)
	{
		char stone = Stone.stone;
		if (stone == 'p' | stone == 'r' |
			stone == 'n' | stone == 'b' |
			stone == 'q' | stone == 'k')
		{
			return true;
		}
		return false;

	}




	public static bool IsValidPawnMove(Cell fromCell, Cell toCell, ChessContext ctx)
	{
		if (char.ToLower(fromCell.stone) != 'p')// parametreye dikkat et ilk başta hepsi string tanımlı
		{
			return false;
		}
		//direction -1 for white ,1for black
		int dir = ctx.whiteTurn ? -1 : 1;



		if (fromCell.j == toCell.j &&
			toCell.i - fromCell.i == dir &&
			IsStoneEmpty(toCell, ctx))
		{
			return true;
		} //start point of the pawm
		else if (
			fromCell.j == toCell.j && toCell.i - fromCell.i == 2 * dir &&
			(fromCell.i == 1 || fromCell.i == 6) &&
			IsStoneEmpty(toCell, ctx) &&
			ctx.board[fromCell.i + dir, fromCell.j] == '.')
		{
			return true;
		}


		//çapraz yemek için
		if (
			(!IsStoneEmpty(toCell, ctx) && (toCell.j - fromCell.j == dir) && (toCell.i - fromCell.i == dir)) ||
			(!IsStoneEmpty(toCell, ctx) && (toCell.j - fromCell.j == -dir) && (toCell.i - fromCell.i == dir))
			)
		{
			return true;
		}
		// passant move
		if (char.ToLower(ctx.lastFromCell.stone) == 'p' && ctx.lastToCell.j == toCell.j &&
			(
			(ctx.lastFromCell.i == 6 && fromCell.i == 4 && (toCell.i - fromCell.i == dir) && (toCell.j - fromCell.j == -dir)) ||
			(ctx.lastFromCell.i == 6 && fromCell.i == 4 && (toCell.i - fromCell.i == dir) && (toCell.j - fromCell.j == dir)) ||
			(ctx.lastFromCell.i == 1 && fromCell.i == 3 && (toCell.i - fromCell.i == dir) && (toCell.j - fromCell.j == dir)) ||
			(ctx.lastFromCell.i == 1 && fromCell.i == 3 && (toCell.i - fromCell.i == dir) && (toCell.j - fromCell.j == -dir))
			)
			)
		{
			return true;
		}

		return false;
	}




	public static bool IsValidRookMove(Cell fromCell, Cell toCell, ChessContext ctx)
	{
		if (char.ToLower(fromCell.stone) != 'r' &&
			char.ToLower(fromCell.stone) != 'q')// parametreye dikkat et ilk başta hepsi string tanımlı
		{
			return false;
		}

		//yatay sağa doğru
		if (fromCell.i == toCell.i &&
			toCell.j != fromCell.j
			)
		{
			int yDirectionStep = (toCell.j > fromCell.j) ? 1 : -1;

			for (int j = (fromCell.j + yDirectionStep); j != toCell.j; j += yDirectionStep)
			{
				if (ctx.board[fromCell.i, j] != '.')

					return false;
			}


		}
		else if (fromCell.i != toCell.i &&
		fromCell.j == toCell.j)
		{
			int xDirectionStep = (toCell.i > fromCell.i) ? 1 : -1;//dikey

			for (int i = fromCell.i + xDirectionStep; i != toCell.i; i += xDirectionStep)
			{
				if (ctx.board[i, fromCell.j] != '.') return false;
			}
		} else
		{
			return false;
		}

		return true;
	}




	public static bool IsValidKnightMove(Cell fromCell, Cell toCell, ChessContext ctx)
	{

		if (toCell.i == fromCell.i - 2 &&
			((toCell.j == fromCell.j - 1) || (toCell.j == fromCell.j + 1))
			)
		{
			return true;
		}
		else if (toCell.j == fromCell.j - 2 &&
			((toCell.i == fromCell.i - 1) || toCell.i == fromCell.i + 1)
			)
		{
			return true;
		}
		else if (toCell.i == fromCell.i + 2 &&
			((toCell.j == fromCell.j + 1) || (toCell.j == fromCell.j - 1))
			)
		{
			return true;
		}
		else if (toCell.j == fromCell.j + 2 &&
			((toCell.i == fromCell.i - 1) || toCell.i == fromCell.i + 1)
			)
		{
			return true;
		}


		return false;
	}



	public static bool IsValidBishopMove(Cell fromCell, Cell toCell, ChessContext ctx)
	{
		if (char.ToLower(fromCell.stone) != 'b' &&
			char.ToLower(fromCell.stone) != 'q')// parametreye dikkat et ilk başta hepsi string tanımlı
		{
			return false;
		}

		int deltaI = toCell.i - fromCell.i;
		int deltaJ = toCell.j - fromCell.j;
		//çapraz olmalı
		if (Math.Abs(deltaI) != Math.Abs(deltaJ))
		{
			return false;
		}


		// direction yönü neresi
		int dirI = deltaI > 0 ? 1 : -1;
		int dirJ = deltaJ > 0 ? 1 : -1;

		int steps = Math.Abs(deltaI);// kaç adım ilelrlesin
		for (int k = 1; k < steps; k++)// ara karelere bakıp taş varsa false döner
		{
			int checkI = fromCell.i + dirI * k;
			int checkJ = fromCell.j + dirJ * k;
			if (ctx.board[checkI, checkJ] != '.')
			{
				return false;
			}

		}

		return true;
	}



	public static bool IsValidQueenMove(Cell fromCell, Cell toCell, ChessContext ctx)
	{
		if (IsValidBishopMove(fromCell, toCell, ctx) ||
			IsValidRookMove(fromCell, toCell, ctx))
		{
			return true;
		}
		return false;
	}


	public static bool IsValidKingMove(Cell fromCell, Cell toCell, ChessContext ctx)
	{
	  
		// direction 
		int deltaI = toCell.i - fromCell.i;
		int deltaJ = toCell.j - fromCell.j;
		int dirI = deltaI > 0 ? 1 : -1;
		int dirJ = deltaJ > 0 ? 1 : -1;

		//for normal movement 
		if ((Math.Abs(fromCell.i - toCell.i) == 1 && Math.Abs(fromCell.j - toCell.j) == 1) ||
		(Math.Abs(fromCell.i - toCell.i) == 1 && Math.Abs(fromCell.j - toCell.j) == 0) ||
		(Math.Abs(fromCell.i - toCell.i) == 0 && Math.Abs(fromCell.j - toCell.j) == 1))
		{
			return true;
		}

		// does king check in routa


		if (!ctx.IsFakeMovement)
		{
			ChessContext tempCtx = copyBoard(ctx);
			tempCtx.IsFakeMovement = true;

			if (IsValidCastlingMove(fromCell, toCell, tempCtx))
			{
				return true;
			}
		}
	   


			return false;

	}


	public static bool IsValidCastlingMove(Cell fromCell, Cell toCell, ChessContext ctx)
	{

		ChessContext tempCtx;
		tempCtx = copyBoard(ctx);
		tempCtx.IsFakeMovement = true;

		if (ctx.whiteTurn && IsWhitekingUnderThreat(ctx))
		{
			return false;
		}

		if(!ctx.whiteTurn && IsBlackkingUnderThreat(ctx))
		{
			return false;
		}

		if (fromCell.cellIndex == (7, 4) && toCell.cellIndex == (7, 2) &&
		ctx.board[7, 2] == ctx.empty && ctx.board[7, 3] == ctx.empty &&
		!ctx.whiteQueensideRookMoved && ctx.board[7,0] != ctx.empty && !ctx.whiteKingMoved)
		{
			return true;
		}
		else if (fromCell.cellIndex == (7, 4) && toCell.cellIndex == (7, 6) &&
			ctx.board[7, 5] == ctx.empty && ctx.board[7, 6] == ctx.empty &&
			!ctx.whiteKingsideRookMoved && ctx.board[7, 7] != ctx.empty && !ctx.whiteKingMoved)
		{
			return true;
		}



		//black king castling
		if (fromCell.cellIndex == (0, 4) && toCell.cellIndex == (0, 2) &&
		ctx.board[0, 2] == ctx.empty && ctx.board[0, 3] == ctx.empty &&
		!ctx.blackQueensideRookMoved && ctx.board[0, 0] != ctx.empty && !ctx.blackKingMoved)
		{
			return true;
		}
		else if (fromCell.cellIndex == (0, 4) && toCell.cellIndex == (0, 6) &&
			ctx.board[0, 5] == ctx.empty && ctx.board[0, 6] == ctx.empty &&
			!ctx.blackKingsideRookMoved && ctx.board[0, 7] != ctx.empty && !ctx.blackKingMoved)
		{
			return true;
		}
		

		


		return false;
	}

	public static (Cell, Cell) kingLocations(ChessContext ctx)
	{
		Cell whiteKingL = new Cell(-1, -1, ctx);
		Cell blackKingL = new Cell(-1, -1, ctx);

		for (int i = 0; i < 8; i++)
		{
			for (int j = 0; j < 8; j++)
			{
				if (ctx.board[i, j] == 'K')
				{

					whiteKingL = new Cell(i, j, ctx);

				}
				
				if (ctx.board[i, j] == 'k') {

					blackKingL = new Cell(i, j, ctx);

				}

			}

		}

		return (whiteKingL, blackKingL);
	}

	public static bool IsWhitekingUnderThreat(ChessContext ctx)
	{
		(Cell whiteKing, _) = kingLocations(ctx);

		//fro each order stone to white king location
		for (int checkI = 0; checkI < 8; checkI++)
		{
			for (int checkJ = 0; checkJ < 8; checkJ++)
			{
				if (checkI == 1 & checkJ == 3)
				{

				}
				string fromCellOfThreat = IndexToString(checkI, checkJ);
				ChessContext tempCtx = copyBoard(ctx);
				Cell fromCellOfThreatCell = StringToCell(fromCellOfThreat, tempCtx);

				// kendi taşın kendini tehtid edemeyeceği için renk değiştirdik
				tempCtx.whiteTurn = false;
				if (IsStoneBlack(fromCellOfThreatCell, tempCtx) && MoveError(fromCellOfThreat, whiteKing.cellString, tempCtx) == "")
					return true;

			}

		}
		return false;
	}

	public static bool IsBlackkingUnderThreat(ChessContext ctx)
	{
		(Cell whiteKing, Cell blackKing) = kingLocations(ctx);


		//fro each order stone to white king location
		for (int checkI = 0; checkI < 8; checkI++)
		{
			for (int checkJ = 0; checkJ < 8; checkJ++)
			{
				if(checkI == 7& checkJ == 3)
				{

				}
				string fromCellOfThreat = IndexToString(checkI, checkJ);
				ChessContext tempCtx = copyBoard(ctx);
				Cell fromCellOfThreatCell = StringToCell(fromCellOfThreat,tempCtx);

				// kendi taşın kendini tehtid edemeyeceği için renk değiştirdik
				tempCtx.whiteTurn = true ;
				if (IsStoneWhite(fromCellOfThreatCell, tempCtx) && MoveError(fromCellOfThreat, blackKing.cellString, tempCtx) == "")
					return true;
				
			}

		}
		return false;
	}




	public static ChessContext copyBoard(ChessContext ctx)
	{
		ChessContext newCtx = new ChessContext();
		// Tahta kopyalama
		for (int i = 0; i < 8; i++)
			for (int j = 0; j < 8; j++)
				newCtx.board[i, j] = ctx.board[i, j];

		newCtx.inputFrom = ctx.inputFrom;
		newCtx.inputTo = ctx.inputTo;
		newCtx.empty = ctx.empty;
		newCtx.whiteTurn = ctx.whiteTurn;
		newCtx.whiteKingMoved = ctx.whiteKingMoved;
		newCtx.whiteQueensideRookMoved = ctx.whiteQueensideRookMoved;
		newCtx.whiteKingsideRookMoved = ctx.whiteKingsideRookMoved;
		newCtx.blackKingMoved = ctx.blackKingMoved;
		newCtx.blackQueensideRookMoved = ctx.blackQueensideRookMoved;
		newCtx.blackKingsideRookMoved = ctx.blackKingsideRookMoved;
		newCtx.IsFakeMovement = ctx.IsFakeMovement;

		// Cell nesnelerini de kopyala (gerekirse)
		newCtx.touchedCell = new Cell(ctx.touchedCell.i, ctx.touchedCell.j, newCtx);
		newCtx.lastFromCell = new Cell(ctx.lastFromCell.i, ctx.lastFromCell.j, newCtx);
		newCtx.lastToCell = new Cell(ctx.lastToCell.i, ctx.lastToCell.j, newCtx);

		return newCtx;
	}



	public static void CheckGameEnd(ChessContext ctx)
	{
		//the point in order for fromCEll
		for (int i = 0; i < 8; i++)
		{
			for (int j = 0; j < 8; j++)
			{
				string from = IndexToString(i, j);
				char ch = ctx.board[i, j];
				if (ch != '.') continue;
				//toCell in order
				for (int iTo = 0; iTo < 8; iTo++)
				{
					for (int jTo = 0; jTo < 8; jTo++)
					{
						if (i != iTo && j != jTo) continue;
						string to = IndexToString(iTo, jTo);
						ChessContext CheckEnd = copyBoard(ctx);
						bool error = MoveError(from, to, CheckEnd) == "" ? false : true;
						if (IsWhitekingUnderThreat(CheckEnd) && !error) break;

						if (IsWhitekingUnderThreat(CheckEnd) && error)
						{
							ctx.blackWins = true;
						}
						else
						{
							ctx.blackWins = false;
						}

						if (IsBlackkingUnderThreat(CheckEnd) && error)
						{
							ctx.whiteWins = true;
						}
						else
						{
							ctx.whiteWins = false;

						}

						if((!IsWhitekingUnderThreat(CheckEnd) && !IsBlackkingUnderThreat(CheckEnd) && !error) ||
							ctx.howMuchBlackkingMoved == 50 || ctx.howMuchWhitekingMoved == 50
							)
						{
							ctx.drawStuation = true;

						}
						else
						{
							ctx.drawStuation = false;

						}


					}
				}
			}
		}

		//ctx.isGameEnd = true;

		//white king Draw by fifty-move rule
		if (IsThereJustKing(ctx).Item1== true && ctx.howMuchWhitekingMoved == 50)
		{
			ctx.drawStuation = true;
		} else if(IsThereJustKing(ctx).Item2 == true&& ctx.howMuchBlackkingMoved == 50)
		{
			ctx.drawStuation = true;
		}



	}



	public static (bool, bool) IsThereJustKing(ChessContext ctx)
	{
		bool whiteKing = false;
		bool blackKing = false;
		for (int i = 0; i < 8; i++)
		{
			for (int j = 0; j < 8; j++)
			{
				char ch = ctx.board[i, j];
				if (ch == 'K')
					whiteKing = true;
				if (ch == 'k')
					blackKing = true;
				if (ch != 'K' && IsStoneWhiteC(ch))
					whiteKing = false;
				if(ch != 'k'&&IsStoneWhiteC(ch))
					blackKing = false;


			}
		}
		// Sadece iki şah varsa true, true dön
		return (whiteKing, blackKing);
	}





	public static string MoveError(string from,string to, ChessContext ctx)
	{
		if(from == "" && to == "")
		{
			return "";
		}

		Cell fromCell= StringToCell(from, ctx);
		Cell toCell = StringToCell(to, ctx);

		//for flag
		if(toCell.cellIndex == (2, 4)&& toCell.stone == 'K' && fromCell.cellIndex== (1,3))
		{
		   
		}
		if (fromCell.stone== '.')
		{
			return "From Cell cannot be empty!!";
		}
		if (IsValidFromToCondition( from ,to, ctx) == 1)
		{
			return "From or To cannot be empty!!";
		}
        if (IsValidFromToCondition(from, to, ctx) == 11)
        {
            return "To cannot be empty!!";
        }


        if (IsValidFromToCondition(from, to, ctx) == 4)
		{
			return "From must be 2 characters long!!";
		}
		if (IsValidFromToCondition(from, to, ctx) == 5)
		{
			return "To must be 2 characters long!!";
		}
		if (IsValidFromToCondition(from,to,ctx)==2)
		{
			 return "From or To must be a valid letter!!";
		}
		if (IsValidFromToCondition(from, to, ctx) == 3)
		{
			return "Y Axis of From must be a valid between 1-7";
		}

		if(ctx.whiteTurn & !IsStoneWhite(fromCell, ctx))
		{
			return "Row of White Stones";
		}
		else if (!ctx.whiteTurn & IsStoneWhite(fromCell, ctx))
		{
			return "Row of black Stones";
		}


		if (IsStoneWhite(fromCell, ctx) & (IsStoneWhite(toCell, ctx) & !IsStoneEmpty(toCell,ctx)))
		{
			
			return "Cannot Whites Move Owm Stone";
		}
		if (IsStoneBlack(fromCell,ctx) & (IsStoneBlack(toCell,ctx) & !IsStoneEmpty(toCell,ctx)))
		{
		   return "Cannot Blacks Move Owm Stone";   
		}

		switch (char.ToLower(fromCell.stone))
		{
			case 'p':
				if (!IsValidPawnMove(fromCell, toCell, ctx))
					return "Invalid pawn move!";
				break;
			case 'r':
				if (!IsValidRookMove(fromCell, toCell, ctx))
					return "Invalid rook move!";
				break;
			case 'n':
				if (!IsValidKnightMove(fromCell, toCell, ctx))
					return "Invalid knight move!";
				break;
			case 'b':
				if (!IsValidBishopMove(fromCell, toCell, ctx))
					return "Invalid bishop move!";
				break;
			case 'q':
				if (!IsValidQueenMove(fromCell, toCell, ctx))
					return "ınvalid queen move!";
				break;
			case 'k':
				if (!IsValidKingMove(fromCell, toCell, ctx))
					return "Invalid king move!";
				break;

		}

		// check if the move puts the king in check
		if (!ctx.IsFakeMovement)
		{
			ChessContext nextCtx = copyBoard(ctx);
			nextCtx.IsFakeMovement = true; // to prevent stack overflow exception

			MoveStone(fromCell.cellString, toCell.cellString, nextCtx);

			if (nextCtx.whiteTurn && IsBlackkingUnderThreat(nextCtx))
				return "Black king is under threat";

			if (!nextCtx.whiteTurn && IsWhitekingUnderThreat(nextCtx))
				return "White king is under threat";





			ChessContext movableCtx2 = copyBoard(ctx);
			if (IsValidCastlingMove(fromCell, toCell, movableCtx2))
				{
				bool isWhiteKingUnderCheckInRouta = IsWhitekingUnderThreat(movableCtx2);
				bool isBlackKingUnderCheckInRouta = IsBlackkingUnderThreat(movableCtx2);

				(Cell whiteKing,Cell blackKing) = kingLocations(movableCtx2);

				if (!IsWhitekingUnderThreat(ctx))
				{
					if (toCell.cellIndex == (7, 2))
					{
						MoveStone(whiteKing.cellString, "d1", movableCtx2);

						if (isWhiteKingUnderCheckInRouta)
						{
							return "White King Under Check in Long Casling Routa.";
						}

						MoveStone("d1", "c1", movableCtx2);

						if (isWhiteKingUnderCheckInRouta)
						{
							return "White King Under Check in Long Casling Routa.";
						}


					}

					else if (toCell.cellIndex == (7, 6))
					{

						MoveStone(whiteKing.cellString, "f1", movableCtx2);
						if (isWhiteKingUnderCheckInRouta)
						{
							return "White King Under Check in Short Casling Routa.";
						}
						MoveStone("f1", "g1", movableCtx2);
						if (isWhiteKingUnderCheckInRouta)
						{
							return "White King Under Check in Short Casling Routa.";
						}
					}
				}
				

				if (!IsBlackkingUnderThreat(ctx))
				{
					if (toCell.cellIndex == (0, 2))
					{
						MoveStone(blackKing.cellString, "d8", movableCtx2);
						if (isBlackKingUnderCheckInRouta)
						{
							return "Black King Under Check in Long Casling Routa.";
						}
						MoveStone("d8", "c8", movableCtx2);
						if (isBlackKingUnderCheckInRouta)
						{
							return "Black King Under Check in Long Casling Routa.";
						}

					}
					else if (toCell.cellIndex == (0, 6))
					{
						MoveStone(blackKing.cellString, "f8", movableCtx2);

						if (isBlackKingUnderCheckInRouta)
						{
							return "Black King Under Check in Short Casling Routa.";
						}

						MoveStone("f8", "g8", movableCtx2);

						if (isBlackKingUnderCheckInRouta)
						{
							return "Black King Under Check in Short Casling Routa.";
						}

					}
				}   

			}
		}




		return "";


	}
}

class Program
{

	static void Main()
	{
		ChessContext ctx = new ChessContext();
		ctx.board = new char[8, 8]{
		{ 'r', 'n', 'b', 'q', 'k', 'b', 'n', 'r' }, // 8. sıra (siyah)
		{ 'p', 'p', 'p', 'p', 'p', 'p', 'p', 'p' },
		{ '.', '.', '.', '.', '.', '.', '.', '.' },
		{ '.', '.', '.', '.', '.', '.', '.', '.' },
		{ '.', '.', '.', '.', '.', '.', '.', '.' },
		{ '.', '.', '.', '.', '.', '.', '.', '.' },
		{ 'P', 'P', 'P', 'P', 'P', 'P', 'P', 'P' },
		{ 'R', 'N', 'B', 'Q', 'K', 'B', 'N', 'R' }  // 1. sıra (beyaz)
		};




		BoardHelper.PrintBoard(ref ctx);
		while (true)
		{
            BoardHelper.TakeFrom(ref ctx);
            Console.Clear();
            BoardHelper.PrintBoard(ref ctx);


            BoardHelper.TakeTo(ref ctx);
            Console.Clear();

            string error = BoardHelper.MoveError(ctx.inputFrom, ctx.inputTo, ctx);

            if (ctx.inputFrom != "" & ctx.inputTo != "")
            {

                if (error == "")
                {
                    BoardHelper.MoveStone(ctx.inputFrom, ctx.inputTo, ctx);
                }
                else
                {
                    Console.WriteLine(error);
                }
                BoardHelper.PrintBoard(ref ctx);
            }
            //if just king is left  
            if (BoardHelper.IsThereJustKing(ctx).Item1)
			{
				Console.WriteLine($"White King moved {ctx.howMuchWhitekingMoved} times.");
			}
			if (BoardHelper.IsThereJustKing(ctx).Item2)
			{
				Console.WriteLine($"Black King moved {ctx.howMuchBlackkingMoved} times.");

			}



			// Check if the game has ended
			if (ctx.whiteWins)
			{
				Console.WriteLine(" Whites Won.:)");
			}
			else if (ctx.blackWins)
			{
				Console.WriteLine(" Blacks Won.:)");

			}
			else if (ctx.drawStuation)
			{
				Console.WriteLine(" There is not Winner. Draw :)");

			}
			

		}
	}

}
//eksikler
//yanlış from to condition tekrar istemek
//passant move tahtanın ortasındada oldu siyah taşda