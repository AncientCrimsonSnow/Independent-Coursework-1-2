using Unity.MLAgents.Policies;

namespace Extensions.Ml_Agents
{
    public static class BehaviorUtilities
    {
        public static void SetObservationParameter<Agent, Observation, Action, ActionData>(this BehaviorParameters value, Observation observation, Action action, Agent agent)
        where Observation : struct, IObservation<Observation>
        where Action : Action<Action, ActionData, Observation, Agent>, new()
        where Agent : Agent<Agent, Observation, Action, ActionData>
        where ActionData : struct, IActionData<ActionData>
        {
            value.BrainParameters.VectorObservationSize = observation.GetObservationSize();
            var brainParametersActionSpec = value.BrainParameters.ActionSpec;
            brainParametersActionSpec.NumContinuousActions = action.GetNumberOfContinuesActions();
            brainParametersActionSpec.BranchSizes = action.GetBranchSizes(agent);
            value.BrainParameters.ActionSpec = brainParametersActionSpec;
        }
    }
}