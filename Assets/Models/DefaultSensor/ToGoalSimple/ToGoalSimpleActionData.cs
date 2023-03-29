using Extensions.Ml_Agents;
using UnityEngine;

namespace Models.DefaultSensor.ToGoalSimple
{
    public struct ToGoalSimpleActionData : IActionData<ToGoalSimpleActionData>
    {
        public readonly Vector2 move;

        public ToGoalSimpleActionData(Vector2 move)
        {
            this.move = move;
        }
    }
}