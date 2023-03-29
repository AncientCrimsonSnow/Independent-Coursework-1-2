using Extensions.Ml_Agents;
using Extensions.Unity;
using UnityEngine;

namespace Models.DefaultSensor.ToGoalSimple
{
    public struct ToGoalSimpleObservation : IObservation<ToGoalSimpleObservation>
    {
        private readonly Vector2 _relativePosition;

        public ToGoalSimpleObservation(ToGoalSimpleAgent agent)
        {
            _relativePosition = agent.TargetPosition.GetRelativePositionTo(agent.Position);
        }
    }
}