using System.Collections.Generic;
using Extensions.Ml_Agents;
using Extensions.Unity;
using UnityEngine;

namespace Models.DefaultSensor.ToGoalCollisionAvoidanceSimple
{
    public struct ToGoalCollisionAvoidanceSimpleObservation : IObservation<ToGoalCollisionAvoidanceSimpleObservation>
    {
        private readonly Vector2[] _closeAgentsPosNegativRelative;
        private readonly Vector2 _pos;
        private readonly Vector2 _relativeTargetPos;

        public ToGoalCollisionAvoidanceSimpleObservation(IReadOnlyList<Vector2> closeAgentsPos, Vector2 pos, Vector2 targetPos)
        {
            _closeAgentsPosNegativRelative = new Vector2[closeAgentsPos.Count];
            for (var i = 0; i != _closeAgentsPosNegativRelative.Length; i++)
                _closeAgentsPosNegativRelative[i] = pos.GetRelativePositionTo(closeAgentsPos[i]);
            _pos = pos;
            _relativeTargetPos = targetPos.GetRelativePositionTo(pos);
        }
    }
}