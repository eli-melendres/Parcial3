namespace NumberGuessGameApi.DataTransferObjects
{
    public class StartGameResponse
    {
        public Guid GameId { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
