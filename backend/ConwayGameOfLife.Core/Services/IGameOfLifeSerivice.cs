using ConwayGameOfLife.Core.DTOs;

namespace ConwayGameOfLife.Core.Services
{
    /// Defines the contract for Game of Life simulation services.
    public interface IGameOfLifeService
    {
        /// Simulate the Game of Life for the specified number of generations.
        /// A GenerationResult containing the final live cells.
        Task<GenerationResult> SimulateAsync(HashSet<(int X, int Y)> liveCells, int generations, bool isFinal);
    }
}