using System.Collections.Generic;
using Extensions.Ml_Agents.VectorSensor;
using Unity.MLAgents.Actuators;
using UnityEngine;

namespace Models.RaycastSensor.MazeSolver
{
    public class MazeSolverAgent : VectorSensorAgent<
        MazeSolverAgent,
        EmptyVectorObservation,
        MazeSolverAgentAction,
        MazeSolverActionData>
    {
        [SerializeField]
        private Rigidbody2D _rigidbody;
        [SerializeField]
        private MazeGenerator _generator;
        
        private readonly List<MazeNode> _seenNodes = new ();
        private MazeNode _endNode;
        
        public Vector2 Position => _rigidbody.position;
        
        private const int LifeTime = 5;
        
        private float _lifeTimeDeadLine;

        protected override void Setup()
        {
            ResetLifeTime();
            LoadLevel();
        }

        private void LoadLevel()
        {
            _generator.GenerateNewMaze();
            var startNode =  _generator.GetRandomNode();
            transform.position =startNode.transform.position;
            _endNode = _generator.GetRandomNode();
            _seenNodes.Clear();
            startNode.ColorFloor(Color.green);
            _endNode.ColorFloor(Color.red);
        }
        
        private void ResetLifeTime()
        {
            _lifeTimeDeadLine = Time.time + LifeTime;
        }

        public void MovePosition(Vector2 newPosition)
        {
            _rigidbody.MovePosition(newPosition);
        }
        protected override EmptyVectorObservation GetObservation()
        {
            return new EmptyVectorObservation();
        }

        protected override MazeSolverAgentAction GetAction(ActionBuffers actions)
        {
            return new MazeSolverAgentAction(actions, this);
        }

        protected override MazeSolverActionData GetHeuristicData()
        {
            return new MazeSolverActionData(new Vector2(
                Input.GetAxisRaw("Horizontal"),
                Input.GetAxisRaw("Vertical")
                ));
        }

        protected override void Heuristic(MazeSolverActionData data, in ActionSegment<float> continuesActions, in ActionSegment<int> discreteActions)
        {
            continuesActions[0] = data.move.x;
            continuesActions[1] = data.move.y;
        }

        private void OverTime()
        {
            Debug.Log("OVERTIME");
            SetReward(-1);
            MyEndEpisode();
            ResetLifeTime();
            StartLearning();
        }
        
        protected override void OnAfterApplyAction(MazeSolverAgentAction vectorAgentAction)
        {
            if (Time.time > _lifeTimeDeadLine)
            {
                OverTime();
            }
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if(!InEpisode) return;
            
            ResetLifeTime();
            var mazeNode = col.transform.parent.GetComponent<MazeNode>();
            if (!_seenNodes.Contains(mazeNode))
            {
                Debug.Log("NEW TILE");
                _seenNodes.Add(mazeNode);
                SetReward(_lifeTimeDeadLine - Time.time);
                MyEndEpisode();
                if(mazeNode == _endNode){
                    _generator.GenerateNewMaze();
                    var startNode =  _generator.GetRandomNode();
                    transform.position =startNode.transform.position;
                    _endNode = _generator.GetRandomNode();
                    _seenNodes.Clear();
                    startNode.ColorFloor(Color.green);
                    _endNode.ColorFloor(Color.red);
                }
                StartLearning();
            }
            else
            {
                Debug.Log("OlD TILE");
            }
            
        }
    }
}