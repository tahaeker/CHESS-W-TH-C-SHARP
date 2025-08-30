using chess;
using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;


class Cell
{
	public int Row { get; }
	public int Col { get; }
    public char stone { get; set; }

	public (int, int) cellIndex => (Row,Col);
	public string cellString => BoardHelper.IndexToString(Row, Col);

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


	
	

}

class BoardHelper
{
	public static (int, int) StringToIndex(string position)
	{


		position = position.ToLower(); // küçük harfe 
									   //'a' = ascii97
		int col = position[0] - 'a'; // 'a' karakterinden çıkararak sütun indeksini al
									 // '1' = ascii 48
		int row = 8 - (position[1] - '0'); // '0' karakterinden çıkararak satır indeksini al
		return (row, col);
	}





	public static Cell StringToCell(string position, ChessContext ctx)
	{
		(int i, int j) = StringToIndex(position);

		Cell cell = new Cell(i, j, ctx);
		return cell;

	}


	public static string IndexToString(int i, int j)
	{
		char col = (char)('a' + j);
		int row = 8 - i;
		return $"{col}{row}";
	}


	public static Cell CellToCell(Cell MethodCell, ChessContext ctx)
	{
		return new Cell(MethodCell.Row, MethodCell.Col, ctx); // Cell nesnesini yeni bir ChessContext ile oluştur


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

					if (i == ctx.touchedCell.Row && j == ctx.touchedCell.Col)
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
		Console.WriteLine("    a   b   c   d   e   f   g   h");
		if (ctx.whiteTurn)
		{
			Console.WriteLine("(White Turn.)");
		}
		else if (!ctx.whiteTurn)
		{
			Console.WriteLine("(Black Turn.)");
		}


	}



	public static void TakeFrom(ChessContext ctx, IInputProvider inputProvider)
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



	public static void TakeTo(ChessContext ctx, IInputProvider inputProvider)
	{
		string errorMessage = "";
		do
		{
			Console.WriteLine("Nereye taşımak istiyorsunuz? ");
			ctx.inputTo = Console.ReadLine();
			ctx.inputTo = ctx.inputTo.ToLower();
			int errorNo = IsValidFromToCondition(ctx.inputFrom, ctx.inputTo, ctx);
			if (errorNo == 11 || errorNo == 5 || errorNo == 6)
			{
				errorMessage = "Invalid To condition!!";

				Console.WriteLine(errorMessage);
			}
			else
			{
				errorMessage = "";

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




		ctx.board[toCell.Row, toCell.Col] = ctx.board[fromCell.Row, fromCell.Col];
		ctx.board[fromCell.Row, fromCell.Col] = '.';




		// for passant movable
		if (char.ToLower(ctx.lastFromCell.stone) == 'p' &&
			char.ToLower(ctx.lastToCell.stone) == 'p')
		{
			int dir = ctx.whiteTurn ? -1 : 1;
			ctx.board[toCell.Row - dir, toCell.Col] = '.';
		}


        //last movements
        ctx.lastFromCell = CellToCell(fromCell, ctx);
        ctx.lastToCell = CellToCell(toCell, ctx);
		ctx.lastToCell.stone = toCell.stone;
		ctx.lastFromCell.stone = fromCell.stone;


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


		//recording data to list
		var move = new Move
		{
			From = ctx.lastFromCell.cellString,
			To = ctx.lastToCell.cellString,
			Piece = ctx.lastFromCell.stone,
			Captured = ctx.lastToCell.isEmpty ? '.' : ctx.lastFromCell.stone,
			TurnNumber = (ctx.MoveHistory.Count / 2) + 1,
			IsWhiteTurn = ctx.whiteTurn,
		};
        ctx.MoveHistory.Add(move);



		//Turn change
		ctx.whiteTurn = !ctx.whiteTurn;
	}


	public static void MoveHistoryPrinter(ChessContext ctx) {

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
	



	public static bool IsValidPawnMove(Cell fromCell, Cell toCell, ChessContext ctx)
	{
		if (char.ToLower(fromCell.stone) != 'p')// parametreye dikkat et ilk başta hepsi string tanımlı
		{
			return false;
		}
		//direction -1 for white ,1for black
		int dir = ctx.whiteTurn ? -1 : 1;
		if(toCell.Row == 4&& toCell.Col == 2)
		{

		}


		if (fromCell.Col == toCell.Col &&
			toCell.Row - fromCell.Row == dir &&
			toCell.isEmpty)		{
			return true;
		} //start point of the pawm
		else if (
			fromCell.Col == toCell.Col && toCell.Row - fromCell.Row == 2 * dir &&
			(fromCell.Row == 1 || fromCell.Row == 6) &&
			toCell.isEmpty &&
			ctx.board[fromCell.Row + dir, fromCell.Col] == '.')
		{
			return true;
		}


		//çapraz yemek için
		if (
			(!toCell.isEmpty && (toCell.Col - fromCell.Col == dir) && (toCell.Row - fromCell.Row == dir)) ||
			(!toCell.isEmpty && (toCell.Col - fromCell.Col == -dir) && (toCell.Row - fromCell.Row == dir))
			)
		{
			return true;
		}
		// passant move
		if (char.ToLower(ctx.lastFromCell.stone) == 'p' && ctx.lastToCell.Col == toCell.Col &&
			(
			(ctx.lastFromCell.Row == 6 && fromCell.Row == 4 && (toCell.Row - fromCell.Row == dir) && (toCell.Col - fromCell.Col == -dir)) ||
			(ctx.lastFromCell.Row == 6 && fromCell.Row == 4 && (toCell.Row - fromCell.Row == dir) && (toCell.Col - fromCell.Col == dir)) ||
			(ctx.lastFromCell.Row == 1 && fromCell.Row == 3 && (toCell.Row - fromCell.Row == dir) && (toCell.Col - fromCell.Col == dir)) ||
			(ctx.lastFromCell.Row == 1 && fromCell.Row == 3 && (toCell.Row - fromCell.Row == dir) && (toCell.Col - fromCell.Col == -dir))
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
		if (fromCell.Row == toCell.Row &&
			toCell.Col != fromCell.Col
			)
		{
			int yDirectionStep = (toCell.Col > fromCell.Col) ? 1 : -1;

			for (int j = (fromCell.Col + yDirectionStep); j != toCell.Col; j += yDirectionStep)
			{
				if (ctx.board[fromCell.Row, j] != '.')

					return false;
			}


		}
		else if (fromCell.Row != toCell.Row &&
		fromCell.Col == toCell.Col)
		{
			int xDirectionStep = (toCell.Row > fromCell.Row) ? 1 : -1;//dikey

			for (int i = fromCell.Row + xDirectionStep; i != toCell.Row; i += xDirectionStep)
			{
				if (ctx.board[i, fromCell.Col] != '.') return false;
			}
		} else
		{
			return false;
		}

		return true;
	}




	public static bool IsValidKnightMove(Cell fromCell, Cell toCell, ChessContext ctx)
	{

		if (toCell.Row == fromCell.Row - 2 &&
			((toCell.Col == fromCell.Col - 1) || (toCell.Col == fromCell.Col + 1))
			)
		{
			return true;
		}
		else if (toCell.Col == fromCell.Col - 2 &&
			((toCell.Row == fromCell.Row - 1) || toCell.Row == fromCell.Row + 1)
			)
		{
			return true;
		}
		else if (toCell.Row == fromCell.Row + 2 &&
			((toCell.Col == fromCell.Col + 1) || (toCell.Col == fromCell.Col - 1))
			)
		{
			return true;
		}
		else if (toCell.Col == fromCell.Col + 2 &&
			((toCell.Row == fromCell.Row - 1) || toCell.Row == fromCell.Row + 1)
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

		int deltaI = toCell.Row - fromCell.Row;
		int deltaJ = toCell.Col - fromCell.Col;
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
			int checkI = fromCell.Row + dirI * k;
			int checkJ = fromCell.Col + dirJ * k;
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
		int deltaI = toCell.Row - fromCell.Row;
		int deltaJ = toCell.Col - fromCell.Col;
		int dirI = deltaI > 0 ? 1 : -1;
		int dirJ = deltaJ > 0 ? 1 : -1;

		//for normal movement 
		if ((Math.Abs(fromCell.Row - toCell.Row) == 1 && Math.Abs(fromCell.Col - toCell.Col) == 1) ||
		(Math.Abs(fromCell.Row - toCell.Row) == 1 && Math.Abs(fromCell.Col - toCell.Col) == 0) ||
		(Math.Abs(fromCell.Row - toCell.Row) == 0 && Math.Abs(fromCell.Col - toCell.Col) == 1))
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

                if (fromCellOfThreatCell.stone == 'q'&& whiteKing.cellString == "d1")
				{

				}

                    // kendi taşın kendini tehtid edemeyeceği için renk değiştirdik
                    tempCtx.whiteTurn = false;
			
				if (!fromCellOfThreatCell.IsWhite && MoveError(fromCellOfThreat, whiteKing.cellString, tempCtx) == "")

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
				if (fromCellOfThreatCell.IsWhite && MoveError(fromCellOfThreat, blackKing.cellString, tempCtx) == "")
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
		newCtx.touchedCell = new Cell(ctx.touchedCell.Row, ctx.touchedCell.Col, newCtx);
		newCtx.lastFromCell = new Cell(ctx.lastFromCell.Row, ctx.lastFromCell.Col, newCtx);
		newCtx.lastToCell = new Cell(ctx.lastToCell.Row, ctx.lastToCell.Col, newCtx);

		return newCtx;
	}



	public static void CheckGameEnd(ChessContext ctx)
	{

        bool hasLegalMove = false;
        bool isWhiteTurn = ctx.whiteTurn;
        bool kingUnderThreat = isWhiteTurn ? IsWhitekingUnderThreat(ctx) : IsBlackkingUnderThreat(ctx);

		//the point in order for fromCEll
		for (int i = 0; i < 8; i++)
		{
			for (int j = 0; j < 8; j++)
			{
				char fromch = ctx.board[i, j];

				if (fromch == '.') continue;

				if ((isWhiteTurn && !IsStoneWhiteC(fromch)) || (!isWhiteTurn && IsStoneWhiteC(fromch)))
					continue;

				string from = IndexToString(i, j);
				//toCell in order
				for (int iTo = 0; iTo < 8; iTo++)
				{
					for (int jTo = 0; jTo < 8; jTo++)
					{

						if (i == iTo && j == jTo) continue;
						string to = IndexToString(iTo, jTo);

						ChessContext tempCtx = copyBoard(ctx);
						string error = MoveError(from, to, tempCtx);

						if (error == "")
						{
							hasLegalMove = true;
							break;
						}
					}
					if (hasLegalMove) break;
				}
				if (hasLegalMove) break;
			}
			if (hasLegalMove) break;
		}
        ctx.whiteWins = false;
        ctx.blackWins = false;
        ctx.drawStuation = false;
        
		if (!hasLegalMove && kingUnderThreat)
        {
            if (isWhiteTurn)
                ctx.blackWins = true;
            else
                ctx.whiteWins = true;
        }
        else if (!hasLegalMove && !kingUnderThreat)
        {
            ctx.drawStuation = true;
        }
        else if (ctx.howMuchBlackkingMoved == 50 || ctx.howMuchWhitekingMoved == 50 ||
                 (IsThereJustKing(ctx).Item1 && IsThereJustKing(ctx).Item2))
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

		if(ctx.whiteTurn & !fromCell.IsWhite)
		{
			return "Row of White Stones";
		}
		else if (!ctx.whiteTurn & fromCell.IsWhite)
		{
			return "Row of black Stones";
		}


		if (fromCell.IsWhite & (toCell.IsWhite) & !toCell.isEmpty)

        {
			
			return "Cannot Whites Move Owm Stone";
		}
		if (fromCell.IsBlack & (toCell.IsBlack & !toCell.isEmpty))
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
			movableCtx2.IsFakeMovement = true;
			if (IsValidCastlingMove(fromCell, toCell, movableCtx2))
				{
				bool isBlackKingUnderCheckInRouta = IsBlackkingUnderThreat(movableCtx2);
				bool isWhiteKingUnderCheckInRouta = IsWhitekingUnderThreat(movableCtx2);

				(Cell whiteKing,Cell blackKing) = kingLocations(movableCtx2);
				if (!IsWhitekingUnderThreat(ctx))
				{
				//long white casling
					if (toCell.cellIndex == (7, 2))
					{
						MoveStone(whiteKing.cellString, "d1", movableCtx2);
                        isWhiteKingUnderCheckInRouta = IsWhitekingUnderThreat(movableCtx2);
                        if (isWhiteKingUnderCheckInRouta)
						{
							return "White King Under Check in Long Casling Routa.";
						}

						MoveStone("d1", "c1", movableCtx2);
                        isWhiteKingUnderCheckInRouta = IsWhitekingUnderThreat(movableCtx2);
                        if (isWhiteKingUnderCheckInRouta)
						{
							return "White King Under Check in Long Casling Routa.";
						}


					}
					//short white caslting
					else if (toCell.cellIndex == (7, 6))
					{

						MoveStone(whiteKing.cellString, "f1", movableCtx2);
                        isWhiteKingUnderCheckInRouta = IsWhitekingUnderThreat(movableCtx2);
                        if (isWhiteKingUnderCheckInRouta)
						{
							return "White King Under Check in Short Casling Routa.";
						}
						MoveStone("f1", "g1", movableCtx2);
                        isWhiteKingUnderCheckInRouta = IsWhitekingUnderThreat(movableCtx2);
                        if (isWhiteKingUnderCheckInRouta)
						{
							return "White King Under Check in Short Casling Routa.";
						}
					}
				}
				

				if (!IsBlackkingUnderThreat(ctx))
				{
					//long black castling
					if (toCell.cellIndex == (0, 2))
					{
						MoveStone(blackKing.cellString, "d8", movableCtx2);
                        isBlackKingUnderCheckInRouta = IsBlackkingUnderThreat(movableCtx2);
                        if (isBlackKingUnderCheckInRouta)
						{
							return "Black King Under Check in Long Casling Routa.";
						}
						MoveStone("d8", "c8", movableCtx2);
                        isBlackKingUnderCheckInRouta = IsBlackkingUnderThreat(movableCtx2);
                        if (isBlackKingUnderCheckInRouta)
						{
							return "Black King Under Check in Long Casling Routa.";
						}

					}
					// short black castling
					else if (toCell.cellIndex == (0, 6))
					{
						MoveStone(blackKing.cellString, "f8", movableCtx2);
                        isBlackKingUnderCheckInRouta = IsBlackkingUnderThreat(movableCtx2);

                        if (isBlackKingUnderCheckInRouta)
						{
							return "Black King Under Check in Short Casling Routa.";
						}

						MoveStone("f8", "g8", movableCtx2);
                        isBlackKingUnderCheckInRouta = IsBlackkingUnderThreat(movableCtx2);

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


class Program
{

	static void Main()
	{
		ChessContext ctx = new ChessContext();
		ctx.board = new char[8, 8]{
		{ 'r', 'n', 'b', 'q', 'k', 'b', 'n', 'r' }, // 8. sıra (siyah)
		{ 'p', 'p', 'p', '.', '.', '.', 'p', 'p' },
		{ '.', '.', '.', '.', '.', '.', '.', '.' },
		{ '.', '.', '.', '.', 'R', '.', '.', '.' },
		{ '.', '.', '.', '.', '.', '.', '.', '.' },
		{ '.', '.', '.', '.', '.', '.', '.', '.' },
		{ 'P', 'P', 'P', '.', '.', '.', 'P', 'P' },
		{ 'R', '.', '.', '.', 'K', '.', '.', 'R' }  // 1. sıra (beyaz)
		};

		IInputProvider inputProvider = new ConsoleInputProvider();

        BoardHelper.PrintBoard(ref ctx);
			int i = 0;
		while (true)
		{
            BoardHelper.TakeFrom(ctx, inputProvider);
            Console.Clear();
            BoardHelper.PrintBoard(ref ctx);


            BoardHelper.TakeTo(ctx, inputProvider);
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


			//Write History Move
			BoardHelper.MoveHistoryPrinter(ctx);
			//Console.WriteLine($"Last Move: {ctx.MoveHistory.Last()}");





			//if just king is left  
			if (BoardHelper.IsThereJustKing(ctx).Item1)
			{
				Console.WriteLine($"White King moved {ctx.howMuchWhitekingMoved} times.");
			}
			if (BoardHelper.IsThereJustKing(ctx).Item2)
			{
				Console.WriteLine($"Black King moved {ctx.howMuchBlackkingMoved} times.");

			}


			BoardHelper.CheckGameEnd(ctx);
            // Check if the game has ended
            if (ctx.whiteWins)
			{
				Console.WriteLine(" Whites Won.:)");
				break;
			}
			else if (ctx.blackWins)
			{
				Console.WriteLine(" Blacks Won.:)");
				break;
			}
			else if (ctx.drawStuation)
			{
				Console.WriteLine(" There is not Winner. Draw :)");
				break;
			}
			

		}
	}

}
//eksikler
//passant move tahtanın ortasındada oldu siyah taşda