using System;
using MiscUtil;
using UnityEngine;

namespace GlobalVariable.Actions
{
    [Serializable]
    public class AddAction<T> : IVariableAction<T>
    {
        [SerializeField] protected T value;
        
        public void Perform(Variable<T> variable)
        {
            variable.SetValue(Operator<T>.Add(value, variable.Value));
        }
    }
    
    [Serializable] public class AddActionInt : AddAction<int> { }
    [Serializable] public class AddActionFloat : AddAction<float> { }
}