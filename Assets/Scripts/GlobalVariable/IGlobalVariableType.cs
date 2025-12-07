using System;
using UnityEngine;

namespace GlobalVariable
{
    public interface IGlobalVariableType
    {
        public Type VariableType { get; }
        public T GetValue<T>();
    }

    [Serializable]
    public abstract class GlobalVariableType<T> : IGlobalVariableType
    {
        
        [field: SerializeField] public T Value { get; set; }
        [field: SerializeField] public T DefaultValue { get; set; }

        public Type VariableType => typeof(T);

        T1 IGlobalVariableType.GetValue<T1>() 
        {
            if(Value is T1 value)
                return value;
            
            throw new TypeAccessException();
        }
    }
}