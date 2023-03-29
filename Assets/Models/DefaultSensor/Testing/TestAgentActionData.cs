using Extensions.Ml_Agents;

namespace Models.DefaultSensor.Testing
{
    public struct TestAgentActionData : IActionData<TestAgentActionData>
    {
        public int value;
    }
}