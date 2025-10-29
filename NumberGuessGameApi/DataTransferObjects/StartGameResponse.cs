namespace NumberGuessGameApi.DataTransferObjects
{
    public class StartGameResponse
    {
        public int GameId { get; set; }
        public int PlayerId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}