namespace GlobalVariable.Conditions
{
    public interface IVariableCondition<in T> 
    {
        public bool CheckCondition(T other);
    }
}