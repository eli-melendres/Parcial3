using System.ComponentModel.DataAnnotations;

namespace NumberGuessGameApi.DataTransferObjects
{
    public class RegisterPlayerRequest
    {
        [Required(ErrorMessage = "El nombre es requerido")]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es requerido")]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "La edad es requerida")]
        [Range(1, 120)]
        public int Age { get; set; }
    }
}