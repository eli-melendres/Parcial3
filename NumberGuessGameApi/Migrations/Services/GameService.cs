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

        public Task<bool> GameExistsAsync(int gameId)
        {
            throw new NotImplementedException();
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
                var result = Evaluator.ValidateAttempt(game.SecretNumber, request.AttemptedNumber);
                //Console.WriteLine(result);

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

                    // Auditoría de juego completado
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
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al procesar intento. GameId: {GameId}, Número: {Number}",
                   request.GameId, request.AttemptedNumber);
            }
           
        }

        public Task<bool> HasActiveGameAsync(int playerId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsGameFinishedAsync(int gameId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> PlayerExistsAsync(int playerId)
        {
            throw new NotImplementedException();
        }

        public Task<RegisterPlayerResponse> RegisterPlayerAsync(RegisterPlayerRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<StartGameResponse> StartGameAsync(StartGameRequest request)
        {
            throw new NotImplementedException();
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
