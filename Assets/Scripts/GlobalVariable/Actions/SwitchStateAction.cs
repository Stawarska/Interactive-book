using System;

namespace GlobalVariable.Actions
{
    [Serializable]
    public abstract class SwitchStateAction<T> : IVariableAction<T>
    {
        public abstract void Perform(Variable<T> variable);
    }
    
    [Serializable]
    public class SwitchStateActionBool : SwitchStateAction<bool>
    {
        public override void Perform(Variable<bool> other)
        {
            other.SetValue(!other.Value);
        }
    }
}