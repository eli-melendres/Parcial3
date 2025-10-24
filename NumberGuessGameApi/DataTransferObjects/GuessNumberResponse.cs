namespace NumberGuessGameApi.DataTransferObjects
{
    public class GuessNumberResponse
    {
        public int GameId { get; set; }
        public int AttemptedNumber { get; set; }
        public string Message { get; set; }
        public GuessNumberResponse()
        {
            
        }
    }
}
