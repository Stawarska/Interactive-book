namespace GlobalVariable.Conditions
{
    public interface IBoolCondition : IVariableCondition<bool> { }
    public interface IStringCondition : IVariableCondition<string> { }
    public interface IIntCondition : IVariableCondition<int> { }
    public interface IFloatCondition : IVariableCondition<float> { }
}