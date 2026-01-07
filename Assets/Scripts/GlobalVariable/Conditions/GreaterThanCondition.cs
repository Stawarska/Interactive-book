using System;
using UnityEngine;

namespace GlobalVariable.Conditions
{
    [Serializable]
    public class GreaterThanCondition<T> : IVariableCondition<T> where T : IComparable<T>
    {
        [SerializeField] protected T value;
        public bool CheckCondition(T other)
        {
            return other.CompareTo(value) > 0;
        }
    }
    
    [Serializable] public class GreaterThanConditionInt : GreaterThanCondition<int> { }
    [Serializable] public class GreaterThanConditionFloat : GreaterThanCondition<float> { }
}