using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NumberGuessGameApi.Models
{
    [Table("Players")]
    public class Player
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PlayerId { get; set; }

        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } 

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; } 

        [Required]
        [Range(1, 120)]
        public int Age { get; set; }

        [Required]
        public DateTime RegisteredAt { get; set; }

        public virtual ICollection<Game> Games { get; set; }

        public Player()
        {
            RegisteredAt = DateTime.Now;
            Games = new List<Game>();
        }
    }
}