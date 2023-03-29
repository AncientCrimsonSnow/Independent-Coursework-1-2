using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;
using UnityEngine;

namespace Extensions.Ml_Agents
{
    [RequireComponent(typeof(BehaviorParameters))]
    public abstract class Agent<T, O, A, D> : Agent
        where T : Agent<T, O, A, D>
        where O : struct, IObservation<O>
        where A : Action<A, D, O, T>, new()
        where D : struct, IActionData<D>
    {
        protected bool InEpisode { get; private set; }
        protected bool HasModel => _behaviorParameters.Model is not null;
        
        [SerializeField]
        private bool _tickOnClick;
        
        private BehaviorParameters _behaviorParameters;

        protected virtual void OnAfterApplyAction(A action){}
        protected virtual void OnBeforeCollectObservation(){}
        
        
        protected abstract O GetObservation();
        protected abstract A GetAction(ActionBuffers actions);
        protected abstract D GetHeuristicData();
        protected abstract void Heuristic(D data, in ActionSegment<float> continuesActions, in ActionSegment<int> discreteActions);
        
        protected virtual void Setup(){}
        protected internal virtual void SetupInternal(){}

        private void Awake()
        {
            Setup();
            SetupInternal();
            
            if(!Application.isEditor)
                _tickOnClick = false;
            _behaviorParameters = GetComponent<BehaviorParameters>();
            _behaviorParameters.SetObservationParameter<T, O, A, D>(GetObservation(), new A(), GetAgent());
        }
        
        public void StartLearning()
        {
            OnEpisodeBegin();
        }
        
        public override void OnEpisodeBegin()
        {
            InEpisode = true;
        }
        
        public override void CollectObservations(global::Unity.MLAgents.Sensors.VectorSensor sensor)
        {
            OnBeforeCollectObservation();
            GetObservation().AddObservationToVectorSensor(sensor);
        }
        
        public override void OnActionReceived(ActionBuffers actions)
        {
            var action = GetAction(actions);
            action.ApplyAction(GetAgent());
            OnAfterApplyAction(action);
        }
        
        public override void Heuristic(in ActionBuffers actionsOut)
        {
            var actionsOutContinuousActions = actionsOut.ContinuousActions;
            var actionsOutDiscreteActions = actionsOut.DiscreteActions;
            Heuristic(GetHeuristicData(), in actionsOutContinuousActions, in actionsOutDiscreteActions);
        }
        
        private void FixedUpdate()
        {
            if (InEpisode)
            {
                if (!_tickOnClick)
                {
                    RequestDecision();
                    RequestAction(); 
                }
            }
        }
        
        private void Update()
        {
            if (InEpisode)
            {
                if (_tickOnClick)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        RequestDecision();
                        RequestAction();
                    }
                }
            }
        }
        
        public void MyEndEpisode()
        {
            InEpisode = false;
            EndEpisode();
        }
        
        private T GetAgent()
        {
            return (T)this;
        }
    }
}