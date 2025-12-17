namespace GlobalVariable.Actions
{
    public interface IVariableAction<T>
    {
        public T Perform(T other);
    }
}