using NumberGuessGameApi.Data;
using NumberGuessGameApi.DataTransferObjects;

namespace NumberGuessGameApi.Migrations.Services
{
    public class GameService : IGameService
    {
        private readonly GameDbContext _context;
        private readonly ILogger<GameService> _logger;
        public GameService(ILogger<GameService> logger)
        {
            _logger = logger;
        }

        public Task<bool> GameExistsAsync(int gameId)
        {
            throw new NotImplementedException();
        }

        public Task<GuessNumberResponse> GuessNumberAsync(GuessNumberRequest request)
        {
            throw new NotImplementedException();
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
    }
}
