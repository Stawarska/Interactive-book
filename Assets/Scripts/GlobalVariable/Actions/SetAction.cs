using System;
using UnityEngine;

namespace GlobalVariable.Actions
{
    [Serializable]
    public class SetAction<T> : IVariableAction<T>
    {
        [SerializeField] protected T value;

        public void Perform(Variable<T> variable)
        {
            variable.SetValue(value);
        }
    }
    
    [Serializable] public class SetActionInt : SetAction<int> { }
    [Serializable] public class SetActionFloat : SetAction<float> { }
    [Serializable] public class SetActionBool : SetAction<bool> { }
    [Serializable] public class SetActionString : SetAction<string> { }
}