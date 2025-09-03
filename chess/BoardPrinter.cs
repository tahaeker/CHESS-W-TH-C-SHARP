using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chess
{
    internal class BoardPrinter
    {
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
                        string pos = BoardHelper.IndexToString(i, j);
                        string moveError = BoardHelper.MoveError(ctx.inputFrom, pos, ctx);

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
