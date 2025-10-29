using System.ComponentModel.DataAnnotations;

namespace NumberGuessGameApi.DataTransferObjects
{
    public class StartGameRequest
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int PlayerId { get; set; }
    }
}