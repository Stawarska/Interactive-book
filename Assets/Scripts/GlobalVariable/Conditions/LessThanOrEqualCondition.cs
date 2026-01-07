using System;
using UnityEngine;

namespace GlobalVariable.Conditions
{
    [Serializable]
    public class LessThanOrEqualCondition<T> : IVariableCondition<T> where T : IComparable<T>
    {
        [SerializeField] protected T value;
        public bool CheckCondition(T other)
        {
            return other.CompareTo(value) <= 0;
        }
    }
    
    [Serializable] public class LessThanOrEqualConditionInt : LessThanOrEqualCondition<int> { }
    [Serializable] public class LessThanOrEqualConditionFloat : LessThanOrEqualCondition<float> { }
}