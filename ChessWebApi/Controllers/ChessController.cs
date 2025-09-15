using ChessWebApi.Services;
using Microsoft.AspNetCore.Mvc;



namespace ChessWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChessController : ControllerBase
    {
        private readonly ChessService _chessService;
        public ChessController(ChessService chessService)
        {
            _chessService = chessService;
        }

        [HttpPost("newgame")]
        public IActionResult NewGame([FromQuery] string WhiteName, [FromQuery] string BlackName)
        {
            _chessService.StartNewGame();
            _chessService.takePlayerName(WhiteName, BlackName); 
            return Ok(new { message = "New game Started!" });
        }




        [HttpGet("board")]
        public IActionResult GetBoard()
        {
            var board = _chessService.GetBoard();
            return Ok(board);
        }

        [HttpPost("move")]
        public IActionResult MakeMove([FromQuery] string from, [FromQuery] string to)
        {
            if (_chessService.TryMove(from, to, out string message))
                return Ok(new { success = true, message });

            return BadRequest(new { success = false, message });

        }

        [HttpGet("History")]
        public IActionResult GetHistory()
        {
            var history = _chessService.GetHistory();
            return Ok(history);
        }







    }
}

