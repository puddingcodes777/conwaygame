using System.Collections.Generic;

namespace ConwayGameOfLife.Core.DTOs
{
    /// Represents the result of a Game of Life simulation generation.
    public class GenerationResult
    {
        /// The set of live cells after simulation.
        public HashSet<(int X, int Y)> LiveCells { get; set; } = new();

        // The number of Periodic
        public int Periodic { get; set; } = 0;

        // The number of next steps
        public int NextStepNum { get; set; } = 0;
    }


}
