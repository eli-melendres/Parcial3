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
        // Endpoint para registrar un nuevo jugador
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterPlayerRequest request)
        {
            try
            {
                _logger.LogInformation("Intento de registro. Nombre: {FirstName} {LastName}",
                    request?.FirstName, request?.LastName);

                if (request == null)
                {
                    _logger.LogWarning("Request de registro es null");
                    return BadRequest(new { Message = "Los datos del jugador son requeridos" });
                }

                if (string.IsNullOrWhiteSpace(request.FirstName))
                {
                    _logger.LogWarning("Nombre vacío");
                    return BadRequest(new { Message = "El nombre es requerido" });
                }

                if (string.IsNullOrWhiteSpace(request.LastName))
                {
                    _logger.LogWarning("Apellido vacío");
                    return BadRequest(new { Message = "El apellido es requerido" });
                }

                if (request.Age <= 0 || request.Age > 120)
                {
                    _logger.LogWarning("Edad inválida: {Age}", request.Age);
                    return BadRequest(new { Message = "La edad debe estar entre 1 y 120 años" });
                }

                var response = await _gameService.RegisterPlayerAsync(request);
                _logger.LogInformation("Jugador registrado exitosamente. PlayerId: {PlayerId}", response.PlayerId);

                return Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Intento de registro duplicado");
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al registrar jugador");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { Message = "Error interno del servidor" });
            }
        }

        // Endpoint para iniciar un juego
        [HttpPost("start")]
        public async Task<IActionResult> StartGame([FromBody] StartGameRequest request)
        {
            try
            {
                _logger.LogInformation("Intento de iniciar juego. PlayerId: {PlayerId}",
                    request?.PlayerId);

                if (request == null || request.PlayerId <= 0)
                {
                    return BadRequest(new { Message = "PlayerId inválido" });
                }

                if (!await _gameService.PlayerExistsAsync(request.PlayerId))
                {
                    return NotFound(new { Message = $"El jugador con ID {request.PlayerId} no está registrado" });
                }

                var response = await _gameService.StartGameAsync(request);
                _logger.LogInformation("Juego iniciado. GameId: {GameId}, PlayerId: {PlayerId}",
                    response.GameId, response.PlayerId);

                return Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Error de negocio al iniciar juego");
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al iniciar juego");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { Message = "Error interno del servidor" });
            }
        }

    }
}
