using UnityEngine;

namespace Models.RaycastSensor.MazeSolver
{
    public class MazeNode : MonoBehaviour
    {
        [SerializeField]
        private Transform[] _walls;
        [SerializeField]
        private MeshRenderer _floor;
        
        public void RemoveWall(int wallToRemove)
        {
            _walls[wallToRemove].gameObject.SetActive(false);
        }

        public void ColorFloor(Color color)
        {
            _floor.material.color = color;
        }
    }
}
