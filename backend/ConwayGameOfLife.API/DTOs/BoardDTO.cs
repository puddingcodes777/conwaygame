using ConwayGameOfLife.Infrastructure.Models;
using System.ComponentModel.DataAnnotations;

namespace ConwayGameOfLife.API.DTOs
{
    /// Request DTO for Game of Life simulation.
    public class SimulateRequest
    {
        [Required]
        public List<CellDto>? LiveCells { get; set; }

        [Range(0, int.MaxValue)]
        public int MaxStepNum { get; set; }
    }

    // Response DTO for Game of Life simulation
    public class SimulateResponse
    {
        [Required]
        public List<CellDto>? LiveCells { get; set; }
        public int PeriodicNum { get; set; }
        public int GenerateStepNum { get; set; }
    }

    // Response DTO after Game Board Uploading
    public class UploadBoardResponse
    {
        [Required]
        public int BoardId { get; set; }
    }

    // Request DTO for Game Board Uploading
    public class UploadBoardRequest
    {
        [Required]
        public List<CellDto>? LiveCells { get; set; }
    }

    // Response DTO for fetching Board Data
    public class GetBoardResponse
    {
        public List<CellDto> LiveCells { get; set; } = new();
    }

    // Response DTO for fetching All Board
    public class GetAllBoardResponse
    { 
        public List<GameBoard> GameBoards { get; set; }
    }

    // Request DTO for generating 1 or N State Ahead
    public class GenerateNAheadRequest
    {   
        [Required]
        public int GenerationNum { get; set; }
    }

    // Response DTO for generating 1 or N State Ahead
    public class GenerateNAheadResponse
    {
        public List<CellDto> LiveCells { get; set; } = new();
    }

    /// Simple DTO representing a cell coordinate.
    public class CellDto
    {
        public int X { get; set; }
        public int Y { get; set; }
    }
}