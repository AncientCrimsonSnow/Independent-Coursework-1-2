namespace Extensions.Ml_Agents.VectorSensor
{
    public abstract class VectorSensorAgent<T, O, A, D> : Agent<T, O, A, D>
    where T : VectorSensorAgent<T, O, A, D>
    where O : struct, IObservation<O>
    where A : Action<A, D, O, T>, new()
    where D : struct, IActionData<D>
    {
        
    }
}