using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NumberGuessGameApi.DataTransferObjects;
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

        //Metodo que realiza un intento de adivinar el número secreto
        [HttpPost("guess")]
        public async Task<IActionResult> GuessNumber([FromBody] GuessNumberRequest request)
        {
            try
            {
                _logger.LogInformation("Intento de adivinanza. GameId: {GameId}, Número: {Number}", request?.GameId, request?.AttemptedNumber);

                //Validacion de datos requeridos
                if (request == null)
                {
                    _logger.LogWarning("Request de intento es null");
                    return BadRequest(new { Message = "Los datos del intento son requeridos" });
                }

                if (request.GameId <= 0)
                {
                    _logger.LogWarning("GameId inválido: {GameId}", request.GameId);
                    return BadRequest(new { Message = "El ID del juego es requerido y debe ser válido" });
                }

                // Validar formato del número (4 dígitos)
                var numberStr = request.AttemptedNumber.ToString();
                if (numberStr.Length != 4)
                {
                    _logger.LogWarning("Número no tiene 4 dígitos: {Number}", request.AttemptedNumber);
                    return BadRequest(new{Message = "El número debe tener exactamente 4 dígitos"});
                }
                // Validar que no haya dígitos repetidos
                if (numberStr.Length != numberStr.Distinct().Count())
                {
                    _logger.LogWarning("Número tiene dígitos repetidos: {Number}", request.AttemptedNumber);
                    return BadRequest(new {Message = "El número no debe tener dígitos repetidos" });
                }

                // Verificar si el juego existe
                var gameExists = await _gameService.GameExistsAsync(request.GameId);
                if (!gameExists)
                {
                    _logger.LogWarning("Juego no encontrado: {GameId}", request.GameId);
                    return NotFound(new  {  Message = $"El juego con ID {request.GameId} no existe"});
                }

                // Verificar si el juego ya finalizó
                var gameFinished = await _gameService.IsGameFinishedAsync(request.GameId);
                if (gameFinished)
                {
                    _logger.LogWarning("Intento en juego finalizado. GameId: {GameId}", request.GameId);
                    return BadRequest(new
                    { Message = $"El juego {request.GameId} ya ha finalizado." });
                }

                // Procesar el intento
                var response = await _gameService.GuessNumberAsync(request);

                _logger.LogInformation("Intento procesado. GameId: {GameId}, Mensaje: {Message}", response.GameId, response.Message);

                return Ok(response);
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "Error al procesar intento. GameId: {GameId}, Número: {Number}",request?.GameId, request?.AttemptedNumber);
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Error interno del servidor" });

            }
        }
        //Metodo para iniciar un nuevo juego
     {
           [HttpPost("start")]
           public IActionResult Start()
         {
            var game = _gameService.StartGame();

            var response = new StartGameResponse
            {
                GameId = game.GameId,
                Message = "Nuevo juego iniciado. ¡Adivina el número entre 1 y 100!"
            };

            return Ok(response);
        }
    }
}
