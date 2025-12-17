using System;
using UnityEngine;

namespace GlobalVariable.Actions
{
    [Serializable]
    public class AddAction<T> : IVariableAction<T>
    {
        [SerializeField] protected T value;
        
        public virtual T Perform(T other)
        {
           return other;
        }
    }
    
    [Serializable]
    public class AddActionInt : AddAction<int>, IIntAction
    {
        public override int Perform(int other)
        {
            other += value;
            return base.Perform(other);
        }
    }

    [Serializable]
    public class AddActionFloat : AddAction<float>, IFloatAction
    {
        public override float Perform(float other)
        {
            other += value;
            return base.Perform(other);
        }
    }
}