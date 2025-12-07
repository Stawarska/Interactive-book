using System;
using UnityEngine;

namespace GlobalVariable.Conditions
{
    [Serializable]
    public class NotEqualsCondition<T> : IVariableCondition<T> where T : IEquatable<T>
    {
        [SerializeField] protected T value;
        public bool CheckCondition(T other)
        {
            return !other.Equals(value);
        }
    }
    
    [Serializable]
    public class NotEqualsConditionBool : NotEqualsCondition<bool>, IBoolCondition { }
    
    [Serializable]
    public class NotEqualsConditionInt : NotEqualsCondition<int>, IIntCondition { }
    
    [Serializable]
    public class NotEqualsConditionString : NotEqualsCondition<string>, IStringCondition { }
    
    [Serializable]
    public class NotEqualsConditionFloat : NotEqualsCondition<float>, IFloatCondition { }
}