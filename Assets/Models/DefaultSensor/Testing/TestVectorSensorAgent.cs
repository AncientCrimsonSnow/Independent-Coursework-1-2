using Extensions.Ml_Agents;
using Extensions.Ml_Agents.VectorSensor;
using Unity.MLAgents.Actuators;
using UnityEngine;

namespace Models.DefaultSensor.Testing
{
    public class TestVectorSensorAgent : VectorSensorAgent<
        TestVectorSensorAgent,
        TestAgentObservation,
        TestAgentVectorAgentAction,
        TestAgentActionData>
    {
        [SerializeField]
        private int maxValue;

        private int _value;

        protected override void OnEnable()
        {
            base.OnEnable();
            StartLearning();
        }

        protected override TestAgentObservation GetObservation()
        {
            _value = Random.Range(0, maxValue);
            return new TestAgentObservation(_value);
        }

        protected override TestAgentVectorAgentAction GetAction(ActionBuffers actions)
        {
            return new TestAgentVectorAgentAction(actions, this);
        }

        protected override TestAgentActionData GetHeuristicData()
        {
            return new TestAgentActionData
            {
                value = _value
            };
        }

        protected override void Heuristic(TestAgentActionData data, in ActionSegment<float> continuesActions,
            in ActionSegment<int> discreteActions)
        {
            continuesActions[0] = data.value;
        }

        protected override void OnAfterApplyAction(TestAgentVectorAgentAction vectorAgentAction)
        {
            var reward = -Mathf.Abs(vectorAgentAction.Value - _value);
            SetReward(reward);
            MyEndEpisode();
        }
    }
}