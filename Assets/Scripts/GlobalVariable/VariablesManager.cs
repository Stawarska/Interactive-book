using System;
using System.Collections.Generic;
using UnityEngine;

namespace GlobalVariable
{
    public class VariablesManager : MonoBehaviour
    {
        public static VariablesManager Instance;
        public Dictionary<string, IVariable> Variables { get; private set; } = new();
        public Action<IVariable> OnVariableChanged;

        [SerializeField] private GlobalVariables globalVariables;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            
            var variables = globalVariables.Variables;
            
            for (var i = 0; i < variables.Count; i++)
                Variables.Add(variables[i].variableName, variables[i].variable.Clone());
        }
        
        public IVariable GetVariableFromName(string variableName)
        {
            return Variables.GetValueOrDefault(variableName);
        }
    }
}