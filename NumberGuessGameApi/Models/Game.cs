using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NumberGuessGameApi.Models
{
    [Table("Games")]
    public class Game
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int GameId { get; set; }

        [Required]
        public int PlayerId { get; set; }

        [Required]
        [MaxLength(4)]
        public string SecretNumber { get; set; } = string.Empty;

        [Required]
        public DateTime CreatedAt { get; set; }

        public DateTime? FinishedAt { get; set; }

        [Required]
        public bool IsFinished { get; set; }

        [ForeignKey("PlayerId")]
        public virtual Player Player { get; set; }

        public virtual ICollection<Attempt> Attempts { get; set; }

        public Game()
        {
            CreatedAt = DateTime.Now;
            IsFinished = false;
            Attempts = new List<Attempt>();
        }
    }
}
