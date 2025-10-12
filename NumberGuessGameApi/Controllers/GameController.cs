using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NumberGuessGameApi.Migrations.Services;
using Serilog;

namespace NumberGuessGameApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GameController : ControllerBase
    {
        
        private readonly IGameService _gameService;
        private readonly ILogger<GameController> _logger;

        public GameController(IGameService gameService, ILogger<GameController> logger)
        {
            _gameService = gameService;
            _logger = logger;
        }

        //Metodo

        //Metodo

        //Metodo
        
    }
}
