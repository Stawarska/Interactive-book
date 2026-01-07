using System;
using UnityEngine;

namespace GlobalVariable.Conditions
{
    [Serializable]
    public class LessThanCondition<T> : IVariableCondition<T> where T : IComparable<T>
    {
        [SerializeField] protected T value;
        public bool CheckCondition(T other)
        {
            return other.CompareTo(value) < 0;
        }
    }
    
    [Serializable] public class LessThanConditionInt : LessThanCondition<int> { }
    [Serializable] public class LessThanConditionFloat : LessThanCondition<float> { }
}