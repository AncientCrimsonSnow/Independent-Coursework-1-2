using System.Collections.Generic;
using UnityEngine;

public class KDTree
{
    private class Node
    {
        public Vector2 Point { get; set; }
        public Node Left { get; set; }
        public Node Right { get; set; }
    }

    private Node root;
    
    public KDTree(Vector2[] points)
    {
        root = BuildRecursive(points, 0, points.Length - 1, 0);
    }
    
    private Node BuildRecursive(Vector2[] points, int start, int end, int depth)
    {
        if (start > end) return null;

        var mid = (start + end) / 2;
        var node = new Node { Point = points[mid] };

        var nextDepth = (depth + 1) % 2;
        node.Left = BuildRecursive(points, start, mid - 1, nextDepth);
        node.Right = BuildRecursive(points, mid + 1, end, nextDepth);

        return node;
    }

    public List<Vector2> FindNearestNeighbors(Vector2 point, int count)
    {
        var neighbors = new List<Vector2>();
        FindNearestNeighborsRecursive(root, point, count, neighbors);
        return neighbors;
    }

    private void FindNearestNeighborsRecursive(Node node, Vector2 point, int count, List<Vector2> neighbors)
    {
        if (node == null) return;
        
        var dist = node.Point.DistanceSquared(point);

        if (neighbors.Count < count)
        {
            neighbors.Add(node.Point);
        }
        
        else if (dist < neighbors[0].DistanceSquared(point))
        {
            neighbors[0] = node.Point;
        }

        var nextDepth = (node.Left != null && point.x < node.Point.x || node.Left == null && point.y < node.Point.y) ? 0 : 1;

        FindNearestNeighborsRecursive(nextDepth == 0 ? node.Left : node.Right, point, count, neighbors);

        if (neighbors.Count < count || dist < neighbors[0].DistanceSquared(point))
        {
            FindNearestNeighborsRecursive(nextDepth == 0 ? node.Right : node.Left, point, count, neighbors);
        }
    }
}