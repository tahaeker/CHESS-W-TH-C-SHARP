using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngine.Core
{
    public class BoardPrinter
    {
        public static void PrintBoard(ref ChessContext ctx)
        {
            Console.WriteLine("    a   b   c   d   e   f   g   h");

            for (int i = 0; i < ctx.Board.GetLength(0); i++)//0satır 1 stn
            {
                Console.Write("  ");
                for (int m = 0; m < Math.Sqrt(ctx.Board.Length); m++)
                {
                    Console.Write("+---");
                }
                Console.Write("+");
                Console.WriteLine();

                Console.Write(8 - i + " | ");

                for (int j = 0; j < 8; j++)
                {
                    if (ctx.Board[i, j] == ctx.Board[0, 1])
                    {

                    }
                    char stone = ctx.Board[i, j];

                    if (ctx.inputFrom != "")
                    {
                        string pos = BoardConverter.IndexToString(i, j);
                        string moveError = ErrorChecker.MoveError(ctx.inputFrom, pos, ctx);

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
    }
}
