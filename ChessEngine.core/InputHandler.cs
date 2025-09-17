using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngine.Core
{
    public class InputHandler
    {
        public static void TakeFrom(ChessContext ctx, IInputProvider inputProvider)
        {
            string errorMessage;
            do
            {
                Console.WriteLine("Hangi taşı oynatmak istiyorsunuz? ");
                ctx.inputFrom = inputProvider.ReadLine();
                ctx.inputFrom = ctx.inputFrom.ToLower();
                IsValidFromToCondition(ctx.inputFrom, ctx.inputTo, ctx);
                errorMessage = ctx.InputFromError;
                
                bool IstouchedMovable = MoveValidator.IsTouchedCellMovable(ctx.inputFrom, ctx);
               
                
                var (isFromValid, _) = IsValidFromToCondition(ctx.inputFrom, null, ctx);
                if (!isFromValid )
                {
                    errorMessage = ctx.InputFromError;
                    Console.WriteLine(errorMessage);
                }
                else if (!IstouchedMovable)
                {
                    errorMessage = "The selected piece cannot be moved! Please select another piece.";
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
            string errorMessage;
            
            do
            {
                Console.WriteLine("Nereye taşımak istiyorsunuz? ");
                ctx.inputTo = inputProvider.ReadLine();
                ctx.inputTo = ctx.inputTo.ToLower();
                IsValidFromToCondition(ctx.inputFrom, ctx.inputTo, ctx);
                errorMessage = ctx.InputToError;

                var (_, isToValid) = IsValidFromToCondition(ctx.inputFrom, ctx.inputTo, ctx);
                if (!isToValid)
                {
                    errorMessage = ctx.InputToError;
                    Console.WriteLine(errorMessage);
                }
                else
                {
                    errorMessage = "";
                }
                Console.WriteLine();
            } while (errorMessage != "");
        }

        public static (bool, bool) IsValidFromToCondition(string from, string to, ChessContext ctx)
        {
            // From validation
            bool isFromValid = true;
            ctx.InputFromError= "";

            if (string.IsNullOrEmpty(from))
            {
                ctx.InputFromError = "From cannot be empty!!";
                isFromValid = false;
            }
            else if (from.Length != 2)
            {
                ctx.InputFromError = "From must be 2 characters long!!";
                isFromValid = false;
            }
            else
            {
                char letter = from[0];
                int number = from[1] - '0';
                if (!(letter >= 'a' && letter <= 'h'))
                {
                    ctx.InputFromError = "Row (X Axis) of From must be between a-h";
                    isFromValid = false;
                }
                else if (!(number >= 1 && number <= 8))
                {
                    ctx.InputFromError = "Col (Y Axis) of From must be between 1-8";
                    isFromValid = false;
                }
            }

            ctx.IsInputFromValid = isFromValid;
            ctx.InputToError= "";


            // To validation
            bool isToValid = true;

            if (!string.IsNullOrEmpty(to))
            {
                if (to.Length != 2)
                {
                    ctx.InputToError = "To must be 2 characters long!!";
                    isToValid = false;
                }
                else
                {
                    char letter = to[0];
                    int number = to[1] - '0';
                    if (!(letter >= 'a' && letter <= 'h'))
                    {
                        ctx.InputToError = "To must be between a-h!!";
                        isToValid = false;
                    }
                    else if (!(number >= 1 && number <= 8))
                    {
                        ctx.InputToError = "To must be between 1-8!!";
                        isToValid = false;
                    }
                    else if (from != null && from == to)
                    {
                        ctx.InputToError = "From and To cannot be the same!!";
                        isToValid = false;
                    }
                }
            }
            else if (to != null)
            {
                ctx.InputToError = "To cannot be empty!!";
                isToValid = false;
            }

            ctx.IsInputToValid = isToValid;

            return (isFromValid, isToValid);
        }
    }
}
