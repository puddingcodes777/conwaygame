using ConwayGameOfLife.Core.DTOs;
using ConwayGameOfLife.Core.Services;
using ConwayGameOfLife.Core.Helper;

namespace ConwayGameOfLife.Infrastructure.Services
{
    public class HashlifeGameOfLifeService : IGameOfLifeService
    {
        private readonly Dictionary<QuadNode, QuadNode> _nodeCache = new Dictionary<QuadNode, QuadNode>();
        private readonly Dictionary<(int NodeId, int Generations), QuadNode> _resultCache = new Dictionary<(int, int), QuadNode>();
        private readonly QuadNode _aliveNode;
        private readonly QuadNode _deadNode;
        private int _currentStepNum = 0;

        // Cache management
        private const int MAX_CACHE_SIZE = 100000;
        private int _cacheCleanupCounter = 0;

        public HashlifeGameOfLifeService()
        {
            _aliveNode = new QuadNode(true);
            _deadNode = new QuadNode(false);
            _nodeCache[_aliveNode] = _aliveNode;
            _nodeCache[_deadNode] = _deadNode;
        }

        public async Task<GenerationResult> SimulateAsync(HashSet<(int X, int Y)> liveCells, int generations, bool isFinal)
        {
            if (liveCells == null) throw new ArgumentNullException(nameof(liveCells));
            if (generations < 0) throw new ArgumentOutOfRangeException(nameof(generations), "Generations must be non-negative.");

            var result = await Task.Run(() => Simulate(liveCells, generations, isFinal));
            return result;
        }

        private GenerationResult Simulate(HashSet<(int X, int Y)> liveCells, int generations, bool isFinal)
        {
            try
            {
                if (generations == 0)
                {
                    return new GenerationResult
                    {
                        LiveCells = new HashSet<(int X, int Y)>(liveCells),
                        NextStepNum = _currentStepNum,
                        Periodic = 0
                    };
                }

                // Clean cache periodically
                if (++_cacheCleanupCounter % 1000 == 0)
                {
                    CleanupCaches();
                }

                // Use direct simulation for small cases or when Hashlife might be inefficient
                if (generations <= 5 || liveCells.Count < 20 || ShouldUseDirect(liveCells))
                {
                    return SimulateDirect(liveCells, generations, isFinal);
                }

                return SimulateHashlife(liveCells, generations, isFinal);
            }
            catch (Exception ex)
            {
                // Fallback to direct simulation on any error
                return SimulateDirect(liveCells, generations, isFinal);
            }
        }

        private bool ShouldUseDirect(HashSet<(int X, int Y)> liveCells)
        {
            if (liveCells.Count == 0) return true;

            // Check if pattern is very sparse or has extreme coordinates
            var minX = liveCells.Min(c => c.X);
            var maxX = liveCells.Max(c => c.X);
            var minY = liveCells.Min(c => c.Y);
            var maxY = liveCells.Max(c => c.Y);

            int width = maxX - minX + 1;
            int height = maxY - minY + 1;
            int area = width * height;

            // Use direct if pattern is very sparse or coordinates are extreme
            return (liveCells.Count * 10 < area) ||
                   (Math.Abs(minX) > 10000 || Math.Abs(maxX) > 10000 ||
                    Math.Abs(minY) > 10000 || Math.Abs(maxY) > 10000);
        }

        private void CleanupCaches()
        {
            if (_nodeCache.Count > MAX_CACHE_SIZE)
            {
                // Keep only the most recently used entries (simplified cleanup)
                var toRemove = _nodeCache.Count - MAX_CACHE_SIZE / 2;
                var keysToRemove = _nodeCache.Keys.Take(toRemove).ToList();
                foreach (var key in keysToRemove)
                {
                    _nodeCache.Remove(key);
                }
            }

            if (_resultCache.Count > MAX_CACHE_SIZE)
            {
                var toRemove = _resultCache.Count - MAX_CACHE_SIZE / 2;
                var keysToRemove = _resultCache.Keys.Take(toRemove).ToList();
                foreach (var key in keysToRemove)
                {
                    _resultCache.Remove(key);
                }
            }
        }

        private GenerationResult SimulateDirect(HashSet<(int X, int Y)> liveCells, int generations, bool isFinal)
        {
            var currentState = new HashSet<(int X, int Y)>(liveCells);
            var visitedStates = new Dictionary<string, (int Generation, HashSet<(int X, int Y)> State)>();
            int periodicLength = 0;
            bool periodDetected = false;

            for (int gen = 0; gen < generations; gen++)
            {
                if (isFinal && gen % 100 == 0) // Periodic check every 100 generations for performance
                {
                    string stateHash = ComputeHashSetHash(currentState);
                    if (visitedStates.TryGetValue(stateHash, out var previousState))
                    {
                        periodicLength = gen - previousState.Generation;
                        periodDetected = true;

                        int remainingGens = (generations - gen) % periodicLength;
                        for (int i = 0; i < remainingGens; i++)
                        {
                            currentState = NextGenerationDirect(currentState);
                        }
                        break;
                    }
                    else
                    {
                        visitedStates[stateHash] = (gen, new HashSet<(int X, int Y)>(currentState));
                    }
                }

                currentState = NextGenerationDirect(currentState);
            }

            _currentStepNum += generations;
            return new GenerationResult
            {
                LiveCells = currentState,
                NextStepNum = _currentStepNum,
                Periodic = periodDetected ? periodicLength : 0
            };
        }

        private HashSet<(int X, int Y)> NextGenerationDirect(HashSet<(int X, int Y)> current)
        {
            var neighborCounts = new Dictionary<(int X, int Y), int>();

            foreach (var cell in current)
            {
                for (int dx = -1; dx <= 1; dx++)
                {
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        if (dx == 0 && dy == 0) continue;
                        var neighbor = (cell.X + dx, cell.Y + dy);
                        neighborCounts[neighbor] = neighborCounts.GetValueOrDefault(neighbor, 0) + 1;
                    }
                }
            }

            var nextGen = new HashSet<(int X, int Y)>();
            foreach (var kvp in neighborCounts)
            {
                var cell = kvp.Key;
                int count = kvp.Value;
                if (count == 3 || (count == 2 && current.Contains(cell)))
                {
                    nextGen.Add(cell);
                }
            }

            return nextGen;
        }

        private GenerationResult SimulateHashlife(HashSet<(int X, int Y)> liveCells, int generations, bool isFinal)
        {
            try
            {
                // Normalize coordinates to reduce coordinate space
                var normalizedCells = NormalizeCells(liveCells);
                var rootNode = ConvertToQuadNodeWithPadding(normalizedCells, generations);

                if (rootNode == null)
                {
                    return new GenerationResult
                    {
                        LiveCells = new HashSet<(int X, int Y)>(),
                        NextStepNum = _currentStepNum + generations,
                        Periodic = 0
                    };
                }

                var visitedStates = new Dictionary<int, (int Generation, QuadNode Node)>();
                int periodicLength = 0;
                bool periodDetected = false;
                int remainingGenerations = generations;

                while (remainingGenerations > 0)
                {
                    // Expand if necessary before processing
                    while (rootNode.Level < 3 || NeedsExpansion(rootNode))
                    {
                        rootNode = ExpandNode(rootNode);
                        if (rootNode.Level > 10) break; // Prevent excessive expansion
                    }

                    // Check for periodicity
                    if (isFinal && visitedStates.TryGetValue(rootNode.NodeId, out var previousState))
                    {
                        periodicLength = generations - remainingGenerations - previousState.Generation;
                        periodDetected = true;

                        int finalSteps = remainingGenerations % periodicLength;
                        for (int i = 0; i < finalSteps; i++)
                        {
                            rootNode = AdvanceHashlife(rootNode, 1);
                        }
                        break;
                    }
                    else if (isFinal)
                    {
                        visitedStates[rootNode.NodeId] = (generations - remainingGenerations, rootNode);
                    }

                    // Calculate optimal step size
                    int maxPossibleAdvance = rootNode.Level >= 2 ? (1 << (rootNode.Level - 2)) : 1;
                    int stepsToAdvance = Math.Min(maxPossibleAdvance, remainingGenerations);

                    // For large remaining generations, use powers of 2
                    if (remainingGenerations > stepsToAdvance * 2)
                    {
                        stepsToAdvance = Math.Min(stepsToAdvance, GetLargestPowerOf2(remainingGenerations));
                    }

                    rootNode = AdvanceHashlife(rootNode, stepsToAdvance);
                    remainingGenerations -= stepsToAdvance;
                }

                var resultCells = ConvertToHashSet(rootNode);
                _currentStepNum += generations;

                return new GenerationResult
                {
                    LiveCells = resultCells,
                    NextStepNum = _currentStepNum,
                    Periodic = periodDetected ? periodicLength : 0
                };
            }
            catch (Exception)
            {
                // Fallback to direct simulation
                return SimulateDirect(liveCells, generations, isFinal);
            }
        }

        private HashSet<(int X, int Y)> NormalizeCells(HashSet<(int X, int Y)> liveCells)
        {
            if (liveCells.Count == 0) return liveCells;

            var minX = liveCells.Min(c => c.X);
            var minY = liveCells.Min(c => c.Y);

            return new HashSet<(int X, int Y)>(
                liveCells.Select(c => (c.X - minX, c.Y - minY))
            );
        }

        private int GetLargestPowerOf2(int n)
        {
            int power = 1;
            while (power * 2 <= n)
                power *= 2;
            return power;
        }

        private bool NeedsExpansion(QuadNode node)
        {
            // Check if any activity is near the borders
            return HasActivityNearBorder(node.NW) || HasActivityNearBorder(node.NE) ||
                   HasActivityNearBorder(node.SW) || HasActivityNearBorder(node.SE);
        }

        private bool HasActivityNearBorder(QuadNode node)
        {
            if (node.IsLeaf) return node.IsAlive;
            if (node.Level <= 1) return !IsEmpty(node);

            // Check if any of the border regions have activity
            return !IsEmpty(node.NW) || !IsEmpty(node.NE) || !IsEmpty(node.SW) || !IsEmpty(node.SE);
        }

        private bool IsEmpty(QuadNode node)
        {
            if (node == _deadNode) return true;
            if (node.IsLeaf) return !node.IsAlive;
            return IsEmpty(node.NW) && IsEmpty(node.NE) && IsEmpty(node.SW) && IsEmpty(node.SE);
        }

        private QuadNode ConvertToQuadNodeWithPadding(HashSet<(int X, int Y)> liveCells, int generations)
        {
            if (liveCells.Count == 0)
                return CreateEmpty(Math.Max(3, (int)Math.Ceiling(Math.Log2(generations + 8))));

            int minX = liveCells.Min(c => c.X);
            int maxX = liveCells.Max(c => c.X);
            int minY = liveCells.Min(c => c.Y);
            int maxY = liveCells.Max(c => c.Y);

            // More conservative padding calculation
            int patternWidth = maxX - minX + 1;
            int patternHeight = maxY - minY + 1;
            int maxDimension = Math.Max(patternWidth, patternHeight);

            // Padding should be at least the number of generations plus some buffer
            int padding = Math.Max(generations + 4, maxDimension / 4);
            padding = Math.Min(padding, 1000); // Cap padding to prevent excessive memory usage

            minX -= padding;
            minY -= padding;
            int size = Math.Max(patternWidth, patternHeight) + 2 * padding;

            // Find appropriate level
            int level = 0;
            int nodeSize = 1;
            while (nodeSize < size)
            {
                nodeSize *= 2;
                level++;
            }

            level = Math.Max(level, 3);
            level = Math.Min(level, 15); // Cap level to prevent excessive memory usage
            nodeSize = 1 << level;

            return BuildQuadNode(liveCells, minX, minY, nodeSize, level);
        }

        // Rest of the methods remain largely the same but with added error handling
        private QuadNode GetCached(QuadNode node, int generations)
        {
            return _resultCache.TryGetValue((node.NodeId, generations), out var result) ? result : null;
        }

        private void SetCached(QuadNode node, int generations, QuadNode result)
        {
            if (_resultCache.Count < MAX_CACHE_SIZE)
            {
                _resultCache[(node.NodeId, generations)] = result;
            }
        }

        private QuadNode GetCanonical(QuadNode node)
        {
            if (_nodeCache.TryGetValue(node, out var canonical))
                return canonical;

            if (_nodeCache.Count < MAX_CACHE_SIZE)
            {
                _nodeCache[node] = node;
            }
            return node;
        }

        private QuadNode CreateNode(QuadNode nw, QuadNode ne, QuadNode sw, QuadNode se)
        {
            var node = new QuadNode(nw, ne, sw, se);
            return GetCanonical(node);
        }

        private QuadNode CreateEmpty(int level)
        {
            if (level == 0) return _deadNode;
            var empty = CreateEmpty(level - 1);
            return CreateNode(empty, empty, empty, empty);
        }

        private QuadNode ExpandNode(QuadNode node)
        {
            var emptyQuad = CreateEmpty(node.Level);
            return CreateNode(
                CreateNode(emptyQuad, emptyQuad, emptyQuad, node.NW),
                CreateNode(emptyQuad, emptyQuad, node.NE, emptyQuad),
                CreateNode(emptyQuad, node.SW, emptyQuad, emptyQuad),
                CreateNode(node.SE, emptyQuad, emptyQuad, emptyQuad)
            );
        }

        // Simplified AdvanceHashlife with better error handling
        private QuadNode AdvanceHashlife(QuadNode node, int generations)
        {
            if (generations == 0) return node;
            if (node.Level < 2) return ExpandAndAdvance(node, generations);

            var cached = GetCached(node, generations);
            if (cached != null) return cached;

            QuadNode result;
            try
            {
                if (node.Level == 2)
                {
                    result = Advance4x4Block(node, generations);
                }
                else if (generations == 1)
                {
                    result = AdvanceSingleGeneration(node);
                }
                else
                {
                    int maxAdvance = 1 << (node.Level - 2);
                    if (generations >= maxAdvance && node.Level >= 3)
                    {
                        var intermediate = AdvanceMaximal(node);
                        result = generations > maxAdvance ?
                            AdvanceHashlife(intermediate, generations - maxAdvance) :
                            intermediate;
                    }
                    else
                    {
                        // Step by step for smaller advances
                        result = node;
                        int remaining = generations;
                        while (remaining > 0)
                        {
                            int step = Math.Min(remaining, 1);
                            result = AdvanceHashlife(result, step);
                            remaining -= step;
                        }
                    }
                }

                SetCached(node, generations, result);
                return result;
            }
            catch (Exception)
            {
                // Fallback: expand and try again
                return ExpandAndAdvance(node, generations);
            }
        }

        private QuadNode ExpandAndAdvance(QuadNode node, int generations)
        {
            var expanded = ExpandNode(node);
            return AdvanceHashlife(expanded, generations);
        }

        // Keep the rest of your existing methods (AdvanceSingleGeneration, AdvanceMaximal, etc.)
        // but add try-catch blocks for error handling...

        private QuadNode AdvanceSingleGeneration(QuadNode node)
        {
            if (node.Level < 2) throw new ArgumentException("Node must be at least level 2");
            if (node.Level == 2) return Advance4x4Block(node, 1);

            try
            {
                var nw_nw = node.NW;
                var nw_ne = CreateNode(node.NW.NE, node.NE.NW, node.NW.SE, node.NE.SW);
                var ne_ne = node.NE;
                var nw_sw = CreateNode(node.NW.SW, node.NW.SE, node.SW.NW, node.SW.NE);
                var center = CreateNode(node.NW.SE, node.NE.SW, node.SW.NE, node.SE.NW);
                var ne_se = CreateNode(node.NE.SW, node.NE.SE, node.SE.NW, node.SE.NE);
                var sw_sw = node.SW;
                var sw_se = CreateNode(node.SW.NE, node.SE.NW, node.SW.SE, node.SE.SW);
                var se_se = node.SE;

                var r_nw = AdvanceHashlife(nw_nw, 1);
                var r_n = AdvanceHashlife(nw_ne, 1);
                var r_ne = AdvanceHashlife(ne_ne, 1);
                var r_w = AdvanceHashlife(nw_sw, 1);
                var r_c = AdvanceHashlife(center, 1);
                var r_e = AdvanceHashlife(ne_se, 1);
                var r_sw = AdvanceHashlife(sw_sw, 1);
                var r_s = AdvanceHashlife(sw_se, 1);
                var r_se = AdvanceHashlife(se_se, 1);

                return CreateNode(
                    CreateNode(r_nw, r_n, r_w, r_c),
                    CreateNode(r_n, r_ne, r_c, r_e),
                    CreateNode(r_w, r_c, r_sw, r_s),
                    CreateNode(r_c, r_e, r_s, r_se)
                );
            }
            catch (Exception)
            {
                return ExpandAndAdvance(node, 1);
            }
        }

        private QuadNode AdvanceMaximal(QuadNode node)
        {
            if (node.Level < 3) throw new ArgumentException("Node must be at least level 3");

            try
            {
                int halfAdvance = 1 << (node.Level - 3);

                var n00 = AdvanceHashlife(node.NW, halfAdvance);
                var n01 = AdvanceHashlife(CreateNode(node.NW.NE, node.NE.NW, node.NW.SE, node.NE.SW), halfAdvance);
                var n02 = AdvanceHashlife(node.NE, halfAdvance);
                var n10 = AdvanceHashlife(CreateNode(node.NW.SW, node.NW.SE, node.SW.NW, node.SW.NE), halfAdvance);
                var n11 = AdvanceHashlife(CreateNode(node.NW.SE, node.NE.SW, node.SW.NE, node.SE.NW), halfAdvance);
                var n12 = AdvanceHashlife(CreateNode(node.NE.SW, node.NE.SE, node.SE.NW, node.SE.NE), halfAdvance);
                var n20 = AdvanceHashlife(node.SW, halfAdvance);
                var n21 = AdvanceHashlife(CreateNode(node.SW.NE, node.SE.NW, node.SW.SE, node.SE.SW), halfAdvance);
                var n22 = AdvanceHashlife(node.SE, halfAdvance);

                return CreateNode(
                    AdvanceHashlife(CreateNode(n00, n01, n10, n11), halfAdvance),
                    AdvanceHashlife(CreateNode(n01, n02, n11, n12), halfAdvance),
                    AdvanceHashlife(CreateNode(n10, n11, n20, n21), halfAdvance),
                    AdvanceHashlife(CreateNode(n11, n12, n21, n22), halfAdvance)
                );
            }
            catch (Exception)
            {
                return ExpandAndAdvance(node, 1 << (node.Level - 2));
            }
        }

        // Keep your existing methods for Advance4x4Block, BuildQuadNode, ConvertToHashSet, etc.
        // but add appropriate error handling...

        private QuadNode Advance4x4Block(QuadNode node, int generations)
        {
            if (node.Level != 2) throw new ArgumentException("Node must be level 2");

            var grid = new bool[4, 4];
            ExtractGrid(node, grid, 0, 0);

            for (int gen = 0; gen < generations; gen++)
            {
                var newGrid = new bool[4, 4];
                for (int y = 0; y < 4; y++)
                {
                    for (int x = 0; x < 4; x++)
                    {
                        int neighbors = CountNeighbors4x4(grid, x, y);
                        bool isAlive = grid[y, x];
                        newGrid[y, x] = isAlive ? (neighbors == 2 || neighbors == 3) : (neighbors == 3);
                    }
                }
                grid = newGrid;
            }

            return CreateNode(
                CreateNode(
                    grid[0, 0] ? _aliveNode : _deadNode, grid[0, 1] ? _aliveNode : _deadNode,
                    grid[1, 0] ? _aliveNode : _deadNode, grid[1, 1] ? _aliveNode : _deadNode),
                CreateNode(
                    grid[0, 2] ? _aliveNode : _deadNode, grid[0, 3] ? _aliveNode : _deadNode,
                    grid[1, 2] ? _aliveNode : _deadNode, grid[1, 3] ? _aliveNode : _deadNode),
                CreateNode(
                    grid[2, 0] ? _aliveNode : _deadNode, grid[2, 1] ? _aliveNode : _deadNode,
                    grid[3, 0] ? _aliveNode : _deadNode, grid[3, 1] ? _aliveNode : _deadNode),
                CreateNode(
                    grid[2, 2] ? _aliveNode : _deadNode, grid[2, 3] ? _aliveNode : _deadNode,
                    grid[3, 2] ? _aliveNode : _deadNode, grid[3, 3] ? _aliveNode : _deadNode)
            );
        }

        private void ExtractGrid(QuadNode node, bool[,] grid, int x, int y)
        {
            if (node.IsLeaf)
            {
                if (y < grid.GetLength(0) && x < grid.GetLength(1))
                    grid[y, x] = node.IsAlive;
                return;
            }

            int size = 1 << (node.Level - 1);
            ExtractGrid(node.NW, grid, x, y);
            ExtractGrid(node.NE, grid, x + size, y);
            ExtractGrid(node.SW, grid, x, y + size);
            ExtractGrid(node.SE, grid, x + size, y + size);
        }

        private int CountNeighbors4x4(bool[,] grid, int x, int y)
        {
            int count = 0;
            for (int dy = -1; dy <= 1; dy++)
            {
                for (int dx = -1; dx <= 1; dx++)
                {
                    if (dx == 0 && dy == 0) continue;
                    int nx = x + dx, ny = y + dy;
                    if (nx >= 0 && nx < 4 && ny >= 0 && ny < 4 && grid[ny, nx])
                        count++;
                }
            }
            return count;
        }

        private QuadNode BuildQuadNode(HashSet<(int X, int Y)> liveCells, int x, int y, int size, int level)
        {
            if (level == 0)
                return liveCells.Contains((x, y)) ? _aliveNode : _deadNode;

            int halfSize = size / 2;
            var nw = BuildQuadNode(liveCells, x, y, halfSize, level - 1);
            var ne = BuildQuadNode(liveCells, x + halfSize, y, halfSize, level - 1);
            var sw = BuildQuadNode(liveCells, x, y + halfSize, halfSize, level - 1);
            var se = BuildQuadNode(liveCells, x + halfSize, y + halfSize, halfSize, level - 1);

            return CreateNode(nw, ne, sw, se);
        }

        private HashSet<(int X, int Y)> ConvertToHashSet(QuadNode node)
        {
            var result = new HashSet<(int X, int Y)>();
            if (node != null)
            {
                ConvertToHashSetRecursive(node, result, 0, 0, 1 << node.Level);
            }
            return result;
        }

        private void ConvertToHashSetRecursive(QuadNode node, HashSet<(int X, int Y)> result, int x, int y, int size)
        {
            if (node.IsLeaf)
            {
                if (node.IsAlive)
                    result.Add((x, y));
                return;
            }

            int halfSize = size / 2;
            ConvertToHashSetRecursive(node.NW, result, x, y, halfSize);
            ConvertToHashSetRecursive(node.NE, result, x + halfSize, y, halfSize);
            ConvertToHashSetRecursive(node.SW, result, x, y + halfSize, halfSize);
            ConvertToHashSetRecursive(node.SE, result, x + halfSize, y + halfSize, halfSize);
        }

        private string ComputeHashSetHash(HashSet<(int X, int Y)> liveCells)
        {
            var sortedCells = liveCells.OrderBy(c => c.X).ThenBy(c => c.Y).ToList();
            return string.Join(";", sortedCells.Select(c => $"{c.X},{c.Y}"));
        }
    }
}