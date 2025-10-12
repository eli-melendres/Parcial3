using NumberGuessGameApi.DataTransferObjects;

namespace NumberGuessGameApi.Migrations.Services
{
    public interface IGameService
    {
        Task<RegisterPlayerResponse> RegisterPlayerAsync(RegisterPlayerRequest request);
        Task<StartGameResponse> StartGameAsync(StartGameRequest request);
        Task<GuessNumberResponse> GuessNumberAsync(GuessNumberRequest request);
        Task<bool> PlayerExistsAsync(int playerId);
        Task<bool> HasActiveGameAsync(int playerId);
        Task<bool> GameExistsAsync(int gameId);
        Task<bool> IsGameFinishedAsync(int gameId);

    }
}
