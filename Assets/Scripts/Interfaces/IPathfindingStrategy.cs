using System.Collections.Generic;

public interface IPathfindingStrategy
{
    List<PathNode> FindPath(PathNode startNode, PathNode goalNode);
}