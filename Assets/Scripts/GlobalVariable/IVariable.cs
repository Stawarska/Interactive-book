using System;
using UnityEngine;

namespace GlobalVariable
{
    public interface IVariable
    {
        public T GetValue<T>();
        public void SetValue<T>(T value);
        public IVariable Clone();
    }

    [Serializable]
    public abstract class Variable<T> : IVariable
    {
        public delegate void OnValueChangedDelegate(T newValue, T oldValue);
        public event OnValueChangedDelegate OnValueChanged;
        [field: SerializeField] public T Value { get; private set; }

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

            var oldValue = Value;
            Value = newValue;
            OnValueChanged?.Invoke(Value, oldValue);
            VariablesManager.Instance.OnVariableChanged?.Invoke(this);
        }

        public virtual IVariable Clone()
        {
            var newVariable = (Variable<T>)Activator.CreateInstance(GetType());
            newVariable.Value = Value;
            return newVariable;
        }
    }
}