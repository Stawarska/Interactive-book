using System;
using UnityEngine;

namespace GlobalVariable.Actions
{
    [Serializable]
    public class SetAction<T> : IVariableAction<T>
    {
        [SerializeField] protected T value;
        public virtual T Perform(T other)
        {
            return other;
        }
    }
    
    [Serializable]
    public class SetActionInt : SetAction<int>, IIntAction
    {
        public override int Perform(int other)
        {
            other = value;
            return base.Perform(other);
        }
    }

    [Serializable]
    public class SetActionFloat : SetAction<float>, IFloatAction
    {
        public override float Perform(float other)
        {
            other = value;
            return base.Perform(other);
        }
    }
    
    [Serializable]
    public class SetActionBool : SetAction<bool>, IBoolAction
    {
        public override bool Perform(bool other)
        {
            other = value;
            return base.Perform(other);
        }
    }
    
    [Serializable]
    public class SetActionString : SetAction<string>, IStringAction
    {
        public override string Perform(string other)
        {
            other = value;
            return base.Perform(other);
        }
    }
}