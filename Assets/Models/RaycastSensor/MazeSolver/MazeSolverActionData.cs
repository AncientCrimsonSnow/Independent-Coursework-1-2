using Extensions.Ml_Agents;
using UnityEngine;

namespace Models.RaycastSensor.MazeSolver
{
    public struct MazeSolverActionData : IActionData<MazeSolverActionData>
    {
        public readonly Vector2 move;

        public MazeSolverActionData(Vector2 move)
        {
            this.move = move;
        }
    }
}