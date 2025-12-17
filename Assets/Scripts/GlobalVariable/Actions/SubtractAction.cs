using System;
using UnityEngine;

namespace GlobalVariable.Actions
{
    [Serializable]
    public class SubtractAction<T> : IVariableAction<T>
    {
        [SerializeField] protected T value;
        public T Perform(T other)
        {
            return other;
        }
    }
    
    [Serializable]
    public class SubtractActionInt : AddAction<int>, IIntAction
    {
        public override int Perform(int other)
        {
            other -= value;
            return base.Perform(other);
        }
    }

    [Serializable]
    public class SubtractActionFloat : AddAction<float>, IFloatAction
    {
        public override float Perform(float other)
        {
            other -= value;
            return base.Perform(other);
        }
    }
}