using System;
using Unity.MLAgents.Actuators;
using UnityEngine;

namespace Models.DefaultSensor.ToGoalSimple
{
    public class ToGoalSimpleAgentAction : Extensions.Ml_Agents.Action<
        ToGoalSimpleAgentAction,
        ToGoalSimpleActionData,
        ToGoalSimpleObservation,
        ToGoalSimpleAgent>
    {
        public ToGoalSimpleAgentAction(ActionBuffers actions, ToGoalSimpleAgent agent) : base(actions, agent)
        {
        }

        public ToGoalSimpleAgentAction()
        {
            
        }
        public override int[] GetBranchSizes(ToGoalSimpleAgent agent)
        {
            return Array.Empty<int>();
        }

        public float DistanceDelta { private set; get; }

        public override int GetNumberOfContinuesActions()
        {
            return 2;
        }

        
        public override void ApplyAction(ToGoalSimpleAgent agent)
        {
            var oldDistance = Vector2.Distance(agent.Position, agent.TargetPosition);

            var newAgentsPosition = agent.Position + data.move * Time.fixedDeltaTime;
            agent.MovePosition(newAgentsPosition);
            var newDistance = Vector2.Distance(newAgentsPosition, agent.TargetPosition);
            DistanceDelta = oldDistance - newDistance;
        }

        protected override ToGoalSimpleActionData GetData(float[] continuesActions, int[] discreteActions, ToGoalSimpleAgent agent)
        {
            var move = new Vector2(
                continuesActions[0],
                continuesActions[1]
            ).normalized;
            return new ToGoalSimpleActionData(move);
        }
    }
}