using Extensions.Ml_Agents;

namespace Models.DefaultSensor.Testing
{
    public struct TestAgentObservation : IObservation<TestAgentObservation>
    {
        private readonly int _value;

        public TestAgentObservation(int value)
        {
            _value = value;
        }
    }
}