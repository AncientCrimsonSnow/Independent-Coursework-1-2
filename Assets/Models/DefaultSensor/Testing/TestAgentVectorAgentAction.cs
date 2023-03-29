using Extensions.Ml_Agents;
using Unity.MLAgents.Actuators;

namespace Models.DefaultSensor.Testing
{
    public class TestAgentVectorAgentAction : Action<
        TestAgentVectorAgentAction,
        TestAgentActionData,
        TestAgentObservation,
        TestVectorSensorAgent>
    {
        public TestAgentVectorAgentAction()
        {
        }

        public TestAgentVectorAgentAction(ActionBuffers actions, TestVectorSensorAgent agent) : base(actions, agent)
        {
        }

        public int Value => data.value;

        public override int GetNumberOfContinuesActions()
        {
            return 0;
        }

        public override int[] GetBranchSizes(TestVectorSensorAgent agent)
        {
            return new[] { 1000 };
        }

        public override void ApplyAction(TestVectorSensorAgent vectorSensorAgent)
        {
        }

        protected override TestAgentActionData GetData(float[] continuesActions, int[] discreteActions, TestVectorSensorAgent agent)
        {
            return new TestAgentActionData
            {
                value = discreteActions[0]
            };
        }
    }
}