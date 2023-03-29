using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Models.RaycastSensor.MazeSolver
{
    public class MazeGenerator : MonoBehaviour
    {
        [SerializeField]
        private MazeNode _nodePrefab;
        [SerializeField] 
        private Vector2Int _mazeSize;

        private  List<MazeNode> _nodes = new ();
        
        public MazeNode GetRandomNode()
        {
            return _nodes[Random.Range(0, _nodes.Count)];
        }
        
        public void GenerateNewMaze()
        {
            _nodes.Clear();
            foreach (Transform child in transform) {
                Destroy(child.gameObject);
            }
            GenerateMaze();
        }

        private void GenerateMaze()
        {
            // Create _nodes
            var nodePosOffset = new Vector2(-_mazeSize.x / 2f , -_mazeSize.y / 2f);
            
            for (var x = 0; x < _mazeSize.x; x++)
            {
                for (var y = 0; y < _mazeSize.y; y++)
                {
                    var nodePos = new Vector2(x, y) + nodePosOffset;
                    var newNode = Instantiate(_nodePrefab, nodePos, Quaternion.identity, transform);
                    _nodes.Add(newNode);
                }
            }

            var currentPath = new List<MazeNode>();
            var completedNodes = new List<MazeNode>();

            // Choose starting node
            currentPath.Add(_nodes[Random.Range(0, _nodes.Count)]);
            
            while (completedNodes.Count < _nodes.Count)
            {
                // Check _nodes next to the current node
                var possibleNextNodes = new List<int>();
                var possibleDirections = new List<int>();

                var currentNodeIndex = _nodes.IndexOf(currentPath[^1]);
                var currentNodeX = currentNodeIndex / _mazeSize.y;
                var currentNodeY = currentNodeIndex % _mazeSize.y;

                if (currentNodeX < _mazeSize.x - 1)
                {
                    // Check node to the right of the current node
                    if (!completedNodes.Contains(_nodes[currentNodeIndex + _mazeSize.y]) &&
                        !currentPath.Contains(_nodes[currentNodeIndex + _mazeSize.y]))
                    {
                        possibleDirections.Add(1);
                        possibleNextNodes.Add(currentNodeIndex + _mazeSize.y);
                    }
                }

                if (currentNodeX > 0)
                {
                    // Check node to the left of the current node
                    if (!completedNodes.Contains(_nodes[currentNodeIndex - _mazeSize.y]) &&
                        !currentPath.Contains(_nodes[currentNodeIndex - _mazeSize.y]))
                    {
                        possibleDirections.Add(2);
                        possibleNextNodes.Add(currentNodeIndex - _mazeSize.y);
                    }
                }

                if (currentNodeY < _mazeSize.y - 1)
                {
                    // Check node above the current node
                    if (!completedNodes.Contains(_nodes[currentNodeIndex + 1]) &&
                        !currentPath.Contains(_nodes[currentNodeIndex + 1]))
                    {
                        possibleDirections.Add(3);
                        possibleNextNodes.Add(currentNodeIndex + 1);
                    }
                }

                if (currentNodeY > 0)
                {
                    // Check node below the current node
                    if (!completedNodes.Contains(_nodes[currentNodeIndex - 1]) &&
                        !currentPath.Contains(_nodes[currentNodeIndex - 1]))
                    {
                        possibleDirections.Add(4);
                        possibleNextNodes.Add(currentNodeIndex - 1);
                    }
                }

                // Choose next node
                if (possibleDirections.Count > 0)
                {
                    var chosenDirection = Random.Range(0, possibleDirections.Count);
                    var chosenNode = _nodes[possibleNextNodes[chosenDirection]];

                    switch (possibleDirections[chosenDirection])
                    {
                        case 1:
                            chosenNode.RemoveWall(1);
                            currentPath[^1].RemoveWall(0);
                            break;
                        case 2:
                            chosenNode.RemoveWall(0);
                            currentPath[^1].RemoveWall(1);
                            break;
                        case 3:
                            chosenNode.RemoveWall(3);
                            currentPath[^1].RemoveWall(2);
                            break;
                        case 4:
                            chosenNode.RemoveWall(2);
                            currentPath[^1].RemoveWall(3);
                            break;
                    }

                    currentPath.Add(chosenNode);
                }
                else
                {
                    completedNodes.Add(currentPath[^1]);
                    currentPath.RemoveAt(currentPath.Count - 1);
                }
            }
        }
    }
}
