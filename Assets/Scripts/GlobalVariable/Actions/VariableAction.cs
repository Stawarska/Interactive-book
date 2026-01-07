using System;
using UnityEngine;

namespace GlobalVariable.Actions
{
    [Serializable]
    public class VariableAction 
    {
        [SerializeReference] private VariableOperation<IVariableAction> variableAction = new();
        
        public void Perform()
        {
            var variable = VariablesManager.Instance.Variables[variableAction.VariableID];
            variableAction.Operation?.Perform(variable);
        }
        
        #if UNITY_EDITOR
        public static class SerializationHelper
        {
            public static string VariableActionName => nameof(variableAction);
        }
        #endif
    }
}