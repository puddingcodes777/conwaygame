using System.ComponentModel.DataAnnotations;

namespace ConwayGameOfLife.Infrastructure.Models
{
    // Represents a saved Game of Life board state in the database
    // Stores live cells as a JSON string.
    public class GameBoard
    {
        [Key]
        public int Id { get; set; }

        // Serialized Json string of the live cells list
        [Required]
        public string LiveCellsJson { get; set; } = string.Empty; 
    }
}
