namespace NumberGuessGameApi.Models
{
    public class Game
    {
        public Guid GameId { get; set; } = Guid.NewGuid();
        public int SecretNumber { get; set; }
        public int Attempts { get; set; } = 0;
        public bool IsActive { get; set; } = true;
    }
}
