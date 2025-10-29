using GameCore;
using Microsoft.EntityFrameworkCore;
using NumberGuessGameApi.Data;
using NumberGuessGameApi.DataTransferObjects;
using NumberGuessGameApi.Models;
using System.Diagnostics;

namespace NumberGuessGameApi.Migrations.Services
{
    public class GameService : IGameService
    {
        private readonly GameDbContext _context;
        private readonly ILogger<GameService> _logger;
        public GameService(GameDbContext context,ILogger<GameService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> GameExistsAsync(int gameId)
        {
            _logger.LogDebug("Verificando existencia del juego: GameId {GameId}", gameId);
            return await _context.Games.AnyAsync(g => g.GameId == gameId);
        }

        //Procesa un intento de adivinanza usando GuessCore
        public async Task<GuessNumberResponse> GuessNumberAsync(GuessNumberRequest request)
        {
            try
            {
                _logger.LogInformation("═══ PROCESANDO INTENTO DE ADIVINANZA ═══");
                _logger.LogInformation("GameId: {GameId} | Número intentado: {AttemptedNumber}",request.GameId, request.AttemptedNumber);

                // Obtener juego
                _logger.LogDebug("Buscando juego en la base de datos...");
                var game = await _context.Games.FirstOrDefaultAsync(g => g.GameId == request.GameId);

                if (game == null)
                {
                    _logger.LogError("JUEGO NO ENCONTRADO: GameId {GameId}", request.GameId);
                    throw new InvalidOperationException($"El juego {request.GameId} no existe");
                }

                _logger.LogDebug("Juego encontrado. Número secreto: {SecretNumber}", game.SecretNumber);

                // Validar con GuessCore
                _logger.LogDebug("Validando intento con GuessCore...");


                var result = Evaluator.ValidateAttempt(game.SecretNumber, request.AttemptedNumber.ToString());

                // Registrar el intento en la base de datos
                var attempt = new Attempt
                {
                    GameId = request.GameId,
                    AttemptedNumber = request.AttemptedNumber.ToString(),
                    Famas = result.Fama,
                    Picas = result.Pica,
                    ResultMessage = result.Message,
                    AttemptedAt = DateTime.Now
                };

                _context.Attempts.Add(attempt);

                if (result.Fama == 4)
                {
                    game.IsFinished = true;
                    game.FinishedAt = DateTime.Now;
                    _context.Games.Update(game);
                    _logger.LogInformation("¡JUEGO GANADO! GameId: {GameId}, Número: {Number}, Intentos totales: {Attempts}",
                       request.GameId, request.AttemptedNumber,
                       await _context.Attempts.CountAsync(a => a.GameId == request.GameId) + 1);

                    _logger.LogInformation("[AUDITORIA] - Juego completado | GameId: {GameId} | PlayerId: {PlayerId} | Intentos: {Attempts} | Duración: {Duration}",
                        game.GameId, game.PlayerId,
                        await _context.Attempts.CountAsync(a => a.GameId == request.GameId) + 1,
                        (DateTime.Now - game.CreatedAt).TotalMinutes);
                }
                else
                {
                    _logger.LogInformation("Intento registrado. GameId: {GameId}, Famas: {Famas}, Picas: {Picas}",
                        request.GameId, result.Fama, result.Pica);
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation("[AUDITORIA] - Intento registrado | GameId: {GameId} | Intento: {Number} | Famas: {Famas} | Picas: {Picas} | Fecha: {Date}",
                   request.GameId, request.AttemptedNumber.ToString(), result.Fama, result.Pica, DateTime.Now);

                return new GuessNumberResponse
                {
                    GameId = request.GameId,
                    AttemptedNumber = request.AttemptedNumber,
                    Message = result.Message
                };


                // TEMPORAL: Retornar mensaje indicando que falta el paquete
                throw new NotImplementedException("El método GuessNumber requiere el paquete GameCore (ESCMB.GuessCore). Tu compañera debe instalarlo.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al procesar intento. GameId: {GameId}, Número: {Number}",
                   request.GameId, request.AttemptedNumber);
                throw; 
            }

        }

        public async Task<bool> HasActiveGameAsync(int playerId)
        {
            _logger.LogDebug("Verificando juegos activos para: PlayerId {PlayerId}", playerId);
            return await _context.Games.AnyAsync(g => g.PlayerId == playerId && !g.IsFinished);
        }

        public async Task<bool> IsGameFinishedAsync(int gameId)
        {
            _logger.LogDebug("Verificando estado del juego: GameId {GameId}", gameId);
            var game = await _context.Games.FirstOrDefaultAsync(g => g.GameId == gameId);
            return game?.IsFinished ?? false;
        }

        public async Task<bool> PlayerExistsAsync(int playerId)
        {
            _logger.LogDebug("Verificando existencia del jugador: PlayerId {PlayerId}", playerId);
            return await _context.Players.AnyAsync(p => p.PlayerId == playerId);
        }

        public async Task<RegisterPlayerResponse> RegisterPlayerAsync(RegisterPlayerRequest request)
        {
            try
            {
                _logger.LogInformation("═══ REGISTRANDO JUGADOR ═══");
                _logger.LogInformation("Nombre: {FirstName} {LastName}, Edad: {Age}",
                    request.FirstName, request.LastName, request.Age);

                var existingPlayer = await _context.Players
                    .FirstOrDefaultAsync(p => p.FirstName == request.FirstName &&
                                             p.LastName == request.LastName);

                if (existingPlayer != null)
                {
                    _logger.LogWarning("Jugador ya registrado");
                    throw new InvalidOperationException(
                        $"El jugador {request.FirstName} {request.LastName} ya se encuentra registrado");
                }

                var player = new Player
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Age = request.Age,
                    RegisteredAt = DateTime.Now
                };

                _context.Players.Add(player);
                await _context.SaveChangesAsync();

                _logger.LogInformation("[AUDITORIA] - Jugador registrado | PlayerId: {PlayerId}", player.PlayerId);

                return new RegisterPlayerResponse { PlayerId = player.PlayerId };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al registrar jugador");
                throw;
            }
        }
        public async Task<StartGameResponse> StartGameAsync(StartGameRequest request)
        {
            try
            {
                _logger.LogInformation("═══ INICIANDO JUEGO ═══");
                _logger.LogInformation("PlayerId: {PlayerId}", request.PlayerId);

                if (!await PlayerExistsAsync(request.PlayerId))
                {
                    throw new InvalidOperationException($"El jugador con ID {request.PlayerId} no existe");
                }

                if (await HasActiveGameAsync(request.PlayerId))
                {
                    throw new InvalidOperationException("El jugador ya tiene un juego activo. Debe finalizar el juego anterior antes de iniciar uno nuevo");
                }

                var secretNumber = GenerateSecretNumber();
                _logger.LogDebug("Número secreto generado: {SecretNumber}", secretNumber);

                var game = new Game
                {
                    PlayerId = request.PlayerId,
                    SecretNumber = secretNumber,
                    CreatedAt = DateTime.Now,
                    IsFinished = false
                };

                _context.Games.Add(game);
                await _context.SaveChangesAsync();

                _logger.LogInformation("[AUDITORIA] - Juego iniciado | GameId: {GameId} | PlayerId: {PlayerId} | Fecha: {Date}",
                    game.GameId, game.PlayerId, game.CreatedAt);

                return new StartGameResponse
                {
                    GameId = game.GameId,
                    PlayerId = game.PlayerId,
                    CreatedAt = game.CreatedAt
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al iniciar juego para PlayerId: {PlayerId}", request.PlayerId);
                throw;
            }
        }

        // Genera un número secreto de 4 dígitos sin repetir
        private string GenerateSecretNumber()
        {
            var random = new Random();
            var digits = Enumerable.Range(0, 10).OrderBy(x => random.Next()).Take(4).ToArray();
            var secretNumber = string.Join("", digits);

            _logger.LogDebug("Número secreto generado: {SecretNumber}", secretNumber);

            return secretNumber;
        }
    }
}
