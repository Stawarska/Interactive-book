using System;
using UnityEngine;

namespace GlobalVariable
{
    public interface IVariable
    {
        public Type VariableType { get; }
        public T GetValue<T>();
        public void SetValue<T>(T value);
    }

    [Serializable]
    public abstract class Variable<T> : IVariable
    {
        [field: SerializeField] public T Value { get; set; }
        [field: SerializeField] public T DefaultValue { get; set; }

        public Type VariableType => typeof(T);

        T1 IVariable.GetValue<T1>() 
        {
            if(Value is T1 value)
                return value;
            
            throw new TypeAccessException();
        }

        public void SetValue<T1>(T1 value)
        {
            if (value is not T newValue)
                throw new TypeAccessException();

            Value = newValue;
        }
    }
}