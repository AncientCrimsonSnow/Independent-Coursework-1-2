using Unity.MLAgents.Actuators;

namespace Extensions.Ml_Agents
{
    public abstract class Action<T, D, O, A>
        where T : Action<T, D, O, A>, new()
        where D : struct, IActionData<D>
        where O : struct, IObservation<O>
        where A : Agent<A, O, T, D>
    {
        protected readonly D data;
        public abstract int GetNumberOfContinuesActions();
        public abstract int[] GetBranchSizes(A agent);
        public abstract void ApplyAction(A agent);
        protected abstract D GetData(float[] continuesActions, int[] discreteActions, A agent);
        
        protected Action(ActionBuffers actions, A agent)
        {
            var continuesActions = actions.ContinuousActions.Array;
            var discreteActions = actions.DiscreteActions.Array;
            if (continuesActions.Length < GetNumberOfContinuesActions() ||
                discreteActions.Length < GetBranchSizes(agent).Length)
                data = new D();
            else
                data = GetData(continuesActions, discreteActions, agent);
        }
        protected Action() { }
    }
}