using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngine.Core
{
    public class BoardConverter
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

        public static Cell IndexToCell(int row, int col, ChessContext ctx)
        {
            string pos = IndexToString(row, col);
            Cell posCell = StringToCell(pos, ctx);
            return posCell;
        }


    }
}
