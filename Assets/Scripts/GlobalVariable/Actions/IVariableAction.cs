namespace GlobalVariable.Actions
{
    public interface IVariableAction<T> : IVariableAction
    {
        public void Perform(Variable<T> variable);

        void IVariableAction.Perform(IVariable variable)
        {
            Perform((Variable<T>)variable);
        }
    }

    public interface IVariableAction
    {
        void Perform(IVariable variable);
    }
}