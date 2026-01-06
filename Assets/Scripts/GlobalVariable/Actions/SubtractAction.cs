using System;
using MiscUtil;
using UnityEngine;

namespace GlobalVariable.Actions
{
    [Serializable]
    public abstract class SubtractAction<T> : IVariableAction<T>
    {
        [SerializeField] protected T value;
        
        public void Perform(Variable<T> variable)
        {
            variable.SetValue(Operator<T>.Subtract(variable.Value, value));
        }
    }
    
    [Serializable] public class SubtractActionInt : SubtractAction<int> { }
    [Serializable] public class SubtractActionFloat : SubtractAction<float> { }
}