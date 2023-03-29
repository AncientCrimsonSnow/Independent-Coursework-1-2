using Extensions.Ml_Agents;
using Extensions.Ml_Agents.VectorSensor;
using Extensions.Unity;
using Unity.MLAgents.Actuators;
using UnityEngine;

namespace Models.DefaultSensor.ToGoalSimple
{
    public class ToGoalSimpleAgent : VectorSensorAgent<
            ToGoalSimpleAgent,
            ToGoalSimpleObservation,
            ToGoalSimpleAgentAction,
            ToGoalSimpleActionData>
    {
        private const float SafeRadius = 2f;
        private const float SafeRadiusSquared = SafeRadius * SafeRadius;

        [SerializeField]
        private Transform target;

        [SerializeField]
        private Rigidbody2D _rigidbody;

        [SerializeField]
        private Arena2 _arena2;

        private float _startTime;
        public Vector2 Position => _rigidbody.position;
        public Vector2 TargetPosition => target.position;

        protected override void OnEnable()
        {
            ResetPositions();
            base.OnEnable();
            StartLearning();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            ResetPositions();
        }


        public void MovePosition(Vector2 newPosition)
        {
            _rigidbody.MovePosition(newPosition);
        }

        protected override ToGoalSimpleObservation GetObservation()
        {
            return new ToGoalSimpleObservation(this);
        }

        protected override ToGoalSimpleAgentAction GetAction(ActionBuffers actions)
        {
            return new ToGoalSimpleAgentAction(actions, this);
        }

        protected override ToGoalSimpleActionData GetHeuristicData()
        {
            return new ToGoalSimpleActionData(TargetPosition.GetRelativePositionTo(Position));
        }

        protected override void Heuristic(ToGoalSimpleActionData data, in ActionSegment<float> continuesActions,
            in ActionSegment<int> discreteActions)
        {
            continuesActions[0] = data.move.x;
            continuesActions[1] = data.move.y;
        }

        protected override void OnAfterApplyAction(ToGoalSimpleAgentAction agentAction)
        {
            var reward = agentAction.DistanceDelta / Time.fixedDeltaTime;
            SetReward(reward);
            MyEndEpisode();
            if (!HasModel)
                ResetPositions();
        }

        private void ResetPositions()
        {
            GetSpawnsPos(out var agentPos, out var targetPos);
            transform.position = agentPos;
            target.position = targetPos;
        }


        private void GetSpawnsPos(out Vector2 agentPos, out Vector2 targetPos)
        {
            agentPos = Utilities.GetRandomPosInRect(_arena2.Center, _arena2.Size);
            do
            {
                targetPos = Utilities.GetRandomPosInRect(_arena2.Center, _arena2.Size);
            } while (agentPos.DistanceSquared(targetPos) < SafeRadiusSquared);
        }
    }
}