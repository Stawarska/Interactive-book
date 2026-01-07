using System;
using UnityEngine;

namespace GlobalVariable.Conditions
{
    [Serializable]
    public class VariableCondition 
    {
        [SerializeReference] private VariableOperation<IVariableCondition> variableCondition = new();
        
        public bool Check()
        {
            var variable = VariablesManager.Instance.Variables[variableCondition.VariableID];
            return variableCondition.Operation.CheckCondition(variable);
        }
        
        #if UNITY_EDITOR
        public static class SerializationHelper
        {
            public static string VariableConditionName => nameof(variableCondition);
        }
        #endif
    }
}