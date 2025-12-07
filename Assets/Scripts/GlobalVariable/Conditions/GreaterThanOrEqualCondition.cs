using System;
using UnityEngine;

namespace GlobalVariable.Conditions
{
    [Serializable]
    public class GreaterThanOrEqualCondition<T> : IVariableCondition<T> where T : IComparable<T>
    {
        [SerializeField] protected T value;
        public bool CheckCondition(T other)
        {
            return other.CompareTo(value) >= 0;
        }
    }
    
    [Serializable]
    public class GreaterThanOrEqualConditionInt : GreaterThanOrEqualCondition<int>, IIntCondition { }
    
    [Serializable]
    public class GreaterThanOrEqualConditionFloat : GreaterThanOrEqualCondition<float>, IFloatCondition { }
}