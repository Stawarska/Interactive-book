namespace GlobalVariable.Conditions
{
    public interface IVariableCondition<in T> : IVariableCondition
    {
        public bool CheckCondition(T other);
        
        bool IVariableCondition.CheckCondition(IVariable variable)
        {
            return CheckCondition(variable.GetValue<T>());
        }
    }

    public interface IVariableCondition
    {
        public bool CheckCondition(IVariable other);
    }
}