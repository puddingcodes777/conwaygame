namespace ConwayGameOfLife.Core.Helper
{
    // Represents a quadtree node for the Hashlife algorithm
    public class QuadNode
    {
        public int Level { get; }
        public QuadNode NW { get; }  // Northwest
        public QuadNode NE { get; }  // Northeast
        public QuadNode SW { get; }  // Southwest
        public QuadNode SE { get; }  // Southeast

        // For leaf nodes (level 0) - represents a single cell
        public bool IsAlive { get; }

        // Unique ID for this node (used for hashing and equality)
        public int NodeId { get; }
        private static int _nextNodeId = 0;

        // Constructor for leaf nodes
        public QuadNode(bool isAlive)
        {
            Level = 0;
            IsAlive = isAlive;
            NodeId = System.Threading.Interlocked.Increment(ref _nextNodeId);
        }

        // Constructor for internal nodes
        public QuadNode(QuadNode nw, QuadNode ne, QuadNode sw, QuadNode se)
        {
            if (nw.Level != ne.Level || ne.Level != sw.Level || sw.Level != se.Level)
                throw new ArgumentException("All quadrants must have the same level");

            Level = nw.Level + 1;
            NW = nw;
            NE = ne;
            SW = sw;
            SE = se;
            NodeId = System.Threading.Interlocked.Increment(ref _nextNodeId);
        }

        public bool IsLeaf => Level == 0;

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            if (obj is not QuadNode other) return false;
            if (Level != other.Level) return false;

            if (IsLeaf)
                return IsAlive == other.IsAlive;

            return ReferenceEquals(NW, other.NW) && ReferenceEquals(NE, other.NE) &&
                   ReferenceEquals(SW, other.SW) && ReferenceEquals(SE, other.SE);
        }

        public override int GetHashCode()
        {
            if (IsLeaf)
                return HashCode.Combine(Level, IsAlive);

            return HashCode.Combine(Level, NW?.NodeId ?? 0, NE?.NodeId ?? 0, SW?.NodeId ?? 0, SE?.NodeId ?? 0);
        }
    }
}
