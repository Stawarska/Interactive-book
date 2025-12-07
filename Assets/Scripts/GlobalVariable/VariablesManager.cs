using System.Collections.Generic;
using UnityEngine;

namespace GlobalVariable
{
    public class VariablesManager : MonoBehaviour
    {
        
        public static VariablesManager Instance;
        public Dictionary<string, IGlobalVariableType> Variables { get; private set; }

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            
            var variables = GlobalVariables.instance.Variables;
            for (var i = 0; i < variables.Count; i++)
                Variables.Add(variables[i].variableName, variables[i].variableType);
        }
    }
}