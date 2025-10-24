using System.ComponentModel.DataAnnotations;

namespace NumberGuessGameApi.DataTransferObjects
{
    public class GuessNumberRequest
    {
        [Required(ErrorMessage = "El ID del juego es requerido")]
        [Range(1, int.MaxValue, ErrorMessage = "El ID del juego debe ser mayor a 0")]
        public int GameId { get; set; }

        [Required(ErrorMessage = "El número intentado es requerido")]
        [Range(0, 9999, ErrorMessage = "El número debe estar entre 0 y 9999")]
        public int AttemptedNumber { get; set; }
    }
}
