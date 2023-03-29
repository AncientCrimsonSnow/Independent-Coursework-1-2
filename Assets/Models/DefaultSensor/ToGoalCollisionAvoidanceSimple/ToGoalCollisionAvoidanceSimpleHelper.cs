using System.Collections.Generic;
using Extensions.Ml_Agents;
using Extensions.Unity;
using Models.DefaultSensor.ToGoalSimple;
using UnityEngine;

namespace Models.DefaultSensor.ToGoalCollisionAvoidanceSimple
{
    public class ToGoalCollisionAvoidanceSimpleHelper : SingletonMono<ToGoalCollisionAvoidanceSimpleHelper>
    {
        [SerializeField]
        private int _numberOfAgents;

        [SerializeField]
        private int _numberOfAgentsEachAgentSee;

        [SerializeField]
        private Target _crrTarget;

        [SerializeField]
        private ToGoalCollisionAvoidanceSimpleAgent _agentPrefab;

        [SerializeField]
        private Arena2 _arena2;


        private ToGoalCollisionAvoidanceSimpleAgent[] _agents;
        private Dictionary<ToGoalCollisionAvoidanceSimpleAgent, Vector2[]> _closeAgentsMapData;
        public Vector2 CrrTargetPos => _crrTarget.transform.position;

        private void Awake()
        {
            _agents = new ToGoalCollisionAvoidanceSimpleAgent[_numberOfAgents];
            for (var i = 0; i < _numberOfAgents; i++)
            {
                _agents[i] = Instantiate(_agentPrefab, transform);
                SetToFreePos(_agents[i].transform);
            }

            ReloadTargetPos();

            foreach (var agent in _agents) agent.StartLearning();
        }

        private void FixedUpdate()
        {
            _closeAgentsMapData = FindAllCloseAgents();
        }

        public void ReloadTargetPos()
        {
            foreach (var agent in _agents) agent.MyEndEpisode();
            SetToFreePos(_crrTarget.transform);
        }

        public void SetToFreePos(Transform value)
        {
            int colliderCount;
            do
            {
                var pos = Utilities.GetRandomPosInRect(_arena2.Center, _arena2.Size);
                value.position = pos;
                var circleCollider2D = value.GetComponent<CircleCollider2D>();
                var result = new Collider2D[1];
                colliderCount = Physics2D.OverlapCircleNonAlloc(pos, circleCollider2D.radius + 0.1f, result);
            } while (colliderCount != 0);
        }

        public void ResetAllLifeTimes()
        {
            foreach (var agent in _agents) agent.ResetLifeTime();
        }

        public Vector2[] GetCloseAgents(ToGoalCollisionAvoidanceSimpleAgent agent)
        {
            if (_agents[^1] == null)
                return new Vector2[_numberOfAgentsEachAgentSee];

            _closeAgentsMapData ??= FindAllCloseAgents();

            return _closeAgentsMapData[agent];
        }

        private Dictionary<ToGoalCollisionAvoidanceSimpleAgent, Vector2[]> FindAllCloseAgents()
        {
            var result = new Dictionary<ToGoalCollisionAvoidanceSimpleAgent, Vector2[]>();
            var positions = new Vector2[_agents.Length];

            for (var i = 0; i != _agents.Length; i++) positions[i] = _agents[i].transform.position;
            var indices = Utilities.MyGetClosePositionIndices(positions, _numberOfAgentsEachAgentSee);

            for (var i = 0; i != _agents.Length; i++)
            {
                var positionsAgentCanSee = new Vector2[_numberOfAgentsEachAgentSee];
                for (var j = 0; j != _numberOfAgentsEachAgentSee; j++)
                    positionsAgentCanSee[j] = _agents[indices[i, j]].transform.position;
                result.Add(_agents[i], positionsAgentCanSee);
            }

            return result;
        }
    }
}