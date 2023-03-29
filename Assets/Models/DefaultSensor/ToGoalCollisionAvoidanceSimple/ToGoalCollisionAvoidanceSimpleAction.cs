using System;
using Models.DefaultSensor.ToGoalSimple;
using Unity.MLAgents.Actuators;
using UnityEngine;

namespace Models.DefaultSensor.ToGoalCollisionAvoidanceSimple
{
    public class ToGoalCollisionAvoidanceSimpleAction : Extensions.Ml_Agents.Action<
        ToGoalCollisionAvoidanceSimpleAction,
        ToGoalSimpleActionData,
        ToGoalCollisionAvoidanceSimpleObservation,
        ToGoalCollisionAvoidanceSimpleAgent>
    {
        public ToGoalCollisionAvoidanceSimpleAction(ActionBuffers actions, ToGoalCollisionAvoidanceSimpleAgent agent) : base(actions, agent)
        {
        }

        public ToGoalCollisionAvoidanceSimpleAction()
        {
        }

        public float DistanceDelta { private set; get; }

        public override int GetNumberOfContinuesActions()
        {
            return 2;
        }

        public override int[] GetBranchSizes(ToGoalCollisionAvoidanceSimpleAgent agent)
        {
            return Array.Empty<int>();
        }

        public override void ApplyAction(ToGoalCollisionAvoidanceSimpleAgent agent)
        {
            var targetPos = ToGoalCollisionAvoidanceSimpleHelper.Instance.CrrTargetPos;
            var oldDistance = Vector2.Distance(agent.Position, targetPos);
            var newAgentsPosition = agent.Position + data.move * Time.fixedDeltaTime;
            agent.MovePosition(newAgentsPosition);
            var newDistance = Vector2.Distance(newAgentsPosition, targetPos);
            DistanceDelta = oldDistance - newDistance;
        }

        protected override ToGoalSimpleActionData GetData(float[] continuesActions, int[] discreteActions, ToGoalCollisionAvoidanceSimpleAgent agent)
        {
            if (continuesActions.Length < GetNumberOfContinuesActions())
                return new ToGoalSimpleActionData(Vector2.zero);

            var move = new Vector2(
                continuesActions[0],
                continuesActions[1]
            ).normalized;
            return new ToGoalSimpleActionData(move);
        }
    }
}