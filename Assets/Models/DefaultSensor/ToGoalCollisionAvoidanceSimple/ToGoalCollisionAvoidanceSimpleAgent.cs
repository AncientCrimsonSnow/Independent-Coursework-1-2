using Extensions.Ml_Agents;
using Extensions.Ml_Agents.VectorSensor;
using Extensions.Unity;
using Models.DefaultSensor.ToGoalSimple;
using Unity.MLAgents.Actuators;
using UnityEngine;

namespace Models.DefaultSensor.ToGoalCollisionAvoidanceSimple
{
    public class ToGoalCollisionAvoidanceSimpleAgent : VectorSensorAgent<
        ToGoalCollisionAvoidanceSimpleAgent,
        ToGoalCollisionAvoidanceSimpleObservation,
        ToGoalCollisionAvoidanceSimpleAction,
        ToGoalSimpleActionData>
    {
        private const int LifeTime = 30;

        [SerializeField]
        private Rigidbody2D _rigidbody;

        private ToGoalCollisionAvoidanceSimpleHelper _helper;
        private float _lifeTimeDeadLine;
        private float _movementRewardTotal;
        private int _updateCount;
        
        //public float dataCountdown = 300;
        
        //private static float TotalCollisionsWithAgents = 0;

        public Vector2 Position => _rigidbody.position;

        private void OnCollisionEnter2D(Collision2D col)
        {
            if (!InEpisode) return;

            if (col.gameObject.GetComponent<ToGoalCollisionAvoidanceSimpleAgent>().InEpisode)
            {
                if (_updateCount != 0)
                {
                    var timeLefT = _lifeTimeDeadLine - Time.time;
                    var timeModifier = timeLefT / LifeTime;
                    var reward = -1 - timeModifier;
                    AddReward(reward);
                    ProcessMovementReward();
                    //TotalCollisionsWithAgents += 0.5f;
                }

                MyEndEpisode();
                _helper.SetToFreePos(transform);
                StartLearning();
                ResetLifeTime();
            }
            /*
            if(dataCountdown > 0)
                Debug.Log(TotalCollisionsWithAgents);
            */
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!InEpisode) return;

            if (other.GetComponent<Target>())
            {
                AddReward(5);
                ProcessMovementReward();
                MyEndEpisode();
                _helper.ReloadTargetPos();
                StartLearning();
                _helper.ResetAllLifeTimes();
            }
            else if (other.GetComponent<Wall>())
            {
                ProcessBadCollisionReward();
            }
        }

        protected override void Setup()
        {
            _helper = ToGoalCollisionAvoidanceSimpleHelper.Instance;
        }

        public void MovePosition(Vector2 newPosition)
        {
            _rigidbody.MovePosition(newPosition);
        }

        public void ResetLifeTime()
        {
            _lifeTimeDeadLine = Time.time + LifeTime;
        }

        protected override ToGoalCollisionAvoidanceSimpleObservation GetObservation()
        {
            return new ToGoalCollisionAvoidanceSimpleObservation(_helper.GetCloseAgents(this), transform.position,
                _helper.CrrTargetPos);
        }

        protected override ToGoalCollisionAvoidanceSimpleAction GetAction(ActionBuffers actions)
        {
            return new ToGoalCollisionAvoidanceSimpleAction(actions, this);
        }

        protected override ToGoalSimpleActionData GetHeuristicData()
        {
            var directPath = _helper.CrrTargetPos.GetRelativePositionTo(Position);
            var negativeDirectPath = directPath.Rotate(180);
            var half1 = directPath.Rotate(90);
            var half2 = directPath.Rotate(-90);
            var temp = directPath.Rotate(135);
            return new ToGoalSimpleActionData(directPath);
        }

        protected override void Heuristic(ToGoalSimpleActionData data, in ActionSegment<float> continuesActions,
            in ActionSegment<int> discreteActions)
        {
            continuesActions[0] = data.move.x;
            continuesActions[1] = data.move.y;
        }

        protected override void OnAfterApplyAction(ToGoalCollisionAvoidanceSimpleAction action)
        {
            /*
            dataCountdown -= Time.fixedDeltaTime;
            if(dataCountdown <= 0)
                Debug.Log(TotalCollisionsWithAgents+ "FINAL");
            */
            
            _updateCount++;
            var reward = action.DistanceDelta / Time.fixedDeltaTime;
            _movementRewardTotal += reward;

            if (Time.time > _lifeTimeDeadLine)
                ProcessBadCollisionReward();
        }

        private void ProcessMovementReward()
        {
            const int movementRewardModifier = 3;
            if (_updateCount == 0) return;

            var movementReward = _movementRewardTotal / _updateCount;
            AddReward(movementReward * movementRewardModifier);
            _updateCount = 0;
            _movementRewardTotal = 0;
        }

        private void ProcessBadCollisionReward()
        {
            if (_updateCount != 0)
            {
                AddReward(-1);
                ProcessMovementReward();
            }

            MyEndEpisode();
            _helper.SetToFreePos(transform);
            StartLearning();
            ResetLifeTime();
        }
    }
}