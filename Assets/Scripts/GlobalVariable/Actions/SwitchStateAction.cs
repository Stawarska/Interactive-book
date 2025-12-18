using System;

namespace GlobalVariable.Actions
{
    [Serializable]
    public class SwitchStateAction<T> : IVariableAction<T>
    {
        public virtual T Perform(T other)
        {
            return other;
        }
    }
    
    [Serializable]
    public class SwitchStateActionBool : SwitchStateAction<bool>, IBoolAction
    {
        public override bool Perform(bool other)
        {
            other = !other;
            return base.Perform(other);
        }
    }
}