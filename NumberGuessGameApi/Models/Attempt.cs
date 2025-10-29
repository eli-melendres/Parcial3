using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata;

namespace NumberGuessGameApi.Models
{
    public class Attempt
    {
        public string ResultMessage { get; set; } = string.Empty;
        [Required]
        [Key]
        public int AttemptId { get; set; }

        [StringLength(4, MinimumLength = 4)]
        public string AttemptedNumber { get; set; } = string.Empty;
        [Required]
        public int GameId { get; set; }

        [ForeignKey("GameId")]
        public virtual Game Game { get; set; }
        public DateTime AttemptedAt { get; set; }

        [Range(0, 4)]
        public int Famas { get; set; }

        [Range(0, 4)]
        public int Picas { get; set; }

        public Attempt()
        {
            // Inicializa la fecha del intento con la hora actual
            AttemptedAt = DateTime.Now;
        }

        //Verifica si este intento fue exitoso (ganador)
        public bool IsWinningAttempt()
        {
            return Famas == 4;
        }
    
        //Calcula el total de dígitos correctos(Famas + Picas)
        public int GetTotalCorrectDigits()
        {
            return Famas + Picas;
        }
        //Verifica si el intento no tiene ningún dígito correcto
        public bool HasNoCorrectDigits()
        {
            return Famas == 0 && Picas == 0;
        }


        public string GetFriendlyResult()
        {
            if (IsWinningAttempt())
            {
                return "🎉¡Ganaste! Adivinaste el número secreto.";
            }

            if (HasNoCorrectDigits())
            {
                return "❌ Ningún dígito es correcto.";
            }

            if (Famas == 0)
            {
                return $"🔄 Tienes {Picas} dígito(s) correcto(s) pero en posición incorrecta.";
            }

            if (Picas == 0)
            {
                return $"✅ Tienes {Famas} dígito(s) en la posición correcta.";
            }

            return $"📊 {Famas} dígito(s) en posición correcta, {Picas} en posición incorrecta.";
        }

        //Obtiene una representación en texto del intento
        public override string ToString()
        {
            return $"Attempt #{AttemptId} - GameId: {GameId} - Number: {AttemptedNumber} - " +
                   $"Famas: {Famas}, Picas: {Picas} - {AttemptedAt:yyyy-MM-dd HH:mm:ss}";
        }
    }
}
