using System;
using UnityEngine;

namespace GlobalVariable.Conditions
{
    [Serializable]
    public abstract class EqualsCondition<T> : IVariableCondition<T> where T : IEquatable<T>
    {
        [SerializeField] protected T value;
        public bool CheckCondition(T other)
        {
            return other.Equals(value);
        }
    }

    [Serializable]
    public class EqualsConditionBool : EqualsCondition<bool>, IBoolCondition { }
    
    [Serializable]
    public class EqualsConditionInt : EqualsCondition<int>, IIntCondition { }
    
    [Serializable]
    public class EqualsConditionString : EqualsCondition<string>, IStringCondition { }
    
    [Serializable]
    public class EqualsConditionFloat : EqualsCondition<float>, IFloatCondition { }
}