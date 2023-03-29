namespace Extensions.Ml_Agents
{
    public interface IActionData<T>
    where T : struct, IActionData<T>
    {
    }
}