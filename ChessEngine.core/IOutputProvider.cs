using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngine.Core
{
    public interface IOutputProvider
    {
        void Write(string message);
        void WriteLine(string message);
    }
}
