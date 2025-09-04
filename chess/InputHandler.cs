using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chess
{
    internal class InputHandler
    {
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
                    ctx.touchedCell = BoardConverter.StringToCell(ctx.inputFrom, ctx);

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


        public static int IsValidFromToCondition(string from, string to, ChessContext ctx)
        {

            if (string.IsNullOrEmpty(from))
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


    }
}
