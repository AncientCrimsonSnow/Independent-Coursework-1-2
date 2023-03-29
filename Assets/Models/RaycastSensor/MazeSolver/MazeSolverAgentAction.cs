using System;
using Extensions.Ml_Agents.VectorSensor;
using Unity.MLAgents.Actuators;
using UnityEngine;

namespace Models.RaycastSensor.MazeSolver
{
    public class MazeSolverAgentAction : Extensions.Ml_Agents.Action<
        MazeSolverAgentAction,
        MazeSolverActionData,
        EmptyVectorObservation,
        MazeSolverAgent>
    {

        public MazeSolverAgentAction(ActionBuffers actions, MazeSolverAgent agent) : base(actions, agent)
        {
        }

        public MazeSolverAgentAction()
        {
        }
        
        public override int GetNumberOfContinuesActions()
        {
            return 2;
        }

        public override int[] GetBranchSizes(MazeSolverAgent agent)
        {
            return Array.Empty<int>();
        }

        public override void ApplyAction(MazeSolverAgent vectorSensorAgent)
        {
            var newAgentsPosition = vectorSensorAgent.Position + data.move * Time.fixedDeltaTime;
            vectorSensorAgent.MovePosition(newAgentsPosition);
        }

        protected override MazeSolverActionData GetData(float[] continuesActions, int[] discreteActions, MazeSolverAgent agent)
        {
            var move = new Vector2(
                continuesActions[0],
                continuesActions[1]
            ).normalized;
            return new MazeSolverActionData(move);
        }
    }
}